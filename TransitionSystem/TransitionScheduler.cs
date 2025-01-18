using System.Reflection;
using System.Windows;
using System.Data;
using System.Windows.Media;
using System.Collections.Concurrent;
using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem.Basic;

namespace MinimalisticWPF.TransitionSystem
{
    public sealed class TransitionScheduler
    {
        public static int MaxFrameRate
        {
            get => _maxFR;
            set
            {
                _maxFR = Math.Clamp(value, 1, int.MaxValue);
            }
        }
        public static ConcurrentDictionary<Type, ConcurrentDictionary<object, TransitionScheduler>> MachinePool { get; internal set; } = new();
        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> PropertyInfos { get; internal set; } = new();
        public static ConcurrentDictionary<Type, Tuple<ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>, ConcurrentDictionary<string, PropertyInfo>>> SplitedPropertyInfos { get; internal set; } = new();

        public static TransitionScheduler CreateOrFind(object targetObj, params State[] states)
        {
            var type = targetObj.GetType();
            if (MachinePool.TryGetValue(type, out var machinedictionary))
            {
                if (machinedictionary.TryGetValue(targetObj, out var machine))
                {
                    foreach (var state in states)
                    {
                        machine.States.Add(state);
                    }
                    return machine;
                }
                else
                {
                    var newMachine = new TransitionScheduler(targetObj, states);
                    machinedictionary.TryAdd(targetObj, newMachine);
                    return newMachine;
                }
            }
            else
            {
                var newMachine = new TransitionScheduler(targetObj, states);
                var newChildDic = new ConcurrentDictionary<object, TransitionScheduler>();
                newChildDic.TryAdd(targetObj, newMachine);
                MachinePool.TryAdd(type, newChildDic);
                return newMachine;
            }
        }
        public static List<List<Tuple<PropertyInfo, List<object?>>>>? PreloadFrames(object? TransitionApplied, State state, TransitionParams par)
        {
            if (TransitionApplied == null)
            {
                return null;
            }
            var machine = new TransitionScheduler(TransitionApplied, state)
            {
                TransitionParams = par
            };
            var result = ComputingFrames(state, machine);
            return result;
        }
        public static void InitializeTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                if (!PropertyInfos.ContainsKey(type))
                {
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.CanRead &&
                    (x.PropertyType == typeof(double)
                    || x.PropertyType == typeof(Brush)
                    || x.PropertyType == typeof(Transform)
                    || x.PropertyType == typeof(Point)
                    || x.PropertyType == typeof(CornerRadius)
                    || x.PropertyType == typeof(Thickness)
                    || typeof(IInterpolable).IsAssignableFrom(x.PropertyType)
                    ));
                    var propdictionary = new ConcurrentDictionary<string, PropertyInfo>();
                    foreach (var property in properties)
                    {
                        propdictionary.TryAdd(property.Name, property);
                    }
                    PropertyInfos.TryAdd(type, propdictionary);
                    SplitedPropertyInfos.TryAdd(type, Tuple.Create(new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(double)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Brush)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Transform)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Point)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(CornerRadius)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => x.PropertyType == typeof(Thickness)).ToDictionary(x => x.Name, x => x)),
                                                          new ConcurrentDictionary<string, PropertyInfo>(properties.Where(x => typeof(IInterpolable).IsAssignableFrom(x.PropertyType)).ToDictionary(x => x.Name, x => x))));
                }
            }
        }

        public static bool TryGetPropertyInfo(Type type, string propertyname, out PropertyInfo? result)
        {
            if (PropertyInfos.TryGetValue(type, out var propdic))
            {
                if (propdic.TryGetValue(propertyname, out var propertyInfo))
                {
                    result = propertyInfo;
                    return true;
                }
            }
            result = null;
            return false;
        }
        public static bool TryGetScheduler(object target, out TransitionScheduler? result)
        {
            if (MachinePool.TryGetValue(target.GetType(), out var machinedic))
            {
                if (machinedic.TryGetValue(target, out var machine))
                {
                    result = machine;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public object TransitionApplied { get; internal set; }
        public Type Type { get; internal set; }
        public StateCollection States { get; internal set; } = [];
        public double DeltaTime { get => 1000.0 / Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate); }
        public double FrameCount { get => Math.Clamp(TransitionParams.Duration * Math.Clamp(TransitionParams.FrameRate, 1, MaxFrameRate), 1, int.MaxValue); }
        public string? CurrentState { get; internal set; }
        public IExecutableTransition? Interpreter { get; internal set; }
        public ConcurrentQueue<Tuple<string, ITransitionMeta>> Interpreters { get; internal set; } = new();

        public TransitionScheduler Copy()
        {
            var result = new TransitionScheduler(TransitionApplied);
            foreach (var state in States)
            {
                result.States.Add(state);
            }
            return result;
        }

        public void Interrupt()
        {
            IsReSet = true;
            CurrentState = null;
            Interpreter?.Stop();
            Interpreter = null;
            Interpreters.Clear();
        }
        public void Transition(string stateName, Action<TransitionParams>? actionSet, List<List<Tuple<PropertyInfo, List<object?>>>>? preload = null)
        {
            IsReSet = false;

            TransitionParams temp = new();
            actionSet?.Invoke(temp);

            if (Interpreter == null)
            {
                InterpreterScheduler(stateName, temp, preload);
            }
            else
            {
                var targetInterpreter = Interpreters.Where(x => x.Item1 == stateName).ToArray();
                Interpreters.Enqueue(Tuple.Create<string, ITransitionMeta>(stateName, new TransitionMeta(temp, preload ?? [])));
            }
        }
        public void Transition(string stateName, TransitionParams? param, List<List<Tuple<PropertyInfo, List<object?>>>>? preload = null)
        {
            IsReSet = false;

            var temp = param ?? new();

            if (Interpreter == null)
            {
                InterpreterScheduler(stateName, temp, preload);
            }
            else
            {
                var targetInterpreter = Interpreters.Where(x => x.Item1 == stateName).ToArray();
                Interpreters.Enqueue(Tuple.Create<string, ITransitionMeta>(stateName, new TransitionMeta(temp, preload ?? [])));
            }
        }

        internal async void InterpreterScheduler(string stateName, TransitionParams? actionSet, List<List<Tuple<PropertyInfo, List<object?>>>>? preload)
        {
            var targetState = States[stateName];
            actionSet ??= new TransitionParams();
            TransitionParams = actionSet;
            TransitionInterpreter animationInterpreter = new(this, actionSet)
            {
                DeltaTime = (int)DeltaTime,
            };

            if (Application.Current == null)
            {
                return;
            }
            else
            {
                TransitionParams.StartInvoke();
            }

            animationInterpreter.FrameSequence = preload ?? ComputingFrames(targetState, this);
            CurrentState = stateName;
            Interpreter = animationInterpreter;
            await animationInterpreter.Start();
        }
        internal TransitionScheduler(object viewModel, params State[] states)
        {
            TransitionApplied = viewModel;
            Type = viewModel.GetType();
            InitializeTypes(Type);
            foreach (var state in states)
            {
                States.Add(state);
            }
        }
        internal TransitionParams TransitionParams { get; set; } = new();
        internal bool IsReSet { get; set; } = false;

        private static int _maxFR = 240;
        private static List<List<Tuple<PropertyInfo, List<object?>>>> ComputingFrames(State state, TransitionScheduler machine)
        {
            List<List<Tuple<PropertyInfo, List<object?>>>> result = new(7);

            var count = (int)machine.FrameCount;
            var fc = count >= 2 ? count : 2;
            result.Add(DoubleComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(BrushComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(TransformComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(PointComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(CornerRadiusComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(ThicknessComputing(machine.Type, state, machine.TransitionApplied, fc));
            result.Add(ILinearInterpolationComputing(machine.Type, state, machine.TransitionApplied, fc));

            return result;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> DoubleComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item1.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.DoubleComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> BrushComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item2.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.BrushComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> TransformComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item3.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.TransformComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> PointComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item4.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.PointComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> CornerRadiusComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item5.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.CornerRadiusComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> ThicknessComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item6.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = propertyinfo.GetValue(TransitionApplied);
                        var newValue = state.Values[propertyname];
                        if (currentValue != newValue)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, LinearInterpolation.ThicknessComputing(currentValue, newValue, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
        private static List<Tuple<PropertyInfo, List<object?>>> ILinearInterpolationComputing(Type type, State state, object TransitionApplied, int FrameCount)
        {
            List<Tuple<PropertyInfo, List<object?>>> allFrames = new(FrameCount);
            if (SplitedPropertyInfos.TryGetValue(type, out var infodictionary))
            {
                foreach (var propertyname in state.Values.Keys)
                {
                    if (infodictionary.Item7.TryGetValue(propertyname, out var propertyinfo))
                    {
                        var currentValue = (IInterpolable?)propertyinfo.GetValue(TransitionApplied);
                        var newValue = (IInterpolable?)state.Values[propertyname];
                        if (currentValue != newValue && newValue != null)
                        {
                            allFrames.Add(Tuple.Create(propertyinfo, newValue.Interpolate(currentValue?.Self, newValue.Self, FrameCount)));
                        }
                    }
                }
            }
            return allFrames;
        }
    }
}
