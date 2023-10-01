using SystemBase.Core.Components;
using SystemBase.Core.GameSystems;

namespace Systems.Bus
{
    [GameSystem]
    public class BusSystem : GameSystem<BusComponent>
    {
        public override void Register(BusComponent component)
        {
            
        }
    }

    public enum BusState
    {
        ComingIn,
        StoppedOpen,
        StoppedClosed,
        DrivingAway
    }
    
    public class BusComponent : GameComponent
    {
        public BusState State { get; set; } = BusState.ComingIn;
    }
}