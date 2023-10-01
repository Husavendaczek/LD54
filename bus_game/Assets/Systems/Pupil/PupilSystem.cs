﻿using SystemBase.Core.GameSystems;
using SystemBase.Core.StateMachineBase;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Pupil
{
    [GameSystem]
    public class PupilSystem : GameSystem<PupilComponent>
    {
        public override void Register(PupilComponent component)
        {
            component.CurrentTarget = GameObject.Find("bus_target");
            Debug.Log(component.CurrentTarget);
            component.rigidbody2D = component.GetComponent<Rigidbody2D>();

            SystemFixedUpdate(component).Where(pupil => pupil.CurrentTarget)
                .Where(p => p.State == PupilState.Outside)
                .Subscribe(MoveTowardsTarget)
                .AddTo(component);

            component.OnCollisionStay2DAsObservable()
                .Where(collision => collision.gameObject.CompareTag("pupil"))
                .Subscribe(coll => PushOther(component, coll))
                .AddTo(component);

        }

        private void PushOther(PupilComponent me, Collision2D other)
        {
            other.gameObject.GetComponent<Rigidbody2D>()
                .AddForce((other.transform.position - me.transform.position).normalized * 200f);
        }

        private void MoveTowardsTarget(PupilComponent pupil)
        {
            if (TargetReached(pupil))
            {
                pupil.State = PupilState.Inside;
                pupil.CurrentTarget = null;
                pupil.rigidbody2D.velocity = Vector2.zero;
                pupil.rigidbody2D.gravityScale = 1f;
            }
            else
            {
                var targetDirection = pupil.CurrentTarget.transform.position - pupil.transform.position;
                pupil.rigidbody2D.AddForce(targetDirection.normalized * 5f);
            }
        }

        private bool TargetReached(PupilComponent component)
        {
            return (component.CurrentTarget.transform.position - component.transform.position).magnitude < 1f;
        }
    }

    public enum PupilState
    {
        Outside,
        Inside
    }
}