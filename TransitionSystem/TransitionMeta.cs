using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem.Basic;
using MinimalisticWPF.Extension;
using System.Reflection;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionMeta : IMergeableTransition, ITransitionMeta, IConvertibleTransitionMeta, IExecutableTransition, ITransitionWithTarget, ICompilableTransition
    {
        internal TransitionMeta() { }
        public TransitionMeta(TransitionParams transitionParams, State propertyState)
        {
            TransitionParams = transitionParams;
            PropertyState = propertyState;
        }
        public TransitionMeta(TransitionParams transitionParams, List<List<Tuple<PropertyInfo, List<object?>>>> tuples)
        {
            TransitionParams = transitionParams;
            foreach (var tuple in tuples)
            {
                foreach (var value in tuple)
                {
                    PropertyState.AddProperty(value.Item1.Name, value.Item2.LastOrDefault());
                }
            }
        }
        public TransitionMeta(ITransitionMeta transitionMeta)
        {
            TransitionParams = transitionMeta.TransitionParams;
            PropertyState = transitionMeta.PropertyState;
        }
        public TransitionMeta(params TransitionMeta[] transitionMetas)
        {
            Merge(transitionMetas);
        }

        public object? TransitionApplied { get; set; }
        public TransitionParams TransitionParams { get; set; } = new();
        public State PropertyState { get; set; } = new State() { StateName = Transition.TempName };
        public TransitionScheduler TransitionScheduler => TransitionApplied == null ? throw new ArgumentNullException(nameof(TransitionApplied), "The metadata is missing the target instance for this transition effect") : TransitionScheduler.CreateOrFind(TransitionApplied);
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence => TransitionScheduler.PreloadFrames(TransitionApplied, PropertyState, TransitionParams) ?? [];
        public ITransitionMeta Merge(ICollection<ITransitionMeta> metas)
        {
            var result = IMergeableTransition.MergeMetas(metas);
            PropertyState = result.PropertyState;
            return result;
        }
        public TransitionMeta ToTransitionMeta()
        {
            return this;
        }
        public State ToState()
        {
            return PropertyState;
        }
        public TransitionBoard<T> ToTransitionBoard<T>() where T : class
        {
            var result = new TransitionBoard<T>
            {
                PropertyState = ToState(),
                TransitionApplied = TransitionApplied,
                TransitionParams = TransitionParams,
            };
            return result;
        }
        public Task Start(object? target = null)
        {
            TransitionApplied = target ?? TransitionApplied;
            if (TransitionApplied == null) throw new ArgumentNullException(nameof(target), "The metadata is missing the target instance for this transition effect");
            PropertyState.StateName = Transition.TempName + TransitionScheduler.States.BoardSuffix;
            TransitionApplied.BeginTransition(ToState(), TransitionParams);
            return Task.CompletedTask;
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            TransitionScheduler.Interrupt(IsUnsafeStoped);
        }
        public IExecutableTransition Compile()
        {
            var copy = new TransitionMeta()
            {
                TransitionApplied = TransitionApplied,
                TransitionParams = TransitionParams.DeepCopy(),
                PropertyState = PropertyState.DeepCopy(),
            };
            return copy;
        }
    }
}
