using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    internal class MoveAggregator(FrameworkElement target, TransitionParams transitionParams, ICollection<IMoveMeta> moves) : IExecutableMove
    {
        public TransitionScheduler Scheduler { get; private set; } = InitializeScheduler(target);
        public List<List<Tuple<PropertyInfo, List<object?>>>> PreloadData { get; internal set; } = Preload(target, transitionParams, moves);
        public TransitionParams TransitionParams { get; internal set; } = transitionParams;

        public void Start(FrameworkElement target)
        {
            var state = new State() { StateName = "movestateAggregator" };
            state.AddProperty(MoveBehaviorExtension.RenderTransformPropertyInfo.Name, null);
            Scheduler.States.Add(state);
            Scheduler.TransitionParams = TransitionParams;
            Scheduler.InterpreterScheduler("movestateAggregator", TransitionParams, PreloadData);
        }

        public void Stop(FrameworkElement target)
        {
            Scheduler.Interpreter?.Stop();
        }

        public static List<List<Tuple<PropertyInfo, List<object?>>>> Preload(FrameworkElement target, TransitionParams transitionParams, ICollection<IMoveMeta> moves)
        {
            List<object?> frames = new(512);
            double duration = 0;
            var offest = new Point(target.ActualWidth / 2, target.ActualHeight / 2);
            foreach (var move in moves)
            {
#if NET
                double FrameCount = Math.Clamp(move.TransitionParams.Duration * Math.Clamp(move.TransitionParams.FrameRate, 1, TransitionScheduler.MaxFrameRate), 1, int.MaxValue);
#endif

#if NETFRAMEWORK
                double FrameCount = (move.TransitionParams.Duration * move.TransitionParams.FrameRate.Clamp(1, TransitionScheduler.MaxFrameRate)).Clamp(1, int.MaxValue);
#endif
                duration += move.TransitionParams.Duration;
                foreach (var frame in move.GetNormalFrames(offest, (int)FrameCount)[0][0].Item2)
                {
                    frames.Add(frame);
                }
            }
            transitionParams.Duration = duration;
            return [[Tuple.Create(MoveBehaviorExtension.RenderTransformPropertyInfo, frames)]];
        }

        public static TransitionScheduler InitializeScheduler(FrameworkElement target)
        {
            var search = MoveBehaviorExtension.Schedulers.TryGetValue(target, out var value);
            var scheduler = search ? value : TransitionScheduler.CreateIndependentUnit(target);
            if (scheduler is null) throw new ArgumentException("Failed to create TransitionScheduler");
            if (!search)
            {
                MoveBehaviorExtension.Schedulers.TryAdd(target, scheduler);
            }
            var state = new State() { StateName = "movestateAggregator" };
            state.AddProperty(MoveBehaviorExtension.RenderTransformPropertyInfo.Name, null);
            scheduler.States.Add(state);
            return scheduler;
        }
    }
}
