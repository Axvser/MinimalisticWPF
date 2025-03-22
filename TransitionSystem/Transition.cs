﻿using MinimalisticWPF.StructuralDesign.Animator;
using System.Reflection;

namespace MinimalisticWPF.TransitionSystem
{
    public delegate List<object?> InterpolationHandler(object? start, object? end, int steps);

    public static class Transition
    {
        private static string _tempName = "temp";
        public static string TempName
        {
            get => _tempName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _tempName = value;
                }
            }
        }

        public static TransitionBoard<T> Create<T>(T? target = null) where T : class
        {
            return new TransitionBoard<T>() { TransitionApplied = target };
        }
        public static TransitionBoard<T> Create<T>(ICollection<T> values, object? target = null) where T : class, ITransitionMeta
        {
            var meta = new TransitionBoard<T>()
            {
                TransitionApplied = target,
            };
            meta.Merge(values.Select(v => v as ITransitionMeta).ToArray());
            return meta;
        }
        public static TransitionBoard<T> Create<T>(ICollection<T> values, TransitionParams transitionParams, object? target = null) where T : class, ITransitionMeta
        {
            var meta = new TransitionBoard<T>()
            {
                TransitionApplied = target,
                TransitionParams = transitionParams
            };
            meta.Merge(values.Select(v => v as ITransitionMeta).ToArray());
            return meta;
        }
        public static TransitionBoard<T> Create<T>(ICollection<T> values, Action<TransitionParams> transitionSet, object? target = null) where T : class, ITransitionMeta
        {
            var para = new TransitionParams();
            transitionSet(para);
            var meta = new TransitionBoard<T>()
            {
                TransitionApplied = target,
                TransitionParams = para
            };
            meta.Merge(values.Select(v => v as ITransitionMeta).ToArray());
            return meta;
        }

        public static IExecutableTransition Compile<T>(ICollection<T> values, TransitionParams transitionParams, object? target = null) where T : class, ITransitionMeta
        {
            var meta = new TransitionBoard<T>()
            {
                TransitionApplied = target,
                TransitionParams = transitionParams.DeepCopy()
            };
            meta.Merge(values.Select(v => v as ITransitionMeta).ToArray());
            return meta;
        }
        public static IExecutableTransition Compile<T>(ICollection<T> values, Action<TransitionParams> transitionSet, object? target = null) where T : class, ITransitionMeta
        {
            var para = new TransitionParams();
            transitionSet(para);
            var meta = new TransitionBoard<T>()
            {
                TransitionApplied = target,
                TransitionParams = para
            };
            meta.Merge(values.Select(v => v as ITransitionMeta).ToArray());
            return meta;
        }

        public static void DisposeAll()
        {
            foreach (var machinedic in TransitionScheduler.MachinePool.Values)
            {
                foreach (var machine in machinedic.Values)
                {
                    machine.Interpreter?.Stop();
                }
            }
        }
        public static void Dispose(params object[] targets)
        {
            foreach (var target in targets)
            {
                var machine = TransitionScheduler.CreateUniqueUnit(target);
                machine.Interpreter?.Stop();
            }
        }
    }
}
