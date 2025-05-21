using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MinimalisticWPF.FrameworkSupport;

namespace MinimalisticWPF.TransitionSystem
{
    public delegate void FrameEventHandler(object sender, FrameEventArgs e);

    public class FrameEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;
        public double Progress { get; internal set; } = 0;
    }

    /// <summary>
    /// ✨ Describe the transition effect
    /// <para>Static instance</para>
    /// <para>- <see cref="TransitionParams.Hover"/></para>
    /// <para>- <see cref="TransitionParams.Theme"/></para>
    /// <para>- <see cref="TransitionParams.Empty"/></para>
    /// <para>Effect parameter</para>
    /// <para>- <see cref="TransitionParams.FrameRate"/></para>
    /// <para>- <see cref="TransitionParams.Duration"/></para>
    /// <para>- <see cref="TransitionParams.Acceleration"/></para>
    /// <para>- <see cref="TransitionParams.IsAutoReverse"/></para>
    /// <para>- <see cref="TransitionParams.LoopTime"/></para>
    /// <para>- <see cref="TransitionParams.Priority"/></para>
    /// <para>- <see cref="TransitionParams.IsAsync"/></para>
    /// <para>Life cycle</para>
    /// <para>- <see cref="TransitionParams.Awaked"/></para>
    /// <para>- <see cref="TransitionParams.Start"/></para>
    /// <para>- <see cref="TransitionParams.Update"/></para>
    /// <para>- <see cref="TransitionParams.LateUpdate"/></para>
    /// <para>- <see cref="TransitionParams.Canceled"/></para>
    /// <para>- <see cref="TransitionParams.Completed"/></para>
    /// </summary>
    public sealed class TransitionParams : ICloneable
    {
        public TransitionParams() { }
        public TransitionParams(Action<TransitionParams>? action)
        {
            action?.Invoke(this);
        }

        public const int MAX_FPS = 165;
        public const int MIN_FPS = 1;

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
        public static TransitionParams Empty { get; } = new()
        {
            Duration = 0
        };

        public double DeltaTime => 1000.0 / XMath.Clamp(FrameRate, MIN_FPS, MAX_FPS);
        public double FrameCount => XMath.Clamp(Duration * XMath.Clamp(FrameRate, MIN_FPS, MAX_FPS), 1, int.MaxValue);

        public event FrameEventHandler? Awaked;
        public event FrameEventHandler? Start;
        public event FrameEventHandler? Update;
        public event FrameEventHandler? LateUpdate;
        public event FrameEventHandler? Completed;
        public event FrameEventHandler? Canceled;

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
                Awaked = Awaked?.Clone() as FrameEventHandler,
                Start = Start?.Clone() as FrameEventHandler,
                Update = Update?.Clone() as FrameEventHandler,
                LateUpdate = LateUpdate?.Clone() as FrameEventHandler,
                Completed = Completed?.Clone() as FrameEventHandler,
                Canceled = Canceled?.Clone() as FrameEventHandler,
                IsAutoReverse = IsAutoReverse,
                LoopTime = LoopTime,
                Duration = Duration,
                FrameRate = FrameRate,
                Acceleration = Acceleration,
                Priority = Priority,
                IsAsync = IsAsync
            };
            // 不复制事件处理程序
            return copy;
        }

        internal void AwakeInvoke(object sender, FrameEventArgs e)
        {
            Awaked?.Invoke(sender, e);
        }
        internal void StartInvoke(object sender, FrameEventArgs e)
        {
            Start?.Invoke(sender, e);
        }
        internal void UpdateInvoke(object sender, FrameEventArgs e)
        {
            Update?.Invoke(sender, e);
        }
        internal void LateUpdateInvoke(object sender, FrameEventArgs e)
        {
            LateUpdate?.Invoke(sender, e);
        }
        internal void CompletedInvoke(object sender, FrameEventArgs e)
        {
            Completed?.Invoke(sender, e);
        }
        internal void CancledInvoke(object sender, FrameEventArgs e)
        {
            Canceled?.Invoke(sender, e);
        }

        public void ClearAllEventHandlers()
        {
            Awaked = null;
            Start = null;
            Update = null;
            LateUpdate = null;
            Completed = null;
            Canceled = null;
        }

        public object Clone()
        {
            return DeepCopy();
        }
    }
}
