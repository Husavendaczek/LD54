using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Systems.UI
{
    internal class UISystem : MonoBehaviour
    {

        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
