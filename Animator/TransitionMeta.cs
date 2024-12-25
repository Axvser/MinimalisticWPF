using MinimalisticWPF.StructuralDesign.Animator;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionMeta : IMergeableTransition, ITransitionMeta, IConvertibleTransitionMeta, IExecutableTransition, ITransitionWithTarget
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

        public object? Target { get; set; }
        public TransitionParams TransitionParams { get; set; } = new();
        public State PropertyState { get; set; } = new State() { StateName = Transition.TempName };
        public StateMachine Machine => Target == null ? throw new ArgumentNullException(nameof(Target), "The metadata is missing the target instance for this transition effect") : StateMachine.Create(Target);
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence => StateMachine.PreloadFrames(Target, PropertyState, TransitionParams) ?? [];
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
                Target = Target,
                TransitionParams = TransitionParams,
            };
            return result;
        }
        public void Start(object? target = null)
        {
            Target = target ?? Target;
            if (Target == null) throw new ArgumentNullException(nameof(target), "The metadata is missing the target instance for this transition effect");
            PropertyState.StateName = Transition.TempName + Machine.States.BoardSuffix;
            Target.BeginTransition(ToState(), TransitionParams);
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            Machine.Interrupt(IsUnsafeStoped);
        }
    }
}
