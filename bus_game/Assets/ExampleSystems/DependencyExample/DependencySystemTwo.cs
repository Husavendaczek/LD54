using ExampleSystems.Example;
using SystemBase.Core.GameSystems;

namespace ExampleSystems.DependencyExample
{
    [GameSystem(typeof(DependencySystemMaster), typeof(DependencySystemThree))]
    public class DependencySystemTwo : GameSystem<FunnyMovementComponent>
    {
        public override void Register(FunnyMovementComponent component)
        {
        }
    }
}