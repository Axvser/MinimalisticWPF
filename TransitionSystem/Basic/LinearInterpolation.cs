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

            // Solid ↔ Solid
            if (startBrush is SolidColorBrush startSolid && endBrush is SolidColorBrush endSolid)
                return InterpolateSolidColorBrush(startSolid, endSolid, steps);

            // Solid ↔ Linear
            else if (startBrush is SolidColorBrush solidStart && endBrush is LinearGradientBrush linearEnd)
                return InterpolateLinearGradientBrush(ConvertToLinearGradientBrush(solidStart, linearEnd), linearEnd, steps);
            else if (startBrush is LinearGradientBrush linearStart && endBrush is SolidColorBrush solidEnd)
                return InterpolateLinearGradientBrush(linearStart, ConvertToLinearGradientBrush(solidEnd, linearStart), steps, solidEnd);

            // Solid ↔ Radial
            else if (startBrush is SolidColorBrush solidStartRadial && endBrush is RadialGradientBrush radialEnd)
                return InterpolateRadialGradientBrush(ConvertToRadialGradientBrush(solidStartRadial, radialEnd), radialEnd, steps);
            else if (startBrush is RadialGradientBrush radialStart && endBrush is SolidColorBrush solidEndRadial)
                return InterpolateRadialGradientBrush(radialStart, ConvertToRadialGradientBrush(solidEndRadial, radialStart), steps, solidEndRadial);

            // Linear ↔ Linear
            else if (startBrush is LinearGradientBrush startLinear && endBrush is LinearGradientBrush endLinear)
                return InterpolateLinearGradientBrush(startLinear, endLinear, steps);

            // Radial ↔ Radial
            else if (startBrush is RadialGradientBrush startRadial && endBrush is RadialGradientBrush endRadial)
                return InterpolateRadialGradientBrush(startRadial, endRadial, steps);

            // 其他情况不处理过程,直接返回最终值
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
        private static LinearGradientBrush ConvertToLinearGradientBrush(SolidColorBrush solid, LinearGradientBrush template)
        {
            return new LinearGradientBrush(
                [.. template.GradientStops.Select(gs =>
                    new GradientStop(solid.Color, gs.Offset))],
                template.StartPoint,
                template.EndPoint)
            {
                SpreadMethod = template.SpreadMethod,
                MappingMode = template.MappingMode,
                ColorInterpolationMode = template.ColorInterpolationMode
            };
        }
        private static RadialGradientBrush ConvertToRadialGradientBrush(SolidColorBrush solid, RadialGradientBrush template)
        {
            return new RadialGradientBrush
            {
                // 复制模板的几何属性
                GradientOrigin = template.GradientOrigin,
                Center = template.Center,
                RadiusX = template.RadiusX,
                RadiusY = template.RadiusY,

                // 创建单色渐变集合
                GradientStops = [.. template.GradientStops.Select(gs =>
                        new GradientStop(solid.Color, gs.Offset)
                    )],

                // 复制其他画刷属性
                SpreadMethod = template.SpreadMethod,
                MappingMode = template.MappingMode,
                ColorInterpolationMode = template.ColorInterpolationMode
            };
        }
        private static List<object?> InterpolateLinearGradientBrush(LinearGradientBrush start, LinearGradientBrush end, int steps, SolidColorBrush? solidColorBrush = null)
        {
            // 统一两个画刷的渐变点位
            var (normalizedStart, normalizedEnd) = NormalizeGradientStops(start, end);

            List<object?> result = new(steps);
            if (steps == 0) return result;

            for (int i = 0; i < steps; i++)
            {
                double t = (double)(i + 1) / steps;

                var newBrush = new LinearGradientBrush
                {
                    StartPoint = InterpolatePoint(normalizedStart.StartPoint, normalizedEnd.StartPoint, t),
                    EndPoint = InterpolatePoint(normalizedStart.EndPoint, normalizedEnd.EndPoint, t),
                    GradientStops = InterpolateStops(normalizedStart.GradientStops, normalizedEnd.GradientStops, t),
                    SpreadMethod = t >= 1 ? normalizedEnd.SpreadMethod : normalizedStart.SpreadMethod,
                    MappingMode = normalizedStart.MappingMode,
                    ColorInterpolationMode = normalizedStart.ColorInterpolationMode
                };

                result.Add(newBrush);
            }

            if (steps > 1)
            {
                result[0] = start;
                result[result.Count - 1] = solidColorBrush is null ? end : solidColorBrush;
            }
            return result;
        }
        private static List<object?> InterpolateRadialGradientBrush(RadialGradientBrush start, RadialGradientBrush end, int steps, SolidColorBrush? solidColorBrush = null)
        {
            // 统一两个画刷的渐变点位
            var (normalizedStart, normalizedEnd) = NormalizeRadialGradientStops(start, end);

            List<object?> result = new(steps);
            if (steps == 0) return result;

            for (int i = 0; i < steps; i++)
            {
                double t = (double)(i + 1) / steps;

                var newBrush = new RadialGradientBrush
                {
                    GradientOrigin = InterpolatePoint(normalizedStart.GradientOrigin, normalizedEnd.GradientOrigin, t),
                    Center = InterpolatePoint(normalizedStart.Center, normalizedEnd.Center, t),
                    RadiusX = normalizedStart.RadiusX + t * (normalizedEnd.RadiusX - normalizedStart.RadiusX),
                    RadiusY = normalizedStart.RadiusY + t * (normalizedEnd.RadiusY - normalizedStart.RadiusY),
                    GradientStops = InterpolateStops(normalizedStart.GradientStops, normalizedEnd.GradientStops, t),
                    SpreadMethod = t >= 1 ? normalizedEnd.SpreadMethod : normalizedStart.SpreadMethod,
                    MappingMode = normalizedStart.MappingMode,
                    ColorInterpolationMode = normalizedStart.ColorInterpolationMode
                };

                result.Add(newBrush);
            }

            if (steps > 1)
            {
                result[0] = start;
                result[result.Count - 1] = solidColorBrush is null ? end : solidColorBrush;
            }
            return result;
        }

        private static (RadialGradientBrush, RadialGradientBrush) NormalizeRadialGradientStops(RadialGradientBrush a, RadialGradientBrush b)
        {
            if (a.GradientStops.Count == b.GradientStops.Count)
                return (a, b);

            int maxCount = Math.Max(a.GradientStops.Count, b.GradientStops.Count);

            RadialGradientBrush normalizedA = a.GradientStops.Count < maxCount ? ExpandRadialGradientStops(a, maxCount) : a;
            RadialGradientBrush normalizedB = b.GradientStops.Count < maxCount ? ExpandRadialGradientStops(b, maxCount) : b;

            return (normalizedA, normalizedB);
        }
        private static RadialGradientBrush ExpandRadialGradientStops(RadialGradientBrush original, int targetCount)
        {
            if (original.GradientStops.Count >= targetCount)
                return original;

            List<GradientStop> newStops = new();
            var orderedStops = original.GradientStops.OrderBy(s => s.Offset).ToList();

            for (int i = 0; i < targetCount; i++)
            {
                double offset = (double)i / (targetCount - 1);
                Color color = GetColorAtOffset(orderedStops, offset);
                newStops.Add(new GradientStop(color, offset));
            }

            return new RadialGradientBrush
            {
                GradientOrigin = original.GradientOrigin,
                Center = original.Center,
                RadiusX = original.RadiusX,
                RadiusY = original.RadiusY,
                GradientStops = [.. newStops],
                SpreadMethod = original.SpreadMethod,
                MappingMode = original.MappingMode,
                ColorInterpolationMode = original.ColorInterpolationMode
            };
        }
        private static Color GetColorAtOffset(List<GradientStop> stops, double offset)
        {
            if (stops.Count == 0) return Colors.Transparent;
            if (stops.Count == 1) return stops[0].Color;

            // 找到相邻色标
            var lower = stops.LastOrDefault(s => s.Offset <= offset);
            var upper = stops.FirstOrDefault(s => s.Offset >= offset);

            if (lower == null) return upper?.Color ?? Colors.Transparent;
            if (upper == null) return lower.Color;
            if (lower.Offset == upper.Offset) return lower.Color;

            // 计算插值比例
            double t = (offset - lower.Offset) / (upper.Offset - lower.Offset);
            return InterpolateColor(lower.Color, upper.Color, t);
        }

        private static (LinearGradientBrush, LinearGradientBrush) NormalizeGradientStops(LinearGradientBrush a, LinearGradientBrush b)
        {
            if (a.GradientStops.Count == b.GradientStops.Count)
                return (a, b);

            // 确定目标点数
            var maxCount = Math.Max(a.GradientStops.Count, b.GradientStops.Count);

            // 标准化画刷A
            var normalizedA = a.GradientStops.Count < maxCount ?
                ExpandGradientStops(a, maxCount) : a;

            // 标准化画刷B
            var normalizedB = b.GradientStops.Count < maxCount ?
                ExpandGradientStops(b, maxCount) : b;

            return (normalizedA, normalizedB);
        }
        private static LinearGradientBrush ExpandGradientStops(LinearGradientBrush original, int targetCount)
        {
            if (original.GradientStops.Count >= targetCount)
                return original;

            // 生成新的均匀分布点
            var newStops = new List<GradientStop>();
            var existingStops = original.GradientStops.OrderBy(s => s.Offset).ToList();

            // 插入中间点
            for (int i = 0; i < targetCount; i++)
            {
                double targetOffset = (double)i / (targetCount - 1);
                var color = GetColorAtOffset(original, targetOffset);
                newStops.Add(new GradientStop(color, targetOffset));
            }

            return new LinearGradientBrush(new GradientStopCollection(newStops),
                original.StartPoint,
                original.EndPoint)
            {
                SpreadMethod = original.SpreadMethod,
                MappingMode = original.MappingMode,
                ColorInterpolationMode = original.ColorInterpolationMode
            };
        }
        private static Color GetColorAtOffset(LinearGradientBrush brush, double offset)
        {
            var orderedStops = brush.GradientStops.OrderBy(s => s.Offset).ToList();

            // 寻找相邻色标
            GradientStop? lower = orderedStops.LastOrDefault(s => s.Offset <= offset);
            GradientStop? upper = orderedStops.FirstOrDefault(s => s.Offset >= offset);

            if (lower == null) return upper?.Color ?? Colors.Transparent;
            if (upper == null) return lower.Color;
            if (lower.Offset == upper.Offset) return lower.Color;

            // 计算插值比例
            double t = (offset - lower.Offset) / (upper.Offset - lower.Offset);
            return InterpolateColor(lower.Color, upper.Color, t);
        }
        private static GradientStopCollection InterpolateStops(
            GradientStopCollection startStops,
            GradientStopCollection endStops,
            double t)
        {
            return new GradientStopCollection(
                startStops.Zip(endStops, (s, e) => new GradientStop(
                    InterpolateColor(s.Color, e.Color, t),
                    s.Offset + t * (e.Offset - s.Offset)
                ))
            );
        }

        private static Point InterpolatePoint(Point a, Point b, double t)
        {
            return new Point(
                a.X + t * (b.X - a.X),
                a.Y + t * (b.Y - a.Y));
        }
        private static Color InterpolateColor(Color a, Color b, double t)
        {
            return Color.FromArgb(
                (byte)(a.A + t * (b.A - a.A)),
                (byte)(a.R + t * (b.R - a.R)),
                (byte)(a.G + t * (b.G - a.G)),
                (byte)(a.B + t * (b.B - a.B)));
        }
    }
}
