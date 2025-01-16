using MinimalisticWPF.StructuralDesign.Pool;
using System.Collections.Concurrent;
using System.Reflection;

namespace MinimalisticWPF.ObjectPool
{
    public static class Pool
    {
        public static ConcurrentDictionary<Type, ConcurrentQueue<object>> Source { get; internal set; } = new();
        public static ConcurrentDictionary<Type, HashSet<object>> Dispose { get; internal set; } = new();
        public static void Release(object instance)
        {
            if (instance is IPoolApplied methods)
            {
                var type = instance.GetType();
                if (Dispose.TryGetValue(type, out var tarter) && tarter.Contains(instance))
                {
                    if (Source.TryGetValue(type, out var queue) && methods.CanRelease())
                    {
                        methods.OnReusing();
                        queue.Enqueue(instance);
                        methods.OnReused();
                    }
                }
            }
        }
        public static object Dequeue(Type type, params object?[] parameters)
        {
            if (Source.TryGetValue(type, out var queue))
            {
                if (queue.TryPeek(out var fake) && fake is IPoolApplied methods)
                {
                    methods.OnReusing();
                    if (queue.TryDequeue(out var source) && Dispose.TryGetValue(type, out var dispose))
                    {
                        dispose.Add(source);
                        methods.OnReused();
                        return source;
                    }
                }
            }
            try
            {
                var dyn = Activator.CreateInstance(type, parameters);
                return dyn ?? throw new ArgumentException("Failed to automatically create a new resource when the object pool has no available resources");
            }
            catch
            {
                throw new ArgumentException("Failed to automatically create a new resource when the object pool has no available resources");
            }
        }
    }
}
