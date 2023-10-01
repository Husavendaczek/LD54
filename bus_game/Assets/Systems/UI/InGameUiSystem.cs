using SystemBase.Core.GameSystems;
using Systems.Score;
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class InGameUiSystem : GameSystem<UiComponent>
    {
        public override void Register(UiComponent component)
        {
            ScoreBoard.TargetPassengers
                .Subscribe(i => UpdateTarget(i, component))
                .AddTo(component);

            ScoreBoard.Passengers
                .Subscribe(i => UpdatePassengers(i, component))
                .AddTo(component);
        }

        private void UpdatePassengers(int i, UiComponent uiComponent)
        {
            uiComponent.currentNumber.text = i.ToString();
        }

        private void UpdateTarget(int i, UiComponent uiComponent)
        {
            uiComponent.targetNumber.text = i.ToString();
        }
    }
}