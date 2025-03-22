using MinimalisticWPF.StructuralDesign;
using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MinimalisticWPF.MoveBehavior
{
    public class BezierMove : Canvas, IMoveMeta, IOptionalRendeTime
    {
        public BezierMove()
        {
            Visibility = this.AnalizeVisibility();
        }

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register
            ("Duration",
                typeof(double),
                typeof(BezierMove),
                new PropertyMetadata(5.0, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.TransitionParams.Duration = (double)e.NewValue;
                    }
                }));

        public Brush DrawBrush
        {
            get { return (Brush)GetValue(DrawBrushProperty); }
            set { SetValue(DrawBrushProperty, value); }
        }
        public static readonly DependencyProperty DrawBrushProperty =
            DependencyProperty.Register
            ("DrawBrush",
                typeof(Brush),
                typeof(BezierMove),
                new PropertyMetadata(Brushes.Cyan, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.DrawPen = new Pen((Brush)e.NewValue, bm.DrawThickness);
                    }
                }));

        public double DrawThickness
        {
            get { return (double)GetValue(DrawThicknessProperty); }
            set { SetValue(DrawThicknessProperty, value); }
        }
        public static readonly DependencyProperty DrawThicknessProperty =
            DependencyProperty.Register
            ("DrawThickness",
                typeof(double),
                typeof(BezierMove),
                new PropertyMetadata(1.0, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.DrawPen = new Pen(bm.DrawBrush, (double)e.NewValue);
                    }
                }));

        public Pen DrawPen
        {
            get { return (Pen)GetValue(DrawPenProperty); }
            internal set { SetValue(DrawPenProperty, value); }
        }
        public static readonly DependencyProperty DrawPenProperty =
            DependencyProperty.Register
            ("DrawPen",
                typeof(Pen),
                typeof(BezierMove),
                new PropertyMetadata(new Pen(Brushes.Cyan, 1), (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.InvalidateVisual();
                    }
                }));

        public RenderTimes RenderTime
        {
            get { return (RenderTimes)GetValue(RenderTimeProperty); }
            set { SetValue(RenderTimeProperty, value); }
        }
        public static readonly DependencyProperty RenderTimeProperty =
            DependencyProperty.Register
            ("RenderTime",
                typeof(RenderTimes),
                typeof(BezierMove),
                new PropertyMetadata(RenderTimes.DesignTime, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.Visibility = bm.AnalizeVisibility((RenderTimes)e.NewValue);
                    }
                }));

        public Brush AnchorBrush
        {
            get { return (Brush)GetValue(AnchorBrushProperty); }
            set { SetValue(AnchorBrushProperty, value); }
        }
        public static readonly DependencyProperty AnchorBrushProperty =
            DependencyProperty.Register
            ("AnchorBrush",
                typeof(Brush),
                typeof(BezierMove),
                new PropertyMetadata(Brushes.White, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        foreach (Anchor child in bm.Children)
                        {
                            child.Foreground = (Brush)e.NewValue;
                        }
                        bm.InvalidateVisual();
                    }
                }));

        public double AnchorSize
        {
            get { return (double)GetValue(AnchorSizeProperty); }
            set { SetValue(AnchorSizeProperty, value); }
        }
        public static readonly DependencyProperty AnchorSizeProperty =
            DependencyProperty.Register
            ("AnchorSize",
                typeof(double),
                typeof(BezierMove),
                new PropertyMetadata(0.0, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        foreach (Anchor child in bm.Children)
                        {
                            child.Width = (double)e.NewValue;
                            child.Height = child.Width;
                        }
                        bm.InvalidateVisual();
                    }
                }));

        public int Accuracy
        {
            get { return (int)GetValue(AccuracyProperty); }
            set { SetValue(AccuracyProperty, value); }
        }
        public static readonly DependencyProperty AccuracyProperty =
            DependencyProperty.Register
            ("Accuracy",
                typeof(int),
                typeof(BezierMove),
                new PropertyMetadata(99, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.InvalidateVisual();
                    }
                }));

        public List<Point> Anchors => BezierCurve.Generate(Accuracy, GetControlPoints());

        public TransitionParams TransitionParams { get; set; } = new()
        {
            FrameRate = 100,
            Duration = 5
        };

        public List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offest, int framecount)
        {
            if (framecount < 2) framecount = 2;
            List<List<Tuple<PropertyInfo, List<object?>>>> result = [[]];

            Accuracy = framecount - 1;

            List<object?> frames = Anchors.Select(a => (object?)(new TranslateTransform(a.X - offest.X, a.Y - offest.Y))).ToList();
            result[0].Add(Tuple.Create<PropertyInfo, List<object?>>(MoveBehaviorExtension.RenderTransformPropertyInfo, frames));

            return result;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            DrawBezierCurve(dc);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualAdded is Anchor addedPoint)
            {
                addedPoint.Foreground = AnchorBrush;
                HookPositionChanges(addedPoint);
            }

            if (visualRemoved is Anchor removedPoint)
            {
                UnhookPositionChanges(removedPoint);
            }

            InvalidateVisual();
        }

        private void DrawBezierCurve(DrawingContext dc)
        {
            // 获取所有锚点坐标（设计时/运行时兼容）
            var points = GetControlPoints();
            if (points.Count < 2) return;

            // 生成贝塞尔曲线
            var curve = BezierCurve.Generate(Accuracy, points);

            // 创建路径几何
            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = curve[0] };
            foreach (var point in curve.Skip(1))
            {
                figure.Segments.Add(new LineSegment(point, true));
            }
            geometry.Figures.Add(figure);

            // 绘制曲线
            dc.DrawGeometry(null, DrawPen, geometry);
        }

        private List<Point> GetControlPoints()
        {
            var points = new List<Point>();

            foreach (var child in Children.OfType<Anchor>())
            {
                // 强制更新布局确保获取最新坐标
                child.UpdateLayout();

                var left = GetLeft(child);
                var top = GetTop(child);
                points.Add(new Point(
                    double.IsNaN(left) ? 0 : left + child.Width / 2,
                    double.IsNaN(top) ? 0 : top + child.Height / 2
                ));
            }
            return points;
        }

        private void HookPositionChanges(Anchor point)
        {
            // 监听Left属性变化
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(
                Canvas.LeftProperty,
                typeof(Anchor)
            );
            leftDescriptor.AddValueChanged(point, OnPositionChanged);

            // 监听Top属性变化
            var topDescriptor = DependencyPropertyDescriptor.FromProperty(
                Canvas.TopProperty,
                typeof(Anchor)
            );
            topDescriptor.AddValueChanged(point, OnPositionChanged);
        }

        private void UnhookPositionChanges(Anchor point)
        {
            var leftDescriptor = DependencyPropertyDescriptor.FromProperty(
                Canvas.LeftProperty,
                typeof(Anchor)
            );
            leftDescriptor.RemoveValueChanged(point, OnPositionChanged);

            var topDescriptor = DependencyPropertyDescriptor.FromProperty(
                Canvas.TopProperty,
                typeof(Anchor)
            );
            topDescriptor.RemoveValueChanged(point, OnPositionChanged);
        }

        private void OnPositionChanged(object? sender, EventArgs e)
        {
            // 强制重绘画布
            InvalidateVisual();
        }
    }
}