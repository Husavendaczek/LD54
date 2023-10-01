using System;
using SystemBase.Core.GameSystems;
using Systems.Bus;
using Systems.Bus.Events;
using Systems.Score;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.Pupil
{
    [GameSystem]
    public class PupilSystem : GameSystem<PupilComponent, PupilSpawnerComponent, BusComponent>
    {
        public override void Register(PupilComponent component)
        {
            component.CurrentTarget = GameObject.Find("bus_target");
            component.rigidbody2D = component.GetComponent<Rigidbody2D>();

            SystemFixedUpdate(component).Where(pupil => pupil.CurrentTarget)
                .Where(p => p.State == PupilState.Outside)
                .Subscribe(Animate)
                .AddTo(component);

            component.OnCollisionStay2DAsObservable()
                .Where(collision => collision.gameObject.CompareTag("pupil"))
                .Subscribe(coll => PushOther(component, coll))
                .AddTo(component);
            
            Observable.Timer(TimeSpan.FromSeconds(45))
                .Where(_ => component.State == PupilState.Outside)
                .Subscribe(_ => Object.Destroy(component.gameObject))
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
                var angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
                pupil.sprite.transform.rotation =
                    Quaternion.RotateTowards(pupil.transform.rotation, targetRotation, 360);

                pupil.State = PupilState.Inside;
                pupil.CurrentTarget = null;
                pupil.rigidbody2D.velocity = Vector2.zero;
                pupil.rigidbody2D.gravityScale = 1f;
                
                ScoreBoard.AddPassenger();
            }
            else
            {
                var targetDirection = pupil.CurrentTarget.transform.position - pupil.transform.position;

                var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
                pupil.sprite.transform.rotation =
                    Quaternion.RotateTowards(pupil.transform.rotation, targetRotation, 360);

                pupil.rigidbody2D.AddForce(targetDirection.normalized * pupil.speed);
                if (pupil.rigidbody2D.velocity.magnitude > pupil.speed)
                    pupil.rigidbody2D.velocity = pupil.rigidbody2D.velocity.normalized * pupil.speed;

                pupil.rigidbody2D.AddForce(targetDirection.normalized * 5f);
            }
        }

        private bool TargetReached(PupilComponent component)
        {
            return (component.CurrentTarget.transform.position - component.transform.position).magnitude < 1f;
        }

        public override void Register(PupilSpawnerComponent component)
        {
            Observable.Interval(TimeSpan.FromSeconds(1f / component.spawnSpeed))
                .Where(_ => component.IsSpawning)
                .Subscribe(_ => SpawnPupil(component))
                .AddTo(component);

            MessageBroker.Default.Receive<BusDoorOpenedEvent>().Subscribe(_ => component.IsSpawning = true)
                .AddTo(component);
            MessageBroker.Default.Receive<BusDoorClosedEvent>().Subscribe(_ => component.IsSpawning = false)
                .AddTo(component);

            MessageBroker.Default.Receive<BusDespawnedEvent>()
                .Subscribe(_ => ResetPupils(component))
                .AddTo(component);
        }

        private static void ResetPupils(Component component)
        {
            foreach (Transform child in component.transform)
            {
                var pupil = child.GetComponent<PupilComponent>();
                if (pupil.State == PupilState.Inside)
                    Object.Destroy(child.gameObject);
            }
        }

        private static void SpawnPupil(PupilSpawnerComponent component)
        {
            if(component.transform.childCount > ScoreBoard.MaximumSpawnedPassengers) return;
            
            var pupil = Object.Instantiate(component.pupilPrefab, component.transform.position, Quaternion.identity,
                component.transform);
            pupil.GetComponentInChildren<SpriteRenderer>().sprite =
                component.sprites[Random.Range(0, component.sprites.Length - 1)];
        }

        public override void Register(BusComponent component)
        {
            var pupils = GameObject.FindGameObjectsWithTag("pupil");
            foreach (var pupil in pupils)
            {
                var pupilComponent = pupil.GetComponent<PupilComponent>();
                pupilComponent.CurrentTarget = component.targetForPupils;
                pupilComponent.State = PupilState.Outside;
                pupilComponent.rigidbody2D.gravityScale = 0f;
            }
        }
    }

    public enum PupilState
    {
        Outside,
        Inside
    }
}