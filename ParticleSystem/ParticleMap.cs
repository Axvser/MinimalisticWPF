using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.ParticleSystem
{
    public sealed class ParticleMap : IExecutableTransition
    {
        private static Random random = new();

        internal ParticleState start;
        internal ParticleState end = ParticleState.Default.Clone();

        public ParticleState StateValue { get; set; }

        public TransitionScheduler TransitionScheduler { get; private set; }

        public double Duration { get; set; } = 1;

        public bool IsActive { get; private set; } = false;

        public event Action? Started;
        public event Action? Updated;
        public event Action? Completed;

        internal ParticleMap(Point startPosition, double startSize, Pen startPen)
        {
            start = new ParticleState(startPosition, startSize, startPen);
            StateValue = start.Clone();
            TransitionScheduler = TransitionScheduler.CreateUniqueUnit(this);
        }

        public static ParticleMap From(Point position, double size, Pen pen)
        {
            return new ParticleMap(position, size, pen);
        }

        public ParticleMap To(Point position, double size, Pen pen)
        {
            end = new ParticleState(position, size, pen);
            return this;
        }

        public Task Start(object? target = null)
        {
            this.Transition()
                .SetProperty(map => map.StateValue, end)
                .SetParams((p) =>
                {
                    p.Duration = Duration;
                    p.LoopTime = int.MaxValue;
                    p.Start += () => WhileStart();
                    p.LateUpdate += () => WhileUpdated();
                    p.Completed += () => WhileCompleted();
                })
                .Start();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            TransitionScheduler.Dispose();
            WhileCompleted();
        }

        private void WhileStart()
        {
            StateValue = start.Clone();
            IsActive = true;
            Started?.Invoke();
        }
        private void WhileUpdated()
        {
            Updated?.Invoke();
        }
        private void WhileCompleted()
        {
            StateValue = ParticleState.Default.Clone();
            IsActive = false;
            Completed?.Invoke();
        }
    }
}