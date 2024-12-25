using MinimalisticWPF.StructuralDesign.Animator;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionMeta : IRecomputableTransitionMeta, IMergeableTransition, ITransitionMeta, IConvertibleTransitionMeta, IExecutableTransition, ITransitionWithTarget
    {
        internal TransitionMeta() { }
        public TransitionMeta(TransitionParams transitionParams, List<List<Tuple<PropertyInfo, List<object?>>>> frames)
        {
            TransitionParams = transitionParams;
            FrameSequence = frames;
        }
        public TransitionMeta(ITransitionMeta transitionMeta)
        {
            TransitionParams = transitionMeta.TransitionParams;
            FrameSequence = transitionMeta.FrameSequence;
        }
        public TransitionMeta(params TransitionMeta[] transitionMetas)
        {
            Merge(transitionMetas);
        }

        public TransitionMeta Merge<T>(params T[] metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            return Merge(metas);
        }
        public IExecutableTransition ToPlayer()
        {
            return this;
        }

        public object? Target { get; set; }
        public TransitionParams TransitionParams { get; set; } = new();
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; } = [];
        public StateMachine Machine => Target == null ? throw new ArgumentNullException(nameof(Target), "The metadata is missing the target instance for this transition effect") : StateMachine.Create(Target);
        public TransitionMeta Merge<T>(ICollection<T> metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            return MergeSequence(metas);
        }
        public List<List<Tuple<PropertyInfo, List<object?>>>> RecomputeFrames(int fps)
        {
            for (int i = 0; i < FrameSequence.Count; i++)
            {
                for (int j = 0; j < FrameSequence[i].Count; j++)
                {
                    var start = FrameSequence[i][j].Item2.FirstOrDefault();
                    var end = FrameSequence[i][j].Item2.LastOrDefault();
                    if (FrameSequence[i][j].Item1.PropertyType == typeof(double))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.DoubleComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Brush))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.BrushComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Transform))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.TransformComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Point))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.PointComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(Thickness))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.ThicknessComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(CornerRadius))
                    {
                        FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, LinearInterpolation.CornerRadiusComputing(start, end, fps));
                    }
                    else if (FrameSequence[i][j].Item1.PropertyType == typeof(IInterpolable))
                    {
                        if (start != null && end != null)
                        {
                            var ac0 = (IInterpolable)start;
                            if (ac0 != null)
                            {
                                FrameSequence[i][j] = Tuple.Create(FrameSequence[i][j].Item1, ac0.Interpolate(start, end, fps));
                            }
                        }
                    }
                }
            }
            return FrameSequence;
        }
        public TransitionMeta ToTransitionMeta()
        {
            return this;
        }
        public State ToState()
        {
            var state = new State();
            foreach (var frames in FrameSequence)
            {
                foreach (var frame in frames)
                {
                    state.AddProperty(frame.Item1.Name, frame.Item2.LastOrDefault());
                }
            }
            return state;
        }
        public TransitionBoard<T> ToTransitionBoard<T>() where T : class
        {
            var result = new TransitionBoard<T>
            {
                TempState = ToState(),
                Target = Target,
                TransitionParams = TransitionParams,
                FrameSequence = FrameSequence
            };
            return result;
        }
        public void Start(object? target = null)
        {
            Target = target ?? Target;
            if (Target == null) throw new ArgumentNullException(nameof(target), "The metadata is missing the target instance for this transition effect");
            Target.BeginTransition(ToState(), TransitionParams);
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            Machine.Interrupt(IsUnsafeStoped);
        }

        private TransitionMeta MergeSequence<T>(ICollection<T> metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            foreach (var meta in metas)
            {
                if (meta.TransitionParams.FrameRate != TransitionParams.FrameRate)
                {
                    meta.RecomputeFrames(TransitionParams.FrameRate);
                }
                MergeFrameSequences(meta.FrameSequence);
            }
            return this;
        }
        private TransitionMeta MergeFrameSequences(List<List<Tuple<PropertyInfo, List<object?>>>> source)
        {
            foreach (var propertyFrames in source)
            {
                foreach (var frame in propertyFrames)
                {
                    var old = FrameSequence.Select(pf => pf.FirstOrDefault(f => f.Item1 == frame.Item1)).FirstOrDefault();
                    if (old == null)
                    {
                        FrameSequence.Add([frame]);
                    }
                    else
                    {
                        old = frame;
                    }
                }
            }
            return this;
        }
    }
}
