using MinimalisticWPF.StructuralDesign;
using MinimalisticWPF.StructuralDesign.Move;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinimalisticWPF.MoveBehavior
{
    public class PolygonMove : Canvas, IMoveMeta, IOptionalRendeTime
    {
        public PolygonMove()
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
                typeof(PolygonMove),
                new PropertyMetadata(5.0, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.TransitionParams.Duration = (double)e.NewValue;
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
                typeof(PolygonMove),
                new PropertyMetadata(Brushes.Cyan, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.DrawPen = new Pen((Brush)e.NewValue, pm.DrawThickness);
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
                typeof(PolygonMove),
                new PropertyMetadata(1.0, (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.DrawPen = new Pen(pm.DrawBrush, (double)e.NewValue);
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
                typeof(PolygonMove),
                new PropertyMetadata(new Pen(Brushes.Cyan, 1), (dp, e) =>
                {
                    if (dp is PolygonMove pm)
                    {
                        pm.InvalidateVisual();
                    }
                }));

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
                    if (dp is PolygonMove pm)
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
                        pm.Visibility = pm.AnalizeVisibility((RenderTimes)e.NewValue);
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

        public List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offset, int frameCount)
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