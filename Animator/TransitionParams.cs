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
        public event Func<Task>? StartAsync;
        public event Func<Task>? UpdateAsync;
        public event Func<Task>? LateUpdateAsync;
        public event Func<Task>? CompletedAsync;
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

        internal async Task StartInvoke()
        {
            Start?.Invoke();
            if (StartAsync != null)
            {
                await StartAsync.Invoke();
            }
        }
        internal async Task UpdateInvoke()
        {
            Update?.Invoke();
            if (UpdateAsync != null)
            {
                await UpdateAsync.Invoke();
            }
        }
        internal async Task LateUpdateInvoke()
        {
            LateUpdate?.Invoke();
            if (LateUpdateAsync != null)
            {
                await LateUpdateAsync.Invoke();
            }
        }
        internal async Task CompletedInvoke()
        {
            Completed?.Invoke();
            if (CompletedAsync != null)
            {
                await CompletedAsync.Invoke();
            }
        }
    }
}
