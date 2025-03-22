﻿using System.Reflection;
using System.Collections.Concurrent;
using MinimalisticWPF.StructuralDesign.Animator;

namespace MinimalisticWPF.TransitionSystem.Basic
{
    public sealed class State : ITransitionMeta, ICloneable
    {
        internal State() { }

        public string StateName { get; internal set; } = string.Empty;
        public ConcurrentDictionary<string, object?> Values { get; internal set; } = new();
        public ConcurrentDictionary<string, InterpolationHandler> Calculators { get; internal set; } = new();
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
        public void Add(string propertyName, object? value)
        {
            AddProperty(propertyName, value);
            Calculators.TryRemove(propertyName, out _);
        }
        public void Add(string propertyName, object? value, InterpolationHandler calculator)
        {
            AddProperty(propertyName, value);
            AddCalculator(propertyName, calculator);
        }
        public void AddCalculator(string propertyName, InterpolationHandler value)
        {
            if (Calculators.TryGetValue(propertyName, out var ori))
            {
                Calculators.TryUpdate(propertyName, value, ori);
            }
            else
            {
                Calculators.TryAdd(propertyName, value);
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
            foreach (var calculator in meta.PropertyState.Calculators)
            {
                AddCalculator(calculator.Key, calculator.Value);
            }
            return this;
        }

        internal State DeepCopy()
        {
            var newState = new State
            {
                StateName = StateName,
                TransitionParams = TransitionParams.DeepCopy(),
            };

            foreach (var kvp in Values)
            {
                newState.Values[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in Calculators)
            {
                newState.Calculators[kvp.Key] = kvp.Value;
            }

            return newState;
        }

        public object Clone()
        {
            return DeepCopy();
        }
    }
}
