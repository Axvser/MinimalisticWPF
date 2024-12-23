using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF
{
    public class TransitionBoard<T> where T : class
    {
        internal TransitionBoard() { }
        internal TransitionBoard(T target)
        {
            Target = target;
            Machine = StateMachine.Create(target);
        }
        internal bool IsStatic { get; set; } = false;
        internal T? Target { get; set; }
        internal StateMachine? Machine { get; set; }
        internal State TempState { get; set; } = new State() { StateName = Transition.TempName };
        internal Action<TransitionParams>? TransitionParams { get; set; }
        internal TransitionParams? ParamsInstance { get; set; }
        internal List<List<Tuple<PropertyInfo, List<object?>>>>? Preload { get; set; }

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
        public TransitionBoard<T> SetParams(Action<TransitionParams> modifyParams)
        {
            TransitionParams = modifyParams;
            return this;
        }
        public TransitionBoard<T> ReflectAny(T reflected)
        {
            TempState = new(reflected, Array.Empty<string>(), Array.Empty<string>())
            {
                StateName = Transition.TempName
            };
            return this;
        }
        public TransitionBoard<T> PreLoad()
        {
            var par = new TransitionParams();
            TransitionParams?.Invoke(par);
            Preload = StateMachine.PreloadFrames(Target, TempState, par);
            return this;
        }
        public TransitionBoard<T> PreLoad(T target)
        {
            var par = new TransitionParams();
            TransitionParams?.Invoke(par);
            Preload = StateMachine.PreloadFrames(target, TempState, par);
            return this;
        }
        public void Start()
        {
            if (IsStatic) throw new InvalidOperationException("This method cannot be used under Type-based creation, instead use an overloaded version of the Start( object ) method");
            if (Machine == null) throw new ArgumentException("StateMachine instance lost");
            if (Target == null) throw new ArgumentException("Target object instance lost");
            Machine.Interrupt();
            TempState.StateName = Transition.TempName + "NonStatic";
            Machine.States.Add(TempState);
            Machine.Transition(TempState.StateName, TransitionParams, Preload);
        }
        public void Start(T target)
        {
            Machine = StateMachine.Create(target);
            Machine.Interrupt();
            TempState.StateName = Transition.TempName + Machine.States.BoardSuffix;
            Machine.States.Add(TempState);
            Machine.Transition(TempState.StateName, TransitionParams, Preload);
        }
    }
}
