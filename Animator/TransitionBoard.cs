using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Media;
using System.Windows;
using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionBoard<T> : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta, IConvertibleTransitionMeta, IFramePreloading, IPropertyRecorder<TransitionBoard<T>, T>, IExecutableTransition, ITransitionWithTarget where T : class
    {
        internal TransitionBoard() { }
        internal State TempState { get; set; } = new State() { StateName = Transition.TempName };
        internal bool IsPreloaded { get; set; } = false;

        public object? Target { get; set; }
        public TransitionParams TransitionParams { get; set; } = new();
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; } = [];
        public StateMachine Machine => Target == null ? throw new ArgumentNullException(nameof(Target), "The metadata is missing the target instance for this transition effect") : StateMachine.Create(Target);
        public void PreLoad(TransitionParams transitionParams)
        {
            if (!IsPreloaded)
            {
                TransitionParams = transitionParams;
                PreLoad();
            }
        }
        public TransitionMeta Merge<T1>(ICollection<T1> metas) where T1 : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta
        {
            if (!IsPreloaded)
            {
                PreLoad();
            }
            foreach (var meta in metas)
            {
                var target = meta as IFramePreloading;
                target?.PreLoad(TransitionParams);
            }
            TransitionMeta transitionMeta = new(this)
            {
                Target = Target
            };
            transitionMeta.Merge(metas);
            return transitionMeta;
        }
        public List<List<Tuple<PropertyInfo, List<object?>>>> RecomputeFrames(int fps)
        {
            TransitionMeta transitionMeta = new(this)
            {
                Target = Target
            };
            transitionMeta.RecomputeFrames(fps);
            FrameSequence = transitionMeta.FrameSequence;
            return FrameSequence;
        }
        public State ToState()
        {
            TransitionMeta transitionMeta = new(this)
            {
                Target = Target
            };
            return transitionMeta.ToState();
        }
        public TransitionBoard<T1> ToTransitionBoard<T1>() where T1 : class
        {
            TransitionMeta transitionMeta = new(this)
            {
                Target = Target
            };
            return transitionMeta.ToTransitionBoard<T1>();
        }
        public TransitionMeta ToTransitionMeta()
        {
            TransitionMeta transitionMeta = new(this)
            {
                Target = Target
            };
            return transitionMeta;
        }
        public void Start(object? target = null)
        {
            if (target == null)
            {
                if (Target == null)
                {
                    throw new ArgumentNullException(nameof(target), "The metadata is missing the target instance for this transition effect");
                }
                else
                {
                    var Machine = StateMachine.Create(Target);
                    Machine.Interrupt();
                    TempState.StateName = Transition.TempName + Machine.States.BoardSuffix;
                    Machine.States.Add(TempState);
                    Machine.Transition(TempState.StateName, TransitionParams, IsPreloaded ? FrameSequence : null);
                }
            }
            else
            {
                var Machine = StateMachine.Create(target);
                Machine.Interrupt();
                TempState.StateName = Transition.TempName + Machine.States.BoardSuffix;
                Machine.States.Add(TempState);
                Machine.Transition(TempState.StateName, TransitionParams, IsPreloaded ? FrameSequence : null);
            }
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, double>> propertyLambda, double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, Brush>> propertyLambda, Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, Transform>> propertyLambda, params Transform[] newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Transform) || newValue.Length == 0)
                {
                    return this;
                }
                var value = newValue.Select(t => t.Value).Aggregate(Matrix.Identity, (acc, matrix) => acc * matrix);
                var interpolatedMatrixStr = $"{value.M11},{value.M12},{value.M21},{value.M22},{value.OffsetX},{value.OffsetY}";
                var result = Transform.Parse(interpolatedMatrixStr);
                TempState.AddProperty(property.Name, (object?)result);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, Point>> propertyLambda, Point newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, CornerRadius>> propertyLambda, CornerRadius newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, Thickness>> propertyLambda, Thickness newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetProperty(Expression<Func<T, IInterpolable>> propertyLambda, IInterpolable newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || !typeof(IInterpolable).IsAssignableFrom(property.PropertyType))
                {
                    return this;
                }
                TempState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            Machine.Interrupt(IsUnsafeStoped);
        }

        public TransitionBoard<T> SetParams(Action<TransitionParams> modifyParams)
        {
            var temp = new TransitionParams();
            modifyParams(temp);
            if (temp.FrameRate != TransitionParams.FrameRate)
            {
                RecomputeFrames(temp.FrameRate);
            }
            TransitionParams = temp;
            return this;
        }
        public TransitionBoard<T> SetParams(TransitionParams newParams)
        {
            if (TransitionParams.FrameRate != newParams.FrameRate)
            {
                RecomputeFrames(newParams.FrameRate);
            }
            TransitionParams = newParams;
            return this;
        }
        public TransitionBoard<T> Reflect(T reflected, params string[] blackList)
        {
            TempState = new(reflected, Array.Empty<string>(), blackList)
            {
                StateName = Transition.TempName
            };
            PreLoad(reflected);
            return this;
        }
        public TransitionBoard<T> PreLoad(T target)
        {
            FrameSequence = StateMachine.PreloadFrames(target, TempState, TransitionParams) ?? [];
            IsPreloaded = true;
            return this;
        }
        public TransitionBoard<T> PreLoad()
        {
            if (Target != null)
            {
                FrameSequence = StateMachine.PreloadFrames(Target, TempState, TransitionParams) ?? [];
                IsPreloaded = true;
            }
            return this;
        }
    }
}
