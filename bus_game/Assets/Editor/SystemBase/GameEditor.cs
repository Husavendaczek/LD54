using SystemBase.EditorCore;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Editor.SystemBase
{
    [InitializeOnLoad]
    public class GameEditor
    {
        public static readonly ReactiveProperty<MouseData> MouseData = new ReactiveProperty<MouseData>();
        public static readonly EditorSystemCollection EditorSystems = new EditorSystemCollection();

        static GameEditor(){
            Debug.Log("Game Editor started");

            EditorSystems.Initialize();
            
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView view)
        {
            UpdateMouseData(view);
        }

        private static void UpdateMouseData(SceneView sceneView)
        {
            
            if (!Event.current.isMouse) return;
            
            var cur = Event.current;
            var mousePos = (Vector3) cur.mousePosition;
            mousePos.y = Camera.current.pixelHeight - mousePos.y;
            var worldPos = sceneView.camera.ScreenToWorldPoint(mousePos);
            
            MouseData.SetValueAndForceNotify(new MouseData
            {
                editorPosition = mousePos,
                mouseWorldPosition = worldPos,
                mouseButtonClicked = cur.clickCount > 0 ? cur.button : -1,
            });
        }
    }
}