using System;

namespace SystemBase.Adapter
{
    public interface ISystemRandom
    {
        void Create(int seed);
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
    }

    public class SystemRandomAdapter : ISystemRandom
    {
        private Random _original;
        
        public void Create(int seed)
        {
            _original = new Random(seed);
        }

        public int Next(int minValue, int maxValue)
        {
            return _original.Next(minValue, maxValue);
        }
        
        public int Next(int maxValue)
        {
            return _original.Next(maxValue);
        }
    }
}