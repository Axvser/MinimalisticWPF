using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Media;
using System.Windows;
using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF.Animator
{
    public sealed class TransitionBoard<T> : ITransitionMeta, IMergeableTransition, IConvertibleTransitionMeta, IPropertyRecorder<TransitionBoard<T>, T>, IExecutableTransition, ITransitionWithTarget, ICompilableTransition where T : class
    {
        private object? _target = null;
        private TransitionParams _params = new();
        private State _propertyState = new() { StateName = Transition.TempName };
        private List<List<Tuple<PropertyInfo, List<object?>>>> _frameSequence = [];

        internal TransitionBoard() { }
        public bool IsPreloaded { get; internal set; } = false;
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence
        {
            get
            {
                if (!IsPreloaded)
                {
                    _frameSequence = StateMachine.PreloadFrames(Target, PropertyState, TransitionParams) ?? [];
                    return _frameSequence;
                }
                return _frameSequence;
            }
        }

        public object? Target
        {
            get => _target;
            set
            {
                if (value != _target)
                {
                    _target = value;
                    IsPreloaded = false;
                }
            }
        }
        public TransitionParams TransitionParams
        {
            get => _params;
            set
            {
                if (value.FrameRate != _params.FrameRate)
                {
                    IsPreloaded = false;
                }
                _params = value;
            }
        }
        public State PropertyState
        {
            get => _propertyState;
            set
            {
                IsPreloaded = false;
                _propertyState = value;
            }
        }
        public StateMachine Machine => Target == null ? throw new ArgumentNullException(nameof(Target), "The metadata is missing the target instance for this transition effect") : StateMachine.Create(Target);
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
                    PropertyState.StateName = Transition.TempName + Machine.States.BoardSuffix;
                    Machine.States.Add(PropertyState);
                    Machine.Transition(PropertyState.StateName, TransitionParams, IsPreloaded ? FrameSequence : null);
                }
            }
            else
            {
                Target = target;
                var Machine = StateMachine.Create(target);
                Machine.Interrupt();
                PropertyState.StateName = Transition.TempName + Machine.States.BoardSuffix;
                Machine.States.Add(PropertyState);
                Machine.Transition(PropertyState.StateName, TransitionParams, IsPreloaded ? FrameSequence : null);
            }
        }
        public void Stop(bool IsUnsafeStoped = false)
        {
            Machine.Interrupt(IsUnsafeStoped);
        }
        public ITransitionMeta Merge(ICollection<ITransitionMeta> metas)
        {
            var result = IMergeableTransition.MergeMetas(metas);
            PropertyState = result.PropertyState;
            return result;
        }
        public State ToState()
        {
            return PropertyState;
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
        public TransitionBoard<T> SetProperty(Expression<Func<T, double>> propertyLambda, double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                PropertyState.AddProperty(property.Name, (object?)newValue);
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
                PropertyState.AddProperty(property.Name, (object?)newValue);
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
                PropertyState.AddProperty(property.Name, (object?)result);
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
                PropertyState.AddProperty(property.Name, (object?)newValue);
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
                PropertyState.AddProperty(property.Name, (object?)newValue);
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
                PropertyState.AddProperty(property.Name, (object?)newValue);
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
                PropertyState.AddProperty(property.Name, (object?)newValue);
            }
            return this;
        }
        public TransitionBoard<T> SetParams(Action<TransitionParams> modifyParams)
        {
            var temp = new TransitionParams();
            modifyParams(temp);
            TransitionParams = temp;
            return this;
        }
        public TransitionBoard<T> SetParams(TransitionParams newParams)
        {
            TransitionParams = newParams;
            return this;
        }
        public IExecutableTransition Compile()
        {
            var meta = new TransitionMeta()
            {
                Target = this.Target,
                TransitionParams = this.TransitionParams.DeepCopy(),
                PropertyState = this.PropertyState.DeepCopy()
            };
            return meta;
        }
    }
}
