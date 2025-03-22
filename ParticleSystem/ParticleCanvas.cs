using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalisticWPF.ParticleSystem
{
    public class ParticleCanvas : Canvas
    {
        private readonly List<ParticleMap> _activeParticles = new();
        private readonly Random _random = new();
        private DrawingVisual _visual;
        private bool _isRendering;
        private Shape? _terminator;
        private List<Shape> _emitters = new();

        public ParticleCanvas()
        {
            Loaded += OnLoaded;

            // 启用硬件加速
            CacheMode = new BitmapCache();
            _visual = new DrawingVisual();
            AddVisualChild(_visual);
        }

        protected override void OnVisualChildrenChanged(DependencyObject added, DependencyObject removed)
        {
            base.OnVisualChildrenChanged(added, removed);
            InitializeBoundaries();
        }

        private void InitializeBoundaries()
        {
            var shapes = Children.OfType<Shape>().ToList();
            if (shapes.Count < 2) return;
            _terminator = shapes.Last();
            _emitters = shapes.Take(shapes.Count - 1).ToList();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(StartRendering), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void StartRendering()
        {
            if (_isRendering || _terminator == null || _emitters.Count == 0) return;

            // 初始化固定数量的粒子
            for (int i = 0; i < 100; i++)
            {
                var particle = CreateNewParticle();
                _activeParticles.Add(particle);
            }

            CompositionTarget.Rendering += OnRenderingFrame;
            _isRendering = true;
        }

        private ParticleMap CreateNewParticle()
        {
            var emitter = _emitters[_random.Next(_emitters.Count)];
            var startPoint = new ShapeEdgeSampler(emitter, this).GetRandomPointOnEdge(_random);
            var endPoint = new ShapeEdgeSampler(_terminator, this).GetRandomPointOnEdge(_random);

            var particle = ParticleMap.From(startPoint, 1, new Pen(Brushes.Cyan, 1))
                .To(endPoint, 1, new Pen(Brushes.Violet, 1));

            particle.Duration = _random.NextDouble() * 2 + 1;
            particle.Completed += () => ResetParticle(particle);
            particle.Start();

            return particle;
        }

        private void ResetParticle(ParticleMap particle)
        {
            var emitter = _emitters[_random.Next(_emitters.Count)];
            var startPoint = new ShapeEdgeSampler(emitter, this).GetRandomPointOnEdge(_random);
            var endPoint = new ShapeEdgeSampler(_terminator, this).GetRandomPointOnEdge(_random);

            particle.To(endPoint, 1, new Pen(Brushes.Violet, 1));
            particle.Duration = _random.NextDouble() * 2 + 1;
            particle.Start();
        }

        private void OnRenderingFrame(object? sender, EventArgs e)
        {
            RenderParticles();
        }

        private void RenderParticles()
        {
            using (var dc = _visual.RenderOpen())
            {
                foreach (var particle in _activeParticles.Where(p => p.IsActive))
                {
                    var state = particle.StateValue;
                    dc.DrawEllipse(state.Pen.Brush, null, state.Position, state.Size, state.Size);
                }
            }
        }

        // 重写视觉树方法以支持 DrawingVisual
        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException(nameof(index));
            return _visual;
        }
    }
}