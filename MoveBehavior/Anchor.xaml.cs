using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimalisticWPF.MoveBehavior
{
    public partial class Anchor : Thumb
    {
        public Anchor()
        {
            InitializeComponent();
        }

        public Point CoordinatePoint
        {
            get { return (Point)GetValue(CoordinatePointProperty); }
            private set { SetValue(CoordinatePointProperty, value); }
        }
        public static readonly DependencyProperty CoordinatePointProperty =
        DependencyProperty.Register(
            "CoordinatePoint",
            typeof(Point),
            typeof(Anchor),
            new FrameworkPropertyMetadata(
                default(Point),
                FrameworkPropertyMetadataOptions.AffectsParentArrange,
                OnCoordinatePointChanged));

        private static void OnCoordinatePointChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            // 通知父画布重绘
            if (d is Anchor anchor && anchor.Parent is Canvas canvas)
            {
                canvas.InvalidateVisual();
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Parent is Canvas canvas)
            {
                // 更新坐标
                Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
                Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);

                // 计算实际坐标
                CoordinatePoint = TransformToAncestor(canvas)
                    .Transform(new Point(Width / 2, Height / 2));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // 设计时响应布局属性变化
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (e.Property == Canvas.LeftProperty ||
                    e.Property == Canvas.TopProperty ||
                    e.Property == WidthProperty ||
                    e.Property == HeightProperty)
                {
                    // 通知父画布更新
                    if (Parent is Canvas canvas)
                    {
                        canvas.InvalidateVisual();
                    }
                }
            }
        }
    }
}
