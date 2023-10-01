using SystemBase.Core.Components;
using UnityEngine;

namespace Systems.Pupil
{
    public class PupilComponent : GameComponent
    {
        public PupilState State { get; set; } = PupilState.Outside;
        public GameObject CurrentTarget { get; set; }
        public Rigidbody2D rigidbody2D;
        public GameObject sprite;
        public float speed;
    }
}