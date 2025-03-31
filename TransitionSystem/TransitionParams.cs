using System.Windows.Threading;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionParams : ICloneable
    {
        public TransitionParams() { }

        public TransitionParams(Action<TransitionParams>? action)
        {
            action?.Invoke(this);
        }

        public static int DefaultFrameRate { get; set; } = 60;
        public static DispatcherPriority DefaultPriority { get; set; } = DispatcherPriority.Normal;
        public static TransitionParams Theme { get; set; } = new()
        {
            FrameRate = DefaultFrameRate,
            Duration = 0.5
        };
        public static TransitionParams Hover { get; set; } = new()
        {
            FrameRate = DefaultFrameRate,
            Duration = 0.3
        };
        public static TransitionParams Empty { get; private set; } = new()
        {
            Duration = 0
        };

#if NET
        internal double DeltaTime { get => 1000.0 / Math.Clamp(FrameRate, 1, TransitionScheduler.MaxFrameRate); }
        internal double FrameCount { get => Math.Clamp(Duration * Math.Clamp(FrameRate, 1, TransitionScheduler.MaxFrameRate), 1, int.MaxValue); }
#elif NETFRAMEWORK
        internal double DeltaTime { get => 1000.0 / FrameRate.Clamp(1, TransitionScheduler.MaxFrameRate); }
        internal double FrameCount { get => (Duration * FrameRate.Clamp(1, TransitionScheduler.MaxFrameRate)).Clamp(1, int.MaxValue); }
#endif

        public event Action? Start;
        public event Action? Update;
        public event Action? LateUpdate;
        public event Action? Completed;

        public bool IsAutoReverse { get; set; } = false;
        public int LoopTime { get; set; } = 0;
        public double Duration { get; set; } = 0;
        public int FrameRate { get; set; } = DefaultFrameRate;
        public double Acceleration { get; set; } = 0;
        public DispatcherPriority Priority { get; set; } = DefaultPriority;
        public bool IsAsync { get; set; } = false;

        internal TransitionParams DeepCopy()
        {
            var copy = new TransitionParams
            {
                Start = Start,
                Update = Update,
                LateUpdate = LateUpdate,
                Completed = Completed,
                IsAutoReverse = IsAutoReverse,
                LoopTime = LoopTime,
                Duration = Duration,
                FrameRate = FrameRate,
                Acceleration = Acceleration,
                Priority = Priority,
                IsAsync = IsAsync
            };
            return copy;
        }

        internal void StartInvoke()
        {
            Start?.Invoke();
        }
        internal void UpdateInvoke()
        {
            Update?.Invoke();
        }
        internal void LateUpdateInvoke()
        {
            LateUpdate?.Invoke();
        }
        internal void CompletedInvoke()
        {
            Completed?.Invoke();
        }

        public object Clone()
        {
            return DeepCopy();
        }
    }
}
