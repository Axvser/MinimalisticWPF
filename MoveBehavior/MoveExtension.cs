using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    public enum RenderTimes
    {
        DesignTime,
        RunTime,
        AnyTime,
        None
    }

    public static class MoveBehaviorExtension
    {
        public static PropertyInfo RenderTransformPropertyInfo { get; internal set; } = typeof(UIElement).GetProperty("RenderTransform") ?? throw new ArgumentNullException("Can not get RenderTransform PropertyInfo");
        public static ConcurrentDictionary<object, TransitionScheduler> Schedulers { get; internal set; } = new();
        public static void BeginMove<T>(this FrameworkElement target, T tractionMeta, TransitionParams transitionParams) where T : IExecutableMove, IMoveMeta
        {
            tractionMeta.TransitionParams = transitionParams;
            target.BeginMove(tractionMeta);
        }
        public static void BeginMove(this FrameworkElement target, IExecutableMove tractionMeta)
        {
            tractionMeta.Start(target);
        }
        public static void EndMove(this FrameworkElement target)
        {
            if (Schedulers.TryGetValue(target, out var scheduler))
            {
                scheduler.Interpreter?.Stop();
            }
        }
    }
}
