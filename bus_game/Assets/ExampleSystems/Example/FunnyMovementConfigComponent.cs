using SystemBase.Core.Components;
using SystemBase.Core.StateMachineBase;
using UniRx;

namespace ExampleSystems.Example
{
    public class FunnyMovementConfigComponent : GameComponent
    {
        public FloatReactiveProperty Speed = new(10);
        public StateContext<FunnyMovementComponent> MovementState = new();
    }
}