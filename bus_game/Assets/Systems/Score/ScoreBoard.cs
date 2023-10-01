using UniRx;

namespace Systems.Score
{
    public static class ScoreBoard
    {
        public const int MaximumPassengers = 63;
        public const int MaximumSpawnedPassengers = 500;

        public static IntReactiveProperty TargetPassengers { get; set; } = new(0);
        public static IntReactiveProperty Passengers { get; set; } = new(0);
        public static IntReactiveProperty TotalPassengers { get; set; } = new(0);

        public static void Reset()
        {
            TargetPassengers.Value = 0;
            Passengers.Value = 0;
            TotalPassengers.Value = 0;
        }

        public static void ResetCurrent()
        {
            Passengers.Value = 0;
        }
        
        public static void AddPassenger()
        {
            Passengers.Value++;
            TotalPassengers.Value++;
        }

        public static void SetTargetPassengers(int target)
        {
            TargetPassengers.Value = target;
        }
    }
}