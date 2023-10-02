using SystemBase.Core.Components;
using TMPro;
using UnityEngine;

namespace Systems.UI
{
    public class UiComponent : GameComponent
    {
        public TextMeshProUGUI targetNumber;
        public TextMeshProUGUI currentNumber;
        public GameObject targetPanel;
        public GameObject score;
        public TextMeshProUGUI targetNumberInScore;
    }
}