using SystemBase.Core.Components;
using UniRx;

namespace Systems.Bus
{
    public class BusComponent : GameComponent
    {
        public ReactiveProperty<BusState> State { get; set; } = new(BusState.ComingIn);
        public BusPositionsComponent Positions { get; set; }
    }
}