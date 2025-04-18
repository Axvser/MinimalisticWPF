using System.Windows.Threading;
using System.Windows;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionParams : DependencyObject, ICloneable
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

        /// <summary>
        /// 过渡执行完毕后,是否加载反转过渡以回复到起始状态
        /// </summary>
        public bool IsAutoReverse
        {
            get { return (bool)GetValue(IsAutoReverseProperty); }
            set { SetValue(IsAutoReverseProperty, value); }
        }
        public static readonly DependencyProperty IsAutoReverseProperty =
            DependencyProperty.Register("IsAutoReverse", typeof(bool), typeof(TransitionParams), new PropertyMetadata(false));

        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopTime
        {
            get { return (int)GetValue(LoopTimeProperty); }
            set { SetValue(LoopTimeProperty, value); }
        }
        public static readonly DependencyProperty LoopTimeProperty =
            DependencyProperty.Register("LoopTime", typeof(int), typeof(TransitionParams), new PropertyMetadata(0));

        /// <summary>
        /// 过渡持续时间 S
        /// </summary>
        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(TransitionParams), new PropertyMetadata(0d));

        /// <summary>
        /// 每秒加载的帧数
        /// </summary>
        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }
        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(double), typeof(TransitionParams), new PropertyMetadata(DefaultFrameRate));

        /// <summary>
        /// 加速度a,使时间变化率在xy图呈现斜率为a的直线
        /// </summary>
        public double Acceleration
        {
            get { return (double)GetValue(AccelerationProperty); }
            set { SetValue(AccelerationProperty, value); }
        }
        public static readonly DependencyProperty AccelerationProperty =
            DependencyProperty.Register("Acceleration", typeof(double), typeof(TransitionParams), new PropertyMetadata(0d));

        /// <summary>
        /// 属性更新操作的优先级
        /// </summary>
        public DispatcherPriority Priority
        {
            get { return (DispatcherPriority)GetValue(PriorityProperty); }
            set { SetValue(PriorityProperty, value); }
        }
        public static readonly DependencyProperty PriorityProperty =
            DependencyProperty.Register("Priority", typeof(DispatcherPriority), typeof(TransitionParams), new PropertyMetadata(DefaultFrameRate));

        /// <summary>
        /// 是否使用InvokeAsync调度属性更新操作
        /// <para>通常,这由库自动判断,但是如果您希望以指定的Priority在InvokeAsync执行更新操作,那么此项需要改为true</para>
        /// </summary>
        public bool IsAsync
        {
            get { return (bool)GetValue(IsAsyncProperty); }
            set { SetValue(IsAsyncProperty, value); }
        }
        public static readonly DependencyProperty IsAsyncProperty =
            DependencyProperty.Register("IsAsync", typeof(bool), typeof(TransitionParams), new PropertyMetadata(false));

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
}
