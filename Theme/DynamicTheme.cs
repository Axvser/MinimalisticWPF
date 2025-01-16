using MinimalisticWPF.Extension;
using MinimalisticWPF.StructuralDesign.Theme;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.Theme
{
    public static class DynamicTheme
    {
        public static ConcurrentDictionary<Type, ConcurrentDictionary<Type, State>> TransitionSource { get; internal set; } = new();
        public static HashSet<object> GlobalInstance { get; internal set; } = new(64);

        private static bool _isloaded = false;
        public static object? GetThemeValue(Type classType, Type attributeType, string propertyName)
        {
            Awake();
            if (TransitionSource.TryGetValue(classType, out var statesKVP))
            {
                if (statesKVP.TryGetValue(attributeType, out var state))
                {
                    if (state.Values.TryGetValue(propertyName, out var value))
                    {
                        return value;
                    }
                }
            }
            return null;
        }
        public static void Awake()
        {
            if (!_isloaded)
            {
                var Assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
                var classAssemblies = Assemblies.Where(t => t.GetCustomAttribute(typeof(DynamicThemeAttribute), true) != null);
                var attributeAssemblies = Assemblies.Where(t => typeof(IThemeAttribute).IsAssignableFrom(t) && typeof(Attribute).IsAssignableFrom(t) && !t.IsAbstract);
                KVPGeneration(classAssemblies, attributeAssemblies);
                _isloaded = true;
            }
        }
        public static void Apply(Type attributeType, TransitionParams? param)
        {
            Awake();
            foreach (var item in GlobalInstance)
            {
                if (item is IThemeApplied target)
                {
                    param ??= TransitionParams.Theme.DeepCopy();
                    param.Start += () =>
                    {
                        target.IsThemeChanging = true;
                    };
                    param.Completed += () =>
                    {
                        var old = target.CurrentTheme;
                        target.CurrentTheme = attributeType;
                        target.IsThemeChanging = false;
                        target.OnThemeChanged(old, attributeType);
                    };
                    target.OnThemeChanging(target.CurrentTheme, attributeType);
                    item.ApplyTheme(attributeType, param);
                }
                else
                {
                    param ??= TransitionParams.Theme.DeepCopy();
                    item.ApplyTheme(attributeType, param);
                }
            }
        }
        private static void KVPGeneration(IEnumerable<Type> classes, IEnumerable<Type> attributes)
        {
            foreach (var cs in classes)
            {
                TransitionScheduler.InitializeTypes(cs);
                if (!TransitionScheduler.SplitedPropertyInfos.TryGetValue(cs, out var group)) break;
                var unit = new ConcurrentDictionary<Type, State>();
                foreach (var attribute in attributes)
                {
                    var properties = cs.GetProperties()
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        Context = p.GetCustomAttribute(attribute, true) as IThemeAttribute,
                    });
                    var state = new State();
                    foreach (var info in properties)
                    {
                        if (info.PropertyInfo.CanWrite && info.PropertyInfo.CanRead && info.Context != null)
                        {
                            Func<int> func = (group.Item1.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item2.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item3.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item4.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item5.TryGetValue(info.PropertyInfo.Name, out _),
                                          group.Item6.TryGetValue(info.PropertyInfo.Name, out _))
                            switch
                            {
                                (true, false, false, false, false, false) => () =>
                                {
                                    var value = Convert.ToDouble(info.Context.Parameters.FirstOrDefault() ?? 0);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 1;
                                }
                                ,
                                (false, true, false, false, false, false) => () =>
                                {
                                    var value = info.Context.Parameters.FirstOrDefault()?.ToString()?.ToBrush() ?? Brushes.Transparent;
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 2;
                                }
                                ,
                                (false, false, true, false, false, false) => () =>
                                {
                                    var value = Transform.Parse(info.Context.Parameters.FirstOrDefault()?.ToString() ?? Transform.Identity.ToString());
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 3;
                                }
                                ,
                                (false, false, false, true, false, false) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(Point), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 4;
                                }
                                ,
                                (false, false, false, false, true, false) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(CornerRadius), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 5;
                                }
                                ,
                                (false, false, false, false, false, true) => () =>
                                {
                                    var value = Activator.CreateInstance(typeof(Thickness), info.Context.Parameters);
                                    state.AddProperty(info.PropertyInfo.Name, value);
                                    return 6;
                                }
                                ,
                                _ => () => { return -1; }
                            };
                            func.Invoke();
                        }
                    }
                    unit.TryAdd(attribute, state);
                }
                TransitionSource.TryAdd(cs, unit);
            }
        }
    }
}
