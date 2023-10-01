using Systems.Bus.Events;
using Systems.Score;
using Systems.UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Systems.UI
{
    internal class UISystem : MonoBehaviour
    {
        public void StartGame()
        {
            ScoreBoard.Reset();
            SceneManager.LoadScene(1);
        }

        public void CloseDoors()
        {
            MessageBroker.Default.Publish(new BusDoorCloseEvent());
        }

        public void HoverCloseDoors()
        {
            var animation = gameObject.GetComponentInChildren<Animator>();
            animation.Play("close_button");
        }

        public void ExitHoverOpenDoors()
        {
            var animation = gameObject.GetComponentInChildren<Animator>();
            animation.Play("open_button");
        }
    }
}
