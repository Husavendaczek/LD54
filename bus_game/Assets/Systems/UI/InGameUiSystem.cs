using SystemBase.Core.GameSystems;
using Systems.Bus.Events;
using Systems.Score;
using UniRx;
using UnityEngine;

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

            MessageBroker.Default.Receive<BusDoorCloseEvent>().Subscribe(_ => AnimateCloseButton(component))
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

        private void AnimateCloseButton(UiComponent uiComponent)
        {
            var animation = uiComponent.GetComponentInChildren<Animator>();
            animation.Play("close_button");
        }
    }
}