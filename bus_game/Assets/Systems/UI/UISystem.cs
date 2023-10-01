using Systems.Bus.Events;
using Systems.Score;
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
    }
}
