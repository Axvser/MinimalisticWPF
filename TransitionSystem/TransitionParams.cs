using MinimalisticWPF.TransitionSystem;
using System.Windows.Threading;
#if NETFRAMEWORK
using MinimalisticWPF.FrameworkSupport;
#endif

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
/// <para>- <see cref="TransitionParams.Start"/></para>
/// <para>- <see cref="TransitionParams.Update"/></para>
/// <para>- <see cref="TransitionParams.LateUpdate"/></para>
/// <para>- <see cref="TransitionParams.Completed"/></para>
/// </summary>
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
            Start = Start?.Clone() as Action,
            Update = Update?.Clone() as Action,
            LateUpdate = LateUpdate?.Clone() as Action,
            Completed = Completed?.Clone() as Action,
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
