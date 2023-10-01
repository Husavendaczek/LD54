using Systems.Score;
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
    }
}
