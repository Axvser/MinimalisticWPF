using System.Windows.Threading;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionParams
    {
        public TransitionParams() { }

        public TransitionParams(Action<TransitionParams>? action)
        {
            action?.Invoke(this);
        }

        public static int DefaultFrameRate { get; set; } = 60;
        public static DispatcherPriority DefaultUIPriority { get; set; } = DispatcherPriority.Normal;
        public static bool DefaultIsBeginInvoke { get; set; } = false;
        public static Action<TransitionParams> Theme { get; set; } = (x) =>
        {
            x.FrameRate = DefaultFrameRate;
            x.Duration = 0.5;
        };
        public static Action<TransitionParams> Hover { get; set; } = (x) =>
        {
            x.FrameRate = DefaultFrameRate;
            x.Duration = 0.2;
        };

        public Action? Start { get; set; }
        public Action? Update { get; set; }
        public Action? LateUpdate { get; set; }
        public Action? Completed { get; set; }
        public Func<Task>? StartAsync { get; set; }
        public Func<Task>? UpdateAsync { get; set; }
        public Func<Task>? LateUpdateAsync { get; set; }
        public Func<Task>? CompletedAsync { get; set; }
        public bool IsAutoReverse { get; set; } = false;
        public int LoopTime { get; set; } = 0;
        public double Duration { get; set; } = 0;
        public int FrameRate { get; set; } = DefaultFrameRate;
        public bool IsQueue { get; set; } = false;
        public bool IsLast { get; set; } = false;
        public bool IsUnique { get; set; } = true;
        public double Acceleration { get; set; } = 0;
        public bool IsUnSafe { get; set; } = false;
        public DispatcherPriority UIPriority { get; set; } = DefaultUIPriority;
        public bool IsBeginInvoke { get; set; } = DefaultIsBeginInvoke;

        public TransitionParams DeepCopy()
        {
            var copy = new TransitionParams
            {
                Start = this.Start,
                Update = this.Update,
                LateUpdate = this.LateUpdate,
                Completed = this.Completed,
                StartAsync = this.StartAsync,
                UpdateAsync = this.UpdateAsync,
                LateUpdateAsync = this.LateUpdateAsync,
                CompletedAsync = this.CompletedAsync,
                IsAutoReverse = this.IsAutoReverse,
                LoopTime = this.LoopTime,
                Duration = this.Duration,
                FrameRate = this.FrameRate,
                IsQueue = this.IsQueue,
                IsLast = this.IsLast,
                IsUnique = this.IsUnique,
                Acceleration = this.Acceleration,
                IsUnSafe = this.IsUnSafe,
                UIPriority = this.UIPriority,
                IsBeginInvoke = this.IsBeginInvoke
            };
            return copy;
        }
    }
}
