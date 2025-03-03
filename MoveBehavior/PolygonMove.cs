using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinimalisticWPF.MoveBehavior
{
    public class PolygonMove : Canvas, IMoveMeta, IExecutableMove
    {
        public PolygonMove()
        {
            Visibility = (RenderTime, DesignerProperties.GetIsInDesignMode(this)) switch
            {
                (RenderTimes.RunTime, false) => Visibility.Visible,
                (RenderTimes.DesignTime, true) => Visibility.Visible,
                (RenderTimes.AnyTime, _) => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            foreach(Anchor anchor in Children)
            {
                anchor.Foreground = AnchorBrush;
            }
        }

        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register
            ("IsClosed",
                typeof(bool),
                typeof(PolygonMove),
                new PropertyMetadata(false, (dp, e) =>
                {
                    if(dp is PolygonMove pm)
                    {
                        pm.InvalidateVisual();
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
                typeof(PolygonMove),
                new PropertyMetadata(RenderTimes.DesignTime, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.Visibility = ((RenderTimes)e.NewValue, DesignerProperties.GetIsInDesignMode(pm)) switch
                        {
                            (RenderTimes.RunTime, false) => Visibility.Visible,
                            (RenderTimes.DesignTime, true) => Visibility.Visible,
                            (RenderTimes.AnyTime, _) => Visibility.Visible,
                            _ => Visibility.Collapsed,
                        };
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
                typeof(PolygonMove),
                new PropertyMetadata(Brushes.White, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        foreach (Anchor child in pm.Children)
                        {
                            child.Foreground = (Brush)e.NewValue;
                        }
                        pm.InvalidateVisual();
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
                typeof(PolygonMove),
                new PropertyMetadata(0.0, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        foreach (Anchor child in pm.Children)
                        {
                            child.Width = (double)e.NewValue;
                            child.Height = child.Width;
                        }
                        pm.InvalidateVisual();
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
                typeof(PolygonMove),
                new PropertyMetadata(99, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.InvalidateVisual();
                    }
                }));

        public List<Point> Anchors => GetControlPoints();

        public TransitionParams TransitionParams { get; set; } = new()
        {
            FrameRate = 100,
            Duration = 5
        };

        public void Start(FrameworkElement target)
        {
            var offset = new Point(target.ActualWidth / 2, target.ActualHeight / 2);
            var scheduler = TransitionScheduler.CreateUniqueUnit(target);
            var state = new State() { StateName = "polygonmovestate" };
            state.AddProperty(MoveBehaviorExtension.RenderTransformPropertyInfo.Name, null);
            scheduler.States.Add(state);
            scheduler.TransitionParams = TransitionParams;
            scheduler.InterpreterScheduler(state.StateName, TransitionParams, GetNormalFrames(offset, (int)scheduler.FrameCount));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            DrawPolygon(dc);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // 处理新增控件
                if (visualAdded is Anchor addedPoint)
                {
                    addedPoint.Foreground = AnchorBrush;
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

        private void DrawPolygon(DrawingContext dc)
        {
            // 获取所有锚点坐标
            var points = GetControlPoints();
            if (points.Count < 2) return;

            // 创建路径几何
            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = points[0] };
            foreach (var point in points.Skip(1))
            {
                figure.Segments.Add(new LineSegment(point, true));
            }
            figure.IsClosed = IsClosed;
            geometry.Figures.Add(figure);

            // 绘制多边形
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

        private List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offset, int frameCount)
        {
            if (frameCount < 2) frameCount = 2;
            List<List<Tuple<PropertyInfo, List<object?>>>> result = [[]];

            // 计算路径上的点
            var points = GetControlPoints();
            if (points.Count < 2) return result;

            // 计算每一帧的位置
            List<object?> frames = new List<object?>();
            for (int i = 0; i < frameCount; i++)
            {
                double t = (double)i / (frameCount - 1);
                int segmentIndex = (int)(t * (points.Count - 1));
                double segmentT = t * (points.Count - 1) - segmentIndex;

                Point p1 = points[segmentIndex];
                Point p2 = points[(segmentIndex + 1) % points.Count];
                Point interpolatedPoint = new Point(
                    p1.X + segmentT * (p2.X - p1.X),
                    p1.Y + segmentT * (p2.Y - p1.Y)
                );

                frames.Add(new TranslateTransform(interpolatedPoint.X - offset.X, interpolatedPoint.Y - offset.Y));
            }

            result[0].Add(Tuple.Create<PropertyInfo, List<object?>>(MoveBehaviorExtension.RenderTransformPropertyInfo, frames));
            return result;
        }
    }
}