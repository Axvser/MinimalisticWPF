using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF
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
        public static IExecutableTransition Create<T>() where T : class
        {
            return CreateBoardFromType<T>();
        }
        public static IExecutableTransition Create<T>(T target) where T : class
        {
            var result = CreateBoardFromType<T>();
            result.Target = target;
            return result;
        }
        public static IExecutableTransition Create<T>(ICollection<T> values) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta();
            meta.Target = (values.FirstOrDefault() as ITargetedTransition)?.Target;
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(values.FirstOrDefault()?.TransitionParams ?? new TransitionParams());
            }
            meta.Merge(values);
            return meta;
        }
        public static IExecutableTransition Create<T>(ICollection<T> values, TransitionParams transitionParams) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta();
            meta.Target = (values.FirstOrDefault() as ITargetedTransition)?.Target;
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(transitionParams);
            }
            meta.TransitionParams = transitionParams;
            meta.Merge(values);
            return meta;
        }
        public static IExecutableTransition Create<T>(ICollection<T> values, Action<TransitionParams> transitionSet) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            var meta = new TransitionMeta();
            var para = new TransitionParams();
            transitionSet.Invoke(para);
            meta.Target = (values.FirstOrDefault() as ITargetedTransition)?.Target;
            foreach (var value in values)
            {
                (value as IFramePreloading)?.PreLoad(para);
            }
            meta.TransitionParams = para;
            meta.Merge(values);
            return meta;
        }
        public static void Dispose()
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
        public static void Stop(params object[] targets)
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
        public static void StopSafe(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = StateMachine.Create(target);
                machine.Interpreter?.Stop();
            }
        }
        public static void StopUnSafe(params object[] targets)
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
        internal static TransitionBoard<T> CreateBoardFromType<T>() where T : class
        {
            return new TransitionBoard<T>();
        }
        internal static TransitionBoard<T> CreateBoardFromObject<T>(T target) where T : class
        {
            return new TransitionBoard<T>() { Target = target };
        }
    }
}
