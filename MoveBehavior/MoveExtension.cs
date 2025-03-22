using MinimalisticWPF.StructuralDesign;
using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static void BeginMove(this FrameworkElement target, IMoveMeta move)
        {
            var offest = new Point(target.ActualWidth / 2, target.ActualHeight / 2);
            var search = MoveBehaviorExtension.Schedulers.TryGetValue(target, out var value);
            var scheduler = search ? value : TransitionScheduler.CreateIndependentUnit(target);
            if (scheduler is null) throw new ArgumentException("Failed to create TransitionScheduler");
            if (!search)
            {
                MoveBehaviorExtension.Schedulers.TryAdd(target, scheduler);
            }
            var state = new State() { StateName = "movestate" };
            state.AddProperty(MoveBehaviorExtension.RenderTransformPropertyInfo.Name, null);
            scheduler.States.Add(state);
            scheduler.TransitionParams = move.TransitionParams;
            scheduler.InterpreterScheduler(state.StateName, move.TransitionParams, move.GetNormalFrames(offest, (int)scheduler.FrameCount));
        }
        public static void BeginMove(this FrameworkElement target, IMoveMeta move, TransitionParams transitionParams)
        {
            var offest = new Point(target.ActualWidth / 2, target.ActualHeight / 2);
            var search = MoveBehaviorExtension.Schedulers.TryGetValue(target, out var value);
            var scheduler = search ? value : TransitionScheduler.CreateIndependentUnit(target);
            if (scheduler is null) throw new ArgumentException("Failed to create TransitionScheduler");
            if (!search)
            {
                MoveBehaviorExtension.Schedulers.TryAdd(target, scheduler);
            }
            move.TransitionParams = transitionParams;
            var state = new State() { StateName = "movestate" };
            state.AddProperty(MoveBehaviorExtension.RenderTransformPropertyInfo.Name, null);
            scheduler.States.Add(state);
            scheduler.TransitionParams = transitionParams;
            scheduler.InterpreterScheduler(state.StateName, transitionParams, move.GetNormalFrames(offest, (int)scheduler.FrameCount));
        }
        public static void BeginMove(this FrameworkElement target, IExecutableMove move)
        {
            move.Start(target);
        }
        public static void EndMove(this FrameworkElement target)
        {
            if (Schedulers.TryGetValue(target, out var scheduler))
            {
                scheduler.Interpreter?.Stop();
            }
        }

        public static Visibility AnalizeVisibility<T>(this T element) where T : FrameworkElement, IOptionalRendeTime
        {
            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(element);
#if NETFRAMEWORK
            if ((element.RenderTime == RenderTimes.RunTime && !isInDesignMode) ||
                (element.RenderTime == RenderTimes.DesignTime && isInDesignMode) ||
                (element.RenderTime == RenderTimes.AnyTime))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
#elif NET
            return (element.RenderTime, isInDesignMode) switch
            {
                (RenderTimes.RunTime, false) => Visibility.Visible,
                (RenderTimes.DesignTime, true) => Visibility.Visible,
                (RenderTimes.AnyTime, _) => Visibility.Visible,
                _ => Visibility.Collapsed
            };
#endif
        }
        public static Visibility AnalizeVisibility<T>(this T element,RenderTimes rendertime) where T : FrameworkElement
        {
            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(element);
#if NETFRAMEWORK
            if ((rendertime == RenderTimes.RunTime && !isInDesignMode) ||
                (rendertime == RenderTimes.DesignTime && isInDesignMode) ||
                (rendertime == RenderTimes.AnyTime))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
#elif NET
            return (rendertime, isInDesignMode) switch
            {
                (RenderTimes.RunTime, false) => Visibility.Visible,
                (RenderTimes.DesignTime, true) => Visibility.Visible,
                (RenderTimes.AnyTime, _) => Visibility.Visible,
                _ => Visibility.Collapsed
            };
#endif
        }
    }
}
