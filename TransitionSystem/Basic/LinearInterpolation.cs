using MinimalisticWPF.TransitionSystem.Basic.BrushTransition;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.TransitionSystem.Basic
{
    public static class LinearInterpolation
    {
        public static List<object?> DoubleComputing(object? start, object? end, int steps)
        {
            var d1 = (double)(start ?? 0);
            var d2 = (double)(end ?? d1);

            List<object?> result = new(steps);

            var delta = d2 - d1;

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (var i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;
                result.Add(d1 + t * delta);
            }
            if (steps > 1) result[0] = start;
            result[steps - 1] = end;

            return result;
        }
        public static List<object?> BrushComputing(object? start, object? end, int steps)
        {
            var startBrush = start as Brush ?? Brushes.Transparent;
            var endBrush = end as Brush ?? Brushes.Transparent;

            // 处理纯色画刷 ↔ 纯色画刷
            if (startBrush is SolidColorBrush startSolid && endBrush is SolidColorBrush endSolid)
                return InterpolateSolidColorBrush(startSolid, endSolid, steps);

            // 将纯色画刷转换为对应类型的渐变画刷，再调用结构体插值
            if (startBrush is SolidColorBrush solidStart)
                startBrush = ConvertToGradientBrush(solidStart, endBrush);
            if (endBrush is SolidColorBrush solidEnd)
                endBrush = ConvertToGradientBrush(solidEnd, startBrush);

            // 根据画刷类型调用结构体插值
            if (startBrush is LinearGradientBrush startLinear && endBrush is LinearGradientBrush endLinear)
            {
                var valStart = ValueLinearBrush.FromLinearGradientBrush(startLinear);
                var valEnd = ValueLinearBrush.FromLinearGradientBrush(endLinear);
                return valStart.Interpolate(valStart, valEnd, steps);
            }
            if (startBrush is RadialGradientBrush startRadial && endBrush is RadialGradientBrush endRadial)
            {
                var valStart = ValueRadialBrush.FromRadialGradientBrush(startRadial);
                var valEnd = ValueRadialBrush.FromRadialGradientBrush(endRadial);
                return valStart.Interpolate(valStart, valEnd, steps);
            }

            // 其他不支持的组合直接返回最终值
            return [endBrush];
        }
        public static List<object?> TransformComputing(object? start, object? end, int steps)
        {
            var matrix1 = (Transform)(start ?? Transform.Identity);
            var matrix2 = (Transform)(end ?? matrix1);

            List<object?> result = new(steps);

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (int i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;

                double m11 = matrix1.Value.M11 + t * (matrix2.Value.M11 - matrix1.Value.M11);
                double m12 = matrix1.Value.M12 + t * (matrix2.Value.M12 - matrix1.Value.M12);
                double m21 = matrix1.Value.M21 + t * (matrix2.Value.M21 - matrix1.Value.M21);
                double m22 = matrix1.Value.M22 + t * (matrix2.Value.M22 - matrix1.Value.M22);
                double offsetX = matrix1.Value.OffsetX + t * (matrix2.Value.OffsetX - matrix1.Value.OffsetX);
                double offsetY = matrix1.Value.OffsetY + t * (matrix2.Value.OffsetY - matrix1.Value.OffsetY);

                var interpolatedMatrixStr = $"{m11},{m12},{m21},{m22},{offsetX},{offsetY}";
                var transform = Transform.Parse(interpolatedMatrixStr);
                result.Add(transform);
            }
            if (steps > 1) result[0] = start;
            result[steps - 1] = end;

            return result;
        }
        public static List<object?> PointComputing(object? start, object? end, int steps)
        {
            var point1 = (Point)(start ?? new Point(0, 0));
            var point2 = (Point)(end ?? point1);

            List<object?> result = new(steps);

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (var i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;
                var x = point1.X + t * (point2.X - point1.X);
                var y = point1.Y + t * (point2.Y - point1.Y);
                result.Add(new Point(x, y));
            }
            if (steps > 1) result[0] = start;
            result[steps - 1] = end;

            return result;
        }
        public static List<object?> ThicknessComputing(object? start, object? end, int steps)
        {
            var thickness1 = (Thickness)(start ?? new Thickness(0));
            var thickness2 = (Thickness)(end ?? thickness1);

            List<object?> result = new(steps);

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (var i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;
                var left = thickness1.Left + t * (thickness2.Left - thickness1.Left);
                var top = thickness1.Top + t * (thickness2.Top - thickness1.Top);
                var right = thickness1.Right + t * (thickness2.Right - thickness1.Right);
                var bottom = thickness1.Bottom + t * (thickness2.Bottom - thickness1.Bottom);
                result.Add(new Thickness(left, top, right, bottom));
            }
            if (steps > 1) result[0] = start;
            result[steps - 1] = end;

            return result;
        }
        public static List<object?> CornerRadiusComputing(object? start, object? end, int steps)
        {
            var radius1 = (CornerRadius)(start ?? new CornerRadius(0));
            var radius2 = (CornerRadius)(end ?? radius1);

            List<object?> result = new(steps);

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (var i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;
                var topLeft = radius1.TopLeft + t * (radius2.TopLeft - radius1.TopLeft);
                var topRight = radius1.TopRight + t * (radius2.TopRight - radius1.TopRight);
                var bottomLeft = radius1.BottomLeft + t * (radius2.BottomLeft - radius1.BottomLeft);
                var bottomRight = radius1.BottomRight + t * (radius2.BottomRight - radius1.BottomRight);
                result.Add(new CornerRadius(topLeft, topRight, bottomRight, bottomLeft));
            }
            if (steps > 1) result[0] = start;
            result[steps - 1] = end;

            return result;
        }

        private static List<object?> InterpolateSolidColorBrush(SolidColorBrush start, SolidColorBrush end, int steps)
        {
            var rgb1 = RGB.FromBrush(start);
            var rgb2 = RGB.FromBrush(end);
            return [.. rgb1.Interpolate(rgb1, rgb2, steps).Select(rgb => (object?)(((rgb as RGB) ?? RGB.Empty)).Brush)];
        }
        private static Brush ConvertToGradientBrush(SolidColorBrush solid, Brush target)
        {
            return target switch
            {
                LinearGradientBrush linear => new LinearGradientBrush(
                    [new GradientStop(solid.Color, 0), new GradientStop(solid.Color, 1)],
                    linear.StartPoint,
                    linear.EndPoint
                ),
                RadialGradientBrush radial => new RadialGradientBrush
                {
                    GradientStops = [new GradientStop(solid.Color, 0), new GradientStop(solid.Color, 1)],
                    GradientOrigin = radial.GradientOrigin,
                    Center = radial.Center,
                    RadiusX = radial.RadiusX,
                    RadiusY = radial.RadiusY
                },
                _ => solid // 默认不转换
            };
        }
    }
}
