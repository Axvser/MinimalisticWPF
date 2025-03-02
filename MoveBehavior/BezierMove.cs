using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MinimalisticWPF.MoveBehavior
{
    public class BezierMove() : Canvas, ITractionMeta, IExecutableTraction
    {
        private static PropertyInfo _renderTransformPropertyInfo = typeof(UIElement).GetProperty("RenderTransform") ?? throw new ArgumentNullException("Can not get RenderTransform PropertyInfo");

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
                new PropertyMetadata(20, (dp, e) =>
                {
                    if (dp is BezierMove bm)
                    {
                        bm.InvalidateVisual();
                    }
                }));

        public List<Point> Anchors => BezierCurve.Generate(Accuracy, GetControlPoints());

        public TransitionParams TransitionParams { get; set; } = TransitionParams.Tractive;

        public void Tractive(FrameworkElement target)
        {
            var offest = new Point(target.ActualWidth / 2, target.ActualHeight / 2);
            var scheduler = TransitionScheduler.CreateUniqueUnit(target);
            var state = new State() { StateName = "beziermovestate" };
            state.AddProperty(_renderTransformPropertyInfo.Name, null);
            scheduler.States.Add(state);
            scheduler.TransitionParams = TransitionParams;
            scheduler.InterpreterScheduler(state.StateName, TransitionParams, GetNormalFrames(offest, (int)scheduler.FrameCount));
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

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // 处理新增控件
                if (visualAdded is Anchor addedPoint)
                {
                    HookPositionChanges(addedPoint);
                }

                // 处理移除控件
                if (visualRemoved is Anchor removedPoint)
                {
                    UnhookPositionChanges(removedPoint);
                }
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
            var pen = new Pen(Brushes.Cyan, 1);
            dc.DrawGeometry(null, pen, geometry);
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

        private List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offest, int framecount)
        {
            if (framecount < 2) framecount = 2;
            List<List<Tuple<PropertyInfo, List<object?>>>> result = [[]];

            Accuracy = framecount;

            List<object?> frames = Anchors.Select(a => (object?)(new TranslateTransform(a.X - offest.X, a.Y - offest.Y))).ToList();
            result[0].Add(Tuple.Create<PropertyInfo, List<object?>>(_renderTransformPropertyInfo, frames));

            return result;
        }
    }
}