using MinimalisticWPF.MoveBehavior;
using MinimalisticWPF.StructuralDesign;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalisticWPF.ParticleSystem
{
    public sealed class ParticleCanvas : Canvas, IOptionalRendeTime
    {
        private ParticleMapsBaker? Baker;

        public RenderTimes RenderTime
        {
            get { return (RenderTimes)GetValue(RenderTimeProperty); }
            set { SetValue(RenderTimeProperty, value); }
        }

        public bool CanShapeRendered
        {
            get { return (bool)GetValue(CanShapeRenderedProperty); }
            set { SetValue(CanShapeRenderedProperty, value); }
        }

        public int Capacity
        {
            get { return (int)GetValue(CapacityProperty); }
            set { SetValue(CapacityProperty, value); }
        }

        public double DurationObfuscationConstant
        {
            get { return (double)GetValue(DurationObfuscationConstantProperty); }
            set { SetValue(DurationObfuscationConstantProperty, value); }
        }

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public Brush StartBrush
        {
            get { return (Brush)GetValue(StartBrushProperty); }
            set { SetValue(StartBrushProperty, value); }
        }

        public Brush EndBrush
        {
            get { return (Brush)GetValue(EndBrushProperty); }
            set { SetValue(EndBrushProperty, value); }
        }

        public double StartSize
        {
            get { return (double)GetValue(StartSizeProperty); }
            set { SetValue(StartSizeProperty, value); }
        }

        public double EndSize
        {
            get { return (double)GetValue(EndSizeProperty); }
            set { SetValue(EndSizeProperty, value); }
        }

        public static readonly DependencyProperty EndSizeProperty =
            DependencyProperty.Register("EndSize", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty StartSizeProperty =
            DependencyProperty.Register("StartSize", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty EndBrushProperty =
            DependencyProperty.Register("EndBrush", typeof(Brush), typeof(ParticleCanvas), new PropertyMetadata(Brushes.Violet, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty StartBrushProperty =
            DependencyProperty.Register("StartBrush", typeof(Brush), typeof(ParticleCanvas), new PropertyMetadata(Brushes.Cyan, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty DurationObfuscationConstantProperty =
            DependencyProperty.Register("DurationObfuscationConstant", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty CapacityProperty =
            DependencyProperty.Register("Capacity", typeof(int), typeof(ParticleCanvas), new PropertyMetadata(100, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    ParticleCanvas_Loaded(canvas, new());
                }
            }));

        public static readonly DependencyProperty CanShapeRenderedProperty =
            DependencyProperty.Register("CanShapeRendered", typeof(bool), typeof(ParticleCanvas), new PropertyMetadata(true, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    var state = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
                    foreach (Shape shape in canvas.Children)
                    {
                        shape.Visibility = state;
                    }
                }
            }));

        public static readonly DependencyProperty RenderTimeProperty =
            DependencyProperty.Register("RenderTime", typeof(RenderTimes), typeof(ParticleCanvas), new PropertyMetadata(RenderTimes.AnyTime, (dp, e) =>
            {
                if (dp is ParticleCanvas canvas)
                {
                    var state = canvas.AnalizeVisibility();
                    if (state == Visibility.Visible)
                    {
                        canvas.Baker?.UpdateMaps();
                    }
                    else
                    {
                        canvas?.Baker?.Clear();
                    }
                }
            }));

        public ParticleCanvas()
        {
            CompositionTarget.Rendering += (sender, e) =>
            {
                InvalidateVisual();
            };
            Loaded += ParticleCanvas_Loaded;
        }

        private static void ParticleCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ParticleCanvas canvas)
            {
                canvas.Baker?.Clear();
                canvas.Baker = new(canvas);
                canvas.Baker?.UpdateSamplers();
                canvas.Baker?.UpdateMaps();
            }
        }

        protected override async void OnVisualChildrenChanged(DependencyObject added, DependencyObject removed)
        {
            base.OnVisualChildrenChanged(added, removed);
            if (added is Shape)
            {
                if (Baker is not null)
                {
                    Baker.UpdateSamplers();
                    await Baker.UpdateMaps();
                }
            }
            if (removed is Shape)
            {
                if (Baker is not null)
                {
                    Baker.UpdateSamplers();
                    await Baker.UpdateMaps();
                }
            }
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Baker is null || !Baker.IsActived) return;

            foreach (var context in Baker.ReadSource())
            {
                dc.DrawEllipse(Baker.IsVisible ? context.Item2.Brush : Brushes.Transparent, null, context.Item1, context.Item2.Thickness, context.Item2.Thickness);
            }
        }
    }
}