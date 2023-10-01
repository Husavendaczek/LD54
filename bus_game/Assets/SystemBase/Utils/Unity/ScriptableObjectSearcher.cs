using UnityEngine;

namespace SystemBase.Utils.Unity
{
    public static class ScriptableObjectSearcher
    {
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            return Resources.LoadAll<T>("");
        }
    }
}