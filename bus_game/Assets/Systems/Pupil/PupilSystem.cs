using SystemBase.Core.GameSystems;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Pupil
{
    [GameSystem]
    public class PupilSystem : GameSystem<PupilComponent, PupilSpawnerComponent>
    {
        public override void Register(PupilComponent component)
        {
            component.CurrentTarget = GameObject.Find("bus_target");
            Debug.Log(component.CurrentTarget);
            component.rigidbody2D = component.GetComponent<Rigidbody2D>();

            SystemFixedUpdate(component).Where(pupil => pupil.CurrentTarget)
                .Where(p => p.State == PupilState.Outside)
                .Subscribe(Animate)
                .AddTo(component);

            component.OnCollisionStay2DAsObservable()
                .Where(collision => collision.gameObject.CompareTag("pupil"))
                .Subscribe(coll => PushOther(component, coll))
                .AddTo(component);

        }

        private static void PushOther(Component me, Collision2D other)
        {
            other.gameObject.GetComponent<Rigidbody2D>()
                .AddForce((other.transform.position - me.transform.position).normalized * 200f);
        }

        private void Animate(PupilComponent pupil)
        {
            
            if (TargetReached(pupil))
            {
                var vel = pupil.rigidbody2D.velocity;
                var angle = Mathf.Atan2(vel.y, vel.x ) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
                pupil.sprite.transform.rotation = Quaternion.RotateTowards(pupil.transform.rotation, targetRotation, 360);
                
                pupil.State = PupilState.Inside;
                pupil.CurrentTarget = null;
                pupil.rigidbody2D.velocity = Vector2.zero;
                pupil.rigidbody2D.gravityScale = 1f;
                
            }
            else
            {
                var targetDirection = pupil.CurrentTarget.transform.position - pupil.transform.position;
                
                var angle = Mathf.Atan2(targetDirection.y, targetDirection.x ) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
                pupil.sprite.transform.rotation = Quaternion.RotateTowards(pupil.transform.rotation, targetRotation, 360);
                
                pupil.rigidbody2D.AddForce(targetDirection.normalized * pupil.speed);
                if(pupil.rigidbody2D.velocity.magnitude > pupil.speed)
                    pupil.rigidbody2D.velocity = pupil.rigidbody2D.velocity.normalized * pupil.speed;
            }
            
            
        }

        private bool TargetReached(PupilComponent component)
        {
            return (component.CurrentTarget.transform.position - component.transform.position).magnitude < 1f;
        }

        public override void Register(PupilSpawnerComponent component)
        {
            Observable.Interval(System.TimeSpan.FromSeconds(1f / component.spawnSpeed))
                .Subscribe(_ => SpawnPupil(component))
                .AddTo(component);
        }

        private void SpawnPupil(PupilSpawnerComponent component)
        {
            Object.Instantiate(component.pupilPrefab, component.transform.position, Quaternion.identity, component.transform);
        }
    }

    public enum PupilState
    {
        Outside,
        Inside
    }
}