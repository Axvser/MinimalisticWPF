using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF.Animator
{
    public static class Transition
    {
        private static string _tempName = "temp";
        public static string TempName
        {
            get => _tempName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _tempName = value;
                }
            }
        }

        public static IExecutableTransition Create<T>(T? target = null) where T : class
        {
            return new TransitionBoard<T>() { Target = target };
        }
        public static IExecutableTransition Create<T>(ICollection<T> values, object? target = null) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta
            {
                Target = target ?? (values.FirstOrDefault() as ITransitionWithTarget)?.Target
            };
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(values.FirstOrDefault()?.TransitionParams ?? new TransitionParams());
            }
            meta.Merge(values);
            return meta;
        }
        public static IExecutableTransition Create<T>(ICollection<T> values, TransitionParams transitionParams, object? target = null) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta
            {
                Target = target ?? (values.FirstOrDefault() as ITransitionWithTarget)?.Target
            };
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(transitionParams);
            }
            meta.TransitionParams = transitionParams;
            meta.Merge(values);
            return meta;
        }
        public static IExecutableTransition Create<T>(ICollection<T> values, Action<TransitionParams> transitionSet, object? target = null) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta();
            var para = new TransitionParams();
            transitionSet.Invoke(para);
            meta.Target = target ?? (values.FirstOrDefault() as ITransitionWithTarget)?.Target;
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(para);
            }
            meta.TransitionParams = para;
            meta.Merge(values);
            return meta;
        }

        public static void DisposeAll()
        {
            foreach (var machinedic in StateMachine.MachinePool.Values)
            {
                foreach (var machine in machinedic.Values)
                {
                    machine.Interpreter?.Stop();
                    foreach (var intor in machine.UnSafeInterpreters)
                    {
                        intor.Stop(true);
                    }
                }
            }
        }
        public static void Dispose(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Stop();
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Stop(true);
                }
            }
        }
        public static void DisposeSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Stop();
            }
        }
        public static void DisposeUnSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                foreach (var itor in machine.UnSafeInterpreters)
                {
                    itor.Stop(true);
                }
            }
        }
    }
}
