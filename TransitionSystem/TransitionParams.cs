using System.Windows.Threading;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionParams
    {
        public TransitionParams() { }

        public TransitionParams(Action<TransitionParams>? action)
        {
            action?.Invoke(this);
        }

        public static int DefaultFrameRate { get; set; } = 60;
        public static DispatcherPriority DefaultPriority { get; set; } = DispatcherPriority.Normal;
        public static bool DefaultIsBeginInvoke { get; set; } = false;
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

        public event Action? Start;
        public event Action? Update;
        public event Action? LateUpdate;
        public event Action? Completed;
        public bool IsAutoReverse { get; set; } = false;
        public int LoopTime { get; set; } = 0;
        public double Duration { get; set; } = 0;
        public int FrameRate { get; set; } = DefaultFrameRate;
        public bool IsLast { get; set; } = false;
        public double Acceleration { get; set; } = 0;
        public bool IsUnSafe { get; set; } = false;
        public DispatcherPriority Priority { get; set; } = DefaultPriority;
        public bool IsBeginInvoke { get; set; } = DefaultIsBeginInvoke;

        public TransitionParams DeepCopy()
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
                IsLast = IsLast,
                Acceleration = Acceleration,
                IsUnSafe = IsUnSafe,
                Priority = Priority,
                IsBeginInvoke = IsBeginInvoke
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
    }
}
