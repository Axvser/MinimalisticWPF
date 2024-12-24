using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IPropertyRecorder<T0, T1>
    {
        public T0 SetProperty(Expression<Func<T1, double>> propertyLambda, double newValue);
        public T0 SetProperty(Expression<Func<T1, Brush>> propertyLambda, Brush newValue);
        public T0 SetProperty(Expression<Func<T1, Transform>> propertyLambda, params Transform[] newValue);
        public T0 SetProperty(Expression<Func<T1, Point>> propertyLambda, Point newValue);
        public T0 SetProperty(Expression<Func<T1, CornerRadius>> propertyLambda, CornerRadius newValue);
        public T0 SetProperty(Expression<Func<T1, Thickness>> propertyLambda, Thickness newValue);
        public T0 SetProperty(Expression<Func<T1, IInterpolable>> propertyLambda, IInterpolable newValue);
    }
}
