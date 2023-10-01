using System;
using SystemBase.Core.GameSystems;
using SystemBase.Core.StateMachineBase;
using SystemBase.GameState.States;
using SystemBase.Utils;
using Systems.Bus.Events;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Bus
{
    [GameSystem]
    public class BusSystem : GameSystem<BusComponent, BusPositionsComponent>
    {
        public override void Register(BusComponent component)
        {
            component.Body = component.GetComponent<Rigidbody2D>();
            
            component.State.Where(s => s is BusState.Despawn)
                .Subscribe(_ => MessageBroker.Default.Publish(new BusDespawnedEvent()))
                .AddTo(component);

            SystemFixedUpdate(component)
                .Where(bus => bus.State.Value is BusState.ComingIn)
                .Subscribe(_ => DriveToStop(component))
                .AddTo(component);

            component.State
                .Where(s => s is BusState.Stopped)
                .Subscribe(_ => OpenDoors(component))
                .AddTo(component);

            component.State
                .Where(s => s is BusState.StoppedOpen)
                .Subscribe(_ => WaitForClose(component))
                .AddTo(component);
            
        }

        private static void WaitForClose(BusComponent component)
        {
            SystemUpdate(component)
                .Subscribe(c =>
                {
                    if (!Input.GetKeyDown(KeyCode.Space)) return;
                    
                    MessageBroker.Default.Publish(new BusDoorClosedEvent());
                    c.State.Value = BusState.StoppedClosed;
                })
                .AddTo(component);

        }

        private void OpenDoors(BusComponent component)
        {
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    MessageBroker.Default.Publish(new BusDoorOpenedEvent());
                    component.State.Value++;
                })
                .AddTo(component);
        }

        private void DriveToStop(BusComponent component)
        {
            var target = component.Positions.stop;
            var roadLeft = (float3)(target.transform.position - component.transform.position);
            component.Body.velocity = roadLeft.xy * 10f;

            if (math.length(roadLeft) > 0.5f) return;
            
            component.State.Value += 1;
            component.Body.velocity = Vector2.zero;
        }

        public override void Register(BusPositionsComponent component)
        {
            IoC.Game.gameStateContext.CurrentState
                .Where(state => state is Running)
                .First()
                .Subscribe(_ => SpawnBus(component))
                .AddTo(component);

            MessageBroker.Default.Receive<BusDespawnedEvent>()
                .Subscribe(_ => SpawnBus(component))
                .AddTo(component);
        }

        private static void SpawnBus(BusPositionsComponent component)
        {
            var busObject = Object.Instantiate(component.busPrefab, component.start.transform.position, Quaternion.identity);
            var bus = busObject.GetComponent<BusComponent>();
            bus.Positions = component;
        }
    }

    public enum BusState : int
    {
        ComingIn,
        Stopped,
        StoppedOpen,
        StoppedClosed,
        DrivingAway,
        Despawn
    }
}