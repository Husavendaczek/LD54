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
        private int targetCount = 0;
        private int currentCount = 0;

        public override void Register(UiComponent component)
        {
            ShowScore(component, false);

            ScoreBoard.TargetPassengers
                .Subscribe(i => UpdateTarget(i, component))
                .AddTo(component);

            ScoreBoard.Passengers
                .Subscribe(i => UpdatePassengers(i, component))
                .AddTo(component);

            MessageBroker.Default.Receive<BusDoorCloseEvent>().Subscribe(_ => AnimateCloseButton(component))
                .AddTo(component);

            MessageBroker.Default.Receive<BusDespawnedEvent>().Subscribe(_ =>ShowScore(component, false))
                .AddTo(component);
        }

        private void UpdatePassengers(int i, UiComponent uiComponent)
        {
            currentCount = i;
            uiComponent.currentNumber.text = i.ToString();
        }

        private void UpdateTarget(int i, UiComponent uiComponent)
        {
            targetCount = i;
            uiComponent.targetNumber.text = i.ToString();
        }

        private void AnimateCloseButton(UiComponent uiComponent)
        {
            var animation = uiComponent.GetComponentInChildren<Animator>();
            animation.Play("close_button");

            ShowScore(uiComponent, true);
        }

        private void ShowScore(UiComponent uiComponent, bool show)
        {
            uiComponent.targetPanel.SetActive(!show);
            uiComponent.score.SetActive(show);
            if(show)
            {
                ChooseScoreText(uiComponent);
            }
            
        }

        private void ChooseScoreText(UiComponent uiComponent)
        {
            uiComponent.targetNumberInScore.text = uiComponent.targetNumber.text;
            var value = targetCount - currentCount;
            var tolerance = targetCount * 0.15;

            var text = "Keep on keeping on.";

            if(value == 0)
            {
                text = "Awesome! You're the best busdriver in town!";
                MessageBroker.Default.Publish(new ShowAwesomenessEvent());

            } else if(value > tolerance)
            {
                text = "Man, you left angry kids waiting - although you had empty seats!";
            } else if(value < (tolerance*-1))
            {
                text = "This bus was full. Full with puke!";
            }

            uiComponent.scoreText.text = text;
        }
    }
}