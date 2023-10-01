using System;
using SystemBase.Core.GameSystems;
using SystemBase.GameState.States;
using SystemBase.Utils;
using Systems.Bus.Events;
using Systems.Score;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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

            component.State
                .Where(s => s is BusState.StoppedClosed)
                .Subscribe(_ => CloseDoor(component))
                .AddTo(component);

            SystemFixedUpdate(component)
                .Where(bus => bus.State.Value is BusState.DrivingAway)
                .Subscribe(MoveAway)
                .AddTo(component);
        }

        private void CloseDoor(BusComponent component)
        {
            var animation = component.GetComponentInChildren<Animator>();
            animation.Play("bus_open_door");

            Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(_ => component.State.Value = BusState.DrivingAway);
        }

        private void MoveAway(BusComponent component)
        {
            var target = component.Positions.end;
            var roadLeft = (float3)(target.transform.position - component.transform.position);
            component.Body.velocity = roadLeft.xy * 2f;

            if (component.transform.position.y > target.transform.position.y - 0.5f)
            {
                component.State.Value += 1;
                component.Body.velocity = Vector2.zero;

                var spawner = component.Positions;
                Object.Destroy(component.gameObject);
                
                SpawnBus(spawner);
                MessageBroker.Default.Publish(new BusDespawnedEvent());
            }
        }

        private static void WaitForClose(BusComponent component)
        {
            component.doorCollider.SetActive(false);
            SystemUpdate(component)
                .Subscribe(c =>
                {
                    if (!Input.GetKeyDown(KeyCode.Space)) return;

                    MessageBroker.Default.Publish(new BusDoorClosedEvent());
                    c.State.Value = BusState.StoppedClosed;
                    component.doorCollider.SetActive(true);
                })
                .AddTo(component);
        }

        private void OpenDoors(BusComponent component)
        {
            var animation = component.GetComponentInChildren<Animator>();
            animation.Play("bus_close_door");

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
            component.Body.velocity = roadLeft.xy * 2f;

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
        }

        private static void SpawnBus(BusPositionsComponent component)
        {
            var busObject = Object.Instantiate(component.busPrefab, component.start.transform.position,
                Quaternion.identity);
            var bus = busObject.GetComponent<BusComponent>();
            bus.Positions = component;
            
            ScoreBoard.SetTargetPassengers(Random.Range(2, ScoreBoard.MaximumPassengers-10));
        }
    }

    public enum BusState
    {
        ComingIn,
        Stopped,
        StoppedOpen,
        StoppedClosed,
        DrivingAway,
        Despawn
    }
}