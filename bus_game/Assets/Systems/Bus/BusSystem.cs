using SystemBase.Core.GameSystems;
using SystemBase.GameState.States;
using SystemBase.Utils;
using Systems.Bus.Events;
using UniRx;
using UnityEngine;

namespace Systems.Bus
{
    [GameSystem]
    public class BusSystem : GameSystem<BusComponent, BusPositionsComponent>
    {
        public override void Register(BusComponent component)
        {
            component.State.Where(s => s is BusState.Despawn)
                .Subscribe(_ => MessageBroker.Default.Publish(new BusDespawnedEvent()))
                .AddTo(component);

            SystemFixedUpdate(component)
                .Where(bus => bus.State.Value is BusState.ComingIn)
                .Subscribe(_ => DriveToStop(component))
                .AddTo(component);
        }

        private void DriveToStop(BusComponent component)
        {
            var target = component.Positions.stop;
            var step = Vector3.Lerp(component.transform.position, target.transform.position, 0.2f);
            component.transform.position += step;
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

    public enum BusState
    {
        ComingIn,
        StoppedOpen,
        StoppedClosed,
        DrivingAway,
        Despawn
    }
}