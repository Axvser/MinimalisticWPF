using MinimalisticWPF.StructuralDesign.Pool;
using System.Collections.Concurrent;
using System.Windows;

namespace MinimalisticWPF.ObjectPool
{
    public static class Pool
    {
        internal static ConcurrentDictionary<Type, HashSet<object>> Source { get; set; } = new();
        internal static ConcurrentDictionary<Type, HashSet<object>> Dispose { get; set; } = new();
        public static void Release(object instance)
        {
            if (instance is IPoolApplied methods)
            {
                var type = instance.GetType();
                BuildPool(type);
                var c1 = Source.TryGetValue(type, out var sourceSet) && sourceSet.Contains(instance);
                var c2 = Dispose.TryGetValue(type, out var disposeSet) && disposeSet.Contains(instance);

                if (!c1 && c2)
                {
                    methods.RunReleasing();
                    disposeSet?.Remove(instance);
                    sourceSet?.Add(instance);
                    methods.RunReleased();
                }
                else if (c1 && c2)
                {
                    methods.RunReleasing();
                    disposeSet?.Remove(instance);
                    methods.RunReleased();
                }
                else
                {
                    sourceSet?.Add(instance);
                }
            }
        }
        public static object Reuse(Type type, params object?[] parameters)
        {
            if (!typeof(IPoolApplied).IsAssignableFrom(type)) throw new ArgumentException($"{type.Name} is not a IPoolApplied");
            BuildPool(type);
            var c1 = Source.TryGetValue(type, out var sourceSet);
            var c2 = Dispose.TryGetValue(type, out var disposeSet);
            if (c1)
            {
                if (sourceSet?.Count > 0)
                {
                    var instance = sourceSet.First();
                    if (instance is IPoolApplied methods)
                    {
                        methods.RunReusing();
                        disposeSet?.Add(instance);
                        methods.RunReused();
                    }
                    return instance ?? throw new ArgumentException("The resource pool was emptied in advance under unknown circumstances");
                }
                else
                {
                    var newSource = Activator.CreateInstance(type, parameters);
                    if (newSource != null && c2)
                    {
                        disposeSet?.Add(newSource);
                    }
                    return newSource ?? throw new ArgumentException("An accident occurred during automatic instance generation");
                }
            }
            throw new NotImplementedException("Suspected failure to add key-value pairs to Pool");
        }
        public static void Record(object instance)
        {
            if (instance is IPoolApplied)
            {
                var type = instance.GetType();
                BuildPool(type);
                var c1 = Source.TryGetValue(type, out var sourceSet);
                var c2 = Dispose.TryGetValue(type, out var disposeSet);
                if (c1 && c2)
                {
                    if (sourceSet?.Contains(instance) == false && disposeSet?.Contains(instance) == false)
                    {
                        sourceSet.Add(instance);
                    }
                    else
                    {
                        disposeSet?.Remove(instance);
                        sourceSet?.Add(instance);
                    }
                }
            }
        }
        private static void BuildPool(Type type)
        {

            if (!Source.TryGetValue(type, out _))
            {
                Source.TryAdd(type, []);
            }
            if (!Dispose.TryGetValue(type, out _))
            {
                Dispose.TryAdd(type, []);
            }
        }
    }
}
