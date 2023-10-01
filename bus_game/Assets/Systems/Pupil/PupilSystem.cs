using SystemBase.Core.GameSystems;
using UniRx;
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
        }

        private void MoveTowardsTarget(PupilComponent pupil)
        {
            if (TargetReached(pupil))
            {
                pupil.State = PupilState.Inside;
                pupil.CurrentTarget = null;
                pupil.rigidbody2D.velocity = Vector2.zero;
            }
            else
            {
                var targetDirection = pupil.CurrentTarget.transform.position - pupil.transform.position;
                pupil.rigidbody2D.AddForce(targetDirection.normalized * 10f);
            }
        }

        private bool TargetReached(PupilComponent component)
        {
            return (component.CurrentTarget.transform.position - component.transform.position).magnitude < 0.1f;
        }
    }

    public enum PupilState
    {
        Outside,
        Inside
    }
}