using SystemBase.Core.Components;
using UniRx;
using UnityEngine;

namespace Systems.Bus
{
    public class BusComponent : GameComponent
    {
        public ReactiveProperty<BusState> State { get; set; } = new(BusState.ComingIn);
        public BusPositionsComponent Positions { get; set; }
        public Rigidbody2D Body { get; set; }
        public GameObject doorCollider;
        public GameObject targetForPupils;
    }
}