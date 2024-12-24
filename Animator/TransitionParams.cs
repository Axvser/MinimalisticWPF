using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MinimalisticWPF
{
    public class TransitionParams
    {
        public TransitionParams() { }

        public TransitionParams(Action<TransitionParams>? action)
        {
            action?.Invoke(this);
        }

        /// <summary>
        /// 默认帧率
        /// </summary>
        public static int DefaultFrameRate { get; set; } = 60;
        /// <summary>
        /// 默认优先级
        /// </summary>
        public static DispatcherPriority DefaultUIPriority { get; set; } = DispatcherPriority.Normal;
        /// <summary>
        /// UI刷新是否采用BeginInvoke
        /// </summary>
        public static bool DefaultIsBeginInvoke { get; set; } = false;
        /// <summary>
        /// 主题切换参数
        /// </summary>
        public static Action<TransitionParams> Theme { get; set; } = (x) =>
        {
            x.FrameRate = DefaultFrameRate;
            x.Duration = 0.5;
        };
        /// <summary>
        /// 悬停特效参数
        /// </summary>
        public static Action<TransitionParams> Hover { get; set; } = (x) =>
        {
            x.FrameRate = DefaultFrameRate;
            x.Duration = 0.2;
        };

        /// <summary>
        /// 过渡启动前执行
        /// </summary>
        public Action? Start { get; set; }
        /// <summary>
        /// 每一帧开始时执行
        /// </summary>
        public Action? Update { get; set; }
        /// <summary>
        /// 每一帧结束时执行
        /// </summary>
        public Action? LateUpdate { get; set; }
        /// <summary>
        /// 动画结束后执行
        /// </summary>
        public Action? Completed { get; set; }
        /// <summary>
        /// 过渡启动前执行(可等待)
        /// </summary>
        public Func<Task>? StartAsync { get; set; }
        /// <summary>
        /// 每一帧开始时执行(可等待)
        /// </summary>
        public Func<Task>? UpdateAsync { get; set; }
        /// <summary>
        /// 每一帧结束时执行(可等待)
        /// </summary>
        public Func<Task>? LateUpdateAsync { get; set; }
        /// <summary>
        /// 动画结束后执行(可等待)
        /// </summary>
        public Func<Task>? CompletedAsync { get; set; }
        /// <summary>
        /// 是否自动回复
        /// </summary>
        public bool IsAutoReverse { get; set; } = false;
        /// <summary>
        /// 循环次数
        /// </summary>
        public int LoopTime { get; set; } = 0;
        /// <summary>
        /// 持续时长(单位: s )
        /// </summary>
        public double Duration { get; set; } = 0;
        /// <summary>
        /// 过渡帧率(默认: 60 )
        /// </summary>
        public int FrameRate { get; set; } = DefaultFrameRate;
        /// <summary>
        /// 是否排队执行(默认:不排队)
        /// </summary>
        public bool IsQueue { get; set; } = false;
        /// <summary>
        /// 是否在执行完后,清除其它排队中的过渡效果(默认: 不清除 )
        /// </summary>
        public bool IsLast { get; set; } = false;
        /// <summary>
        /// 申请进入执行队列时,若已有同名切换操作处于列队中,是否继续加入队列(默认:不加入)
        /// </summary>
        public bool IsUnique { get; set; } = true;
        /// <summary>
        /// 加速度(默认:0)
        /// </summary>
        public double Acceleration { get; set; } = 0;
        /// <summary>
        /// ⚠ 不安全操作，启用它，代表这次过渡是无条件、无相关、立刻启动的(默认: false)
        /// <para>1.不打断当前过渡</para>
        /// <para>2.立即创建一个Task开始执行此次过渡</para>
        /// <para></para>
        /// </summary>
        public bool IsUnSafe { get; set; } = false;
        /// <summary>
        /// UI更新优先级
        /// </summary>
        public DispatcherPriority UIPriority { get; set; } = DefaultUIPriority;
        /// <summary>
        /// 刷新属性时是否采用BeginInvoke
        /// </summary>
        public bool IsBeginInvoke { get; set; } = DefaultIsBeginInvoke;
    }
}
