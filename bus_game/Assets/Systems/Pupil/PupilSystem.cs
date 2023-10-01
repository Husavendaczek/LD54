using SystemBase.Core.GameSystems;

namespace Systems.Pupil
{
    [GameSystem]
    public class PupilSystem : GameSystem<PupilComponent>
    {
        public override void Register(PupilComponent component)
        {
            
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