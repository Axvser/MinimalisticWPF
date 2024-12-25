using System.Reflection;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows;
using System.Collections.Concurrent;
using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF.Animator
{
    public sealed class State : ITransitionMeta
    {
        internal State() { }
        internal State(object Target, ICollection<string> WhileList, ICollection<string> BlackList)
        {
            var type = Target.GetType();
            StateMachine.InitializeTypes(type);
            if (StateMachine.PropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var info in infodictionary.Values.Where(x => WhileList.Count > 0 ? WhileList.Contains(x.Name) : false || BlackList.Count <= 0 || !BlackList.Contains(x.Name)))
                {
                    Values.TryAdd(info.Name, info.GetValue(Target));
                }
            }
        }

        public string StateName { get; internal set; } = string.Empty;
        public ConcurrentDictionary<string, object?> Values { get; internal set; } = new ConcurrentDictionary<string, object?>();
        public TransitionParams TransitionParams { get; set; } = new();
        public State PropertyState
        {
            get => this;
            set
            {
                StateName = value.StateName;
                Values = value.Values;
                TransitionParams = value.TransitionParams;
            }
        }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence => [];

        public object? this[string propertyName]
        {
            get
            {
                if (!Values.TryGetValue(propertyName, out _))
                {
                    throw new ArgumentException($"There is no property State value named [ {propertyName} ] in the state named [ {StateName} ]");
                }

                return Values[propertyName];
            }
        }
        public void AddProperty(string propertyName, object? value)
        {
            if (Values.TryGetValue(propertyName, out var ori))
            {
                Values.TryUpdate(propertyName, value, ori);
            }
            else
            {
                Values.TryAdd(propertyName, value);
            }
        }
        public static ObjectTempState<T> FromObject<T>(T Target) where T : class
        {
            ObjectTempState<T> result = new(Target);
            return result;
        }
        public static TypeTempState<T> FromType<T>() where T : class
        {
            TypeTempState<T> state = new();
            return state;
        }
        public State Merge(ICollection<ITransitionMeta> metas)
        {
            foreach (var meta in metas)
            {
                foreach (var values in meta.PropertyState.Values)
                {
                    AddProperty(values.Key, values.Value);
                }
            }
            return this;
        }
        public State Merge(ITransitionMeta meta)
        {
            foreach (var values in meta.PropertyState.Values)
            {
                AddProperty(values.Key, values.Value);
            }
            return this;
        }

        public State DeepCopy()
        {
            var newState = new State
            {
                StateName = this.StateName,
                TransitionParams = this.TransitionParams.DeepCopy(),
            };

            foreach (var kvp in this.Values)
            {
                newState.Values[kvp.Key] = kvp.Value;
            }

            return newState;
        }
    }

    public class ObjectTempState<T> : IPropertyRecorder<ObjectTempState<T>, T>
    {
        internal ObjectTempState(T target) { Value = target; }

        internal T Value { get; set; }
        internal string Name { get; set; } = string.Empty;
        internal List<string> WhiteList { get; set; } = [];
        internal string[] BlackList { get; set; } = [];

        public ObjectTempState<T> SetName(string stateName)
        {
            Name = stateName;
            return this;
        }

        public ObjectTempState<T> SetProperty(
            Expression<Func<T, double>> propertyLambda,
            double newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Brush>> propertyLambda,
            Brush newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Transform>> propertyLambda,
            params Transform[] newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Transform))
                {
                    return this;
                }

                var value = newValue.Select(t => t.Value).Aggregate(Matrix.Identity, (acc, matrix) => acc * matrix);
                var interpolatedMatrixStr = $"{value.M11},{value.M12},{value.M21},{value.M22},{value.OffsetX},{value.OffsetY}";
                var result = Transform.Parse(interpolatedMatrixStr);

                property.SetValue(Value, result);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Point>> propertyLambda,
            Point newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, CornerRadius>> propertyLambda,
            CornerRadius newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, Thickness>> propertyLambda,
            Thickness newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }
        public ObjectTempState<T> SetProperty(
            Expression<Func<T, IInterpolable>> propertyLambda,
            IInterpolable newValue)
        {
            if (Value == null)
            {
                return this;
            }

            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(IInterpolable))
                {
                    return this;
                }
                property.SetValue(Value, newValue);
                WhiteList.Add(property.Name);
            }

            return this;
        }

        public ObjectTempState<T> Except(params Expression<Func<T, string>>[] properties)
        {
            BlackList = properties.Select(p => ((MemberExpression)p.Body).Member.Name).ToArray();
            return this;
        }

        public State ToState(bool IsWhiteList = true)
        {
            if (Value == null) throw new ArgumentException("Target object loss");
            if (string.IsNullOrEmpty(Name)) throw new ArgumentException("The State name cannot be empty");
            if (!IsWhiteList) WhiteList.Clear();
            State result = new(Value, WhiteList, BlackList)
            {
                StateName = Name
            };
            return result;
        }
    }
    public class TypeTempState<T> : IPropertyRecorder<TypeTempState<T>, T>
    {
        internal TypeTempState() { Target = new State(); }

        internal State Target { get; set; }

        public TypeTempState<T> SetName(string name)
        {
            Target.StateName = name;
            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, double>> propertyLambda,
            double newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(double))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Brush>> propertyLambda,
            Brush newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Brush))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, CornerRadius>> propertyLambda,
            CornerRadius newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(CornerRadius))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Point>> propertyLambda,
            Point newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Point))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Thickness>> propertyLambda,
            Thickness newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Thickness))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, Transform>> propertyLambda,
            params Transform[] newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(Transform))
                {
                    return this;
                }

                var value = newValue.Select(t => t.Value).Aggregate(Matrix.Identity, (acc, matrix) => acc * matrix);
                var interpolatedMatrixStr = $"{value.M11},{value.M12},{value.M21},{value.M22},{value.OffsetX},{value.OffsetY}";
                var result = Transform.Parse(interpolatedMatrixStr);

                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = result;
                }
                else
                {
                    Target.AddProperty(property.Name, result);
                }
            }

            return this;
        }
        public TypeTempState<T> SetProperty(
            Expression<Func<T, IInterpolable>> propertyLambda,
            IInterpolable newValue)
        {
            if (propertyLambda.Body is MemberExpression propertyExpr)
            {
                var property = propertyExpr.Member as PropertyInfo;
                if (property == null || !property.CanWrite || property.PropertyType != typeof(IInterpolable))
                {
                    return this;
                }
                if (Target.Values.ContainsKey(property.Name))
                {
                    Target.Values[property.Name] = newValue;
                }
                else
                {
                    Target.AddProperty(property.Name, newValue);
                }
            }

            return this;
        }
        public State ToState()
        {
            return Target;
        }
    }
}
