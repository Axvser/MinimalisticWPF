using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem;

namespace MinimalisticWPF.Extension
{
    public static class TransitionExtension
    {
        public static TransitionBoard<T> Transition<T>(this T element) where T : class
        {
            TransitionBoard<T> tempStoryBoard = new() { TransitionApplied = element };
            return tempStoryBoard;
        }
        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 transfer) where T1 : class where T2 : class, ITransitionMeta
        {
            var result = TransitionSystem.Transition.Compile([transfer], transfer.TransitionParams, source);
            result.Start();
            return result;
        }
        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 state, Action<TransitionParams> set) where T1 : class where T2 : class, ITransitionMeta
        {
            TransitionSystem.Transition.DisposeSafe(source);
            var param = new TransitionParams();
            set.Invoke(param);
            var result = TransitionSystem.Transition.Compile([state], param, source);
            result.Start();
            return result;

        }
        public static IExecutableTransition BeginTransition<T1, T2>(this T1 source, T2 state, TransitionParams param) where T1 : class where T2 : class, ITransitionMeta
        {
            TransitionSystem.Transition.DisposeSafe(source);
            var result = TransitionSystem.Transition.Compile([state], param, source);
            result.Start();
            return result;
        }
        public static TransitionScheduler? FindTransitionScheduler<T>(this T source) where T : class
        {
            if (TransitionScheduler.TryGetScheduler(source, out var Scheduler))
            {
                return Scheduler;
            }
            return null;
        }
    }
}
