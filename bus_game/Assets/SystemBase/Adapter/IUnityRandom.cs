using UnityEngine;

namespace SystemBase.Adapter
{
    public interface IUnityRandom
    {
        float Value { get; }
        int Range(int start, int end);
    }

    public class UnityRandomAdapter : IUnityRandom
    {
        public float Value => Random.value;

        public int Range(int start, int end)
        {
            return Random.Range(start, end);
        }
    }
}