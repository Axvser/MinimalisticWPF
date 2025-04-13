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

        public static List<object?> BrushComputing(object? start, object? end, int steps)
        {
            object startBrush = start ?? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            object endBrush = end ?? new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            return (startBrush.GetType().Name, endBrush.GetType().Name) switch
            {
                (nameof(SolidColorBrush), nameof(SolidColorBrush)) => InterpolateSolidColorBrush((SolidColorBrush)startBrush, (SolidColorBrush)endBrush, steps),
                (nameof(SolidColorBrush), nameof(LinearGradientBrush)) => InterpolateLinearGradientBrush((LinearGradientBrush)ConvertToGradientBrush((SolidColorBrush)startBrush, (Brush)endBrush), (LinearGradientBrush)endBrush, steps, endBrush),
                (nameof(SolidColorBrush), nameof(RadialGradientBrush)) => InterpolateRadialGradientBrush((RadialGradientBrush)ConvertToGradientBrush((SolidColorBrush)startBrush, (Brush)endBrush), (RadialGradientBrush)endBrush, steps, endBrush),
                (nameof(LinearGradientBrush), nameof(SolidColorBrush)) => InterpolateLinearGradientBrush((LinearGradientBrush)startBrush, (LinearGradientBrush)ConvertToGradientBrush((SolidColorBrush)endBrush, (Brush)startBrush), steps, endBrush),
                (nameof(RadialGradientBrush), nameof(SolidColorBrush)) => InterpolateRadialGradientBrush((RadialGradientBrush)startBrush, (RadialGradientBrush)ConvertToGradientBrush((SolidColorBrush)endBrush, (Brush)startBrush), steps, endBrush),
                (nameof(LinearGradientBrush), nameof(LinearGradientBrush)) => InterpolateLinearGradientBrush((LinearGradientBrush)startBrush, (LinearGradientBrush)endBrush, steps, endBrush),
                (nameof(RadialGradientBrush), nameof(RadialGradientBrush)) => InterpolateRadialGradientBrush((RadialGradientBrush)startBrush, (RadialGradientBrush)endBrush, steps, endBrush),
                _ => InterpolateBrushOpacity(startBrush as Brush ?? Brushes.Transparent, endBrush as Brush ?? Brushes.Transparent, steps, endBrush)
            };
        }
        public static List<object?> InterpolateSolidColorBrush(SolidColorBrush start, SolidColorBrush end, int steps)
        {
            var rgb1 = RGB.FromBrush(start);
            var rgb2 = RGB.FromBrush(end);
            return [.. rgb1.Interpolate(rgb1, rgb2, steps).Select(rgb => (object?)(((rgb as RGB) ?? RGB.Empty)).Brush)];
        }
        public static List<object?> InterpolateLinearGradientBrush(LinearGradientBrush start, LinearGradientBrush end, int steps, object oriEnd)
        {
            var result = new List<object?>();

            try
            {
                // 获取对齐后的停止点对
                var stopPairs = GetStopPairs(start, end);

                for (int i = 0; i < steps; i++)
                {
                    double t = (double)(i + 1) / steps;

                    var brush = new LinearGradientBrush
                    {
                        SpreadMethod = start.SpreadMethod,
                        StartPoint = Lerp(start.StartPoint, end.StartPoint, t),
                        EndPoint = Lerp(start.EndPoint, end.EndPoint, t)
                    };

                    // 插值每个点对
                    foreach (var pair in stopPairs)
                    {
                        var startStop = pair.Item1;
                        var endStop = pair.Item2;

                        var color = InterpolateColor(
                            startStop.Color,
                            endStop.Color,
                            t
                        );

                        var offset = startStop.Offset +
                            t * (endStop.Offset - startStop.Offset);

                        brush.GradientStops.Add(new GradientStop(color, offset));
                    }

                    result.Add(brush);
                }
            }
            catch
            {
                // 异常时生成透明画刷序列
                return [.. Enumerable.Repeat<object?>(Brushes.Transparent, steps)];
            }

            return ApplyEdgeCases(result, steps, start, oriEnd);
        }
        public static List<object?> InterpolateRadialGradientBrush(RadialGradientBrush start, RadialGradientBrush end, int steps, object oriEnd)
        {
            List<object?> result = new(steps);
            if (steps <= 0) return [oriEnd];

            if (start.SpreadMethod != end.SpreadMethod ||
                start.Center != end.Center ||
                start.GradientOrigin != end.GradientOrigin)
            {
                return InterpolateBrushOpacity(start, end, steps, oriEnd);
            }

            try
            {
                var stopPairs = GetStopPairs(start, end);

                for (int i = 0; i < steps; i++)
                {
                    double t = (double)(i + 1) / steps;

                    var brush = new RadialGradientBrush
                    {
                        GradientOrigin = Lerp(start.GradientOrigin, end.GradientOrigin, t),
                        Center = Lerp(start.Center, end.Center, t),
                        RadiusX = Lerp(start.RadiusX, end.RadiusX, t),
                        RadiusY = Lerp(start.RadiusY, end.RadiusY, t),
                        SpreadMethod = start.SpreadMethod,
                        MappingMode = start.MappingMode,
                        Opacity = Lerp(start.Opacity, end.Opacity, t)
                    };

                    foreach (var pair in stopPairs)
                    {
                        var startStop = pair.Item1;
                        var endStop = pair.Item2;

                        Color color = InterpolateColor(
                            startStop.Color,
                            endStop.Color,
                            t
                        );

                        double offset = Lerp(startStop.Offset, endStop.Offset, t);
                        brush.GradientStops.Add(new GradientStop(color, offset));
                    }

                    result.Add(brush);
                }
            }
            catch
            {
                return InterpolateBrushOpacity(start, end, steps, oriEnd);
            }

            return ApplyEdgeCases(result, steps, start, oriEnd);
        }
        public static List<object?> InterpolateBrushOpacity(Brush start, Brush end, int steps, object oriEnd)
        {
            var result = new List<object?>(steps);
            if (steps <= 0) return result;

            // 计算实际透明度
            double startAlpha = GetEffectiveOpacity(start);
            double endAlpha = GetEffectiveOpacity(end);

            int halfSteps = (int)Math.Ceiling(steps / 2.0);

            for (int i = 0; i < steps; i++)
            {
                // 每帧独立克隆
                var frameStart = start.CloneCurrentValue() ?? Brushes.Transparent;
                var frameEnd = end.CloneCurrentValue() ?? Brushes.Transparent;

                if (i < halfSteps)
                {
                    // 阶段1：双画刷统一向0.5过渡
                    double t = (double)i / halfSteps;
                    SetCompositeOpacity(frameStart, Lerp(startAlpha, 0.5, t));
                    SetCompositeOpacity(frameEnd, Lerp(0, 0.5, t));
                }
                else
                {
                    // 阶段2：start→透明，end→目标值
                    double t = (double)(i - halfSteps) / (steps - halfSteps);
                    SetCompositeOpacity(frameStart, Lerp(0.5, 0, t));
                    SetCompositeOpacity(frameEnd, Lerp(0.5, endAlpha, t));
                }

                // 使用DrawingBrush实现完美混合
                result.Add(new DrawingBrush(new DrawingGroup
                {
                    Children =
                    {
                        new GeometryDrawing(frameStart, null, new RectangleGeometry(new Rect(0, 0, 1, 1))),
                        new GeometryDrawing(frameEnd, null, new RectangleGeometry(new Rect(0, 0, 1, 1)))
                    }
                }));
            }

            return ApplyEdgeCases(result, steps, start, oriEnd);
        }

        private static double Lerp(double a, double b, double t) => a + (b - a) * t;
        private static double GetEffectiveOpacity(Brush brush)
        {
            if (brush == null) return 0;

            double opacity = brush.Opacity;
            if (brush is SolidColorBrush scb)
            {
                opacity *= scb.Color.A / 255.0;
            }
#if NET
            return Math.Clamp(opacity, 0, 1);
#elif NETFRAMEWORK
            return opacity.Clamp(0, 1);
#endif
        }
        private static void SetCompositeOpacity(Brush brush, double targetOpacity)
        {
            if (brush == null) return;
#if NET
            targetOpacity = Math.Clamp(targetOpacity, 0, 1);
#elif NETFRAMEWORK
            targetOpacity = targetOpacity.Clamp(0, 1);
#endif

            // 优先修改Color.A通道
            if (brush is SolidColorBrush scb)
            {
                var color = scb.Color;
                scb.Color = Color.FromArgb(
                    (byte)(targetOpacity * 255),
                    color.R,
                    color.G,
                    color.B
                );
                brush.Opacity = 1; // 颜色通道已处理透明度
            }
            else
            {
                brush.Opacity = targetOpacity;
            }
        }

        private static Point Lerp(Point a, Point b, double t) => new(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
        private static List<Tuple<GradientStop, GradientStop>> GetStopPairs(GradientBrush startBrush, GradientBrush endBrush)
        {
            var pairs = new List<Tuple<GradientStop, GradientStop>>();

            try
            {
                // 规则4：异常检查
                if (startBrush?.GradientStops == null || endBrush?.GradientStops == null ||
                    startBrush.GradientStops.Count == 0 || endBrush.GradientStops.Count == 0)
                {
                    return [DefaultStopPair()];
                }

                // 获取两个画刷的所有偏移点（不自动添加0.0/1.0）
                var allOffsets = new SortedSet<double>(
                    startBrush.GradientStops.Select(s => s.Offset)
                        .Concat(endBrush.GradientStops.Select(s => s.Offset))
                );

                // 规则1：首尾配对（使用实际的首尾点，非强制0.0/1.0）
                var startFirst = startBrush.GradientStops.OrderBy(s => s.Offset).First();
                var endFirst = endBrush.GradientStops.OrderBy(s => s.Offset).First();
                pairs.Add(Tuple.Create(startFirst, endFirst));

                var startLast = startBrush.GradientStops.OrderBy(s => s.Offset).Last();
                var endLast = endBrush.GradientStops.OrderBy(s => s.Offset).Last();
                pairs.Add(Tuple.Create(startLast, endLast));

                // 规则2：中间点最近匹配
                var middleOffsets = allOffsets
                    .Except([startFirst.Offset, startLast.Offset])
                    .ToList();

                var endStops = endBrush.GradientStops.OrderBy(s => s.Offset).ToList();

                foreach (var offset in middleOffsets)
                {
                    // 在endBrush中找最近的实际停止点
                    var nearestEndStop = endStops
                        .OrderBy(es => Math.Abs(es.Offset - offset))
                        .First();

                    // 在startBrush中取当前偏移量的等价点
                    var startStop = GetStopAtOffset(startBrush, offset);
                    pairs.Add(Tuple.Create(startStop, nearestEndStop));
                }

                // 规则3：补全剩余点（确保所有偏移量都有对应关系）
                var pairedStartOffsets = new HashSet<double>(pairs.Select(p => p.Item1.Offset));
                var remainingOffsets = allOffsets.Except(pairedStartOffsets).ToList();

                foreach (var offset in remainingOffsets)
                {
                    pairs.Add(Tuple.Create(
                        GetStopAtOffset(startBrush, offset),
                        GetStopAtOffset(endBrush, offset)
                    ));
                }

                return [.. pairs.OrderBy(p => p.Item1.Offset)];
            }
            catch
            {
                return [DefaultStopPair()];
            }
        }
        private static Color InterpolateColor(Color color1, Color color2, double fraction)
        {
            byte a = (byte)(color1.A + (color2.A - color1.A) * fraction);
            byte r = (byte)(color1.R + (color2.R - color1.R) * fraction);
            byte g = (byte)(color1.G + (color2.G - color1.G) * fraction);
            byte b = (byte)(color1.B + (color2.B - color1.B) * fraction);

            return Color.FromArgb(a, r, g, b);
        }

        private static Brush ConvertToGradientBrush(SolidColorBrush solid, Brush target)
        {
            return target switch
            {
                LinearGradientBrush linear => CreateAdaptiveGradient(linear, solid.Color),
                RadialGradientBrush radial => CreateAdaptiveGradient(radial, solid.Color),
                _ => solid
            };
        }
        private static LinearGradientBrush CreateAdaptiveGradient(LinearGradientBrush template, Color color)
        {
            // 保持原始渐变结构，用纯色替换颜色值
            var stops = template.GradientStops
                .Select(s => new GradientStop(color, s.Offset))
                .ToList();

            return new LinearGradientBrush(
                [.. stops],
                template.StartPoint,
                template.EndPoint
            )
            {
                SpreadMethod = template.SpreadMethod,
                MappingMode = template.MappingMode
            };
        }
        private static RadialGradientBrush CreateAdaptiveGradient(RadialGradientBrush template, Color color)
        {
            // 保持原始结构，仅替换颜色
            var stops = template.GradientStops
                .Select(s => new GradientStop(color, s.Offset))
                .ToList();

            return new RadialGradientBrush
            {
                GradientStops = [.. stops],
                GradientOrigin = template.GradientOrigin,
                Center = template.Center,
                RadiusX = template.RadiusX,
                RadiusY = template.RadiusY,
                SpreadMethod = template.SpreadMethod,
                MappingMode = template.MappingMode
            };
        }

        private static GradientStop GetStopAtOffset(GradientBrush value, double offset)
        {
            if (value == null || value.GradientStops.Count == 0)
            {
                return new GradientStop(Colors.Transparent, 0.0);
            }

            var stops = value.GradientStops.OrderBy(s => s.Offset).ToList();
            var firstStop = stops.First();
            var lastStop = stops.Last();

            if (stops.Count == 0)
            {
                return new GradientStop(Colors.Transparent, 0.0);
            }

            double normalizedOffset = offset;

            if (value.SpreadMethod == GradientSpreadMethod.Reflect)
            {
                normalizedOffset = Math.Abs(offset % 2);
                if (normalizedOffset > 1)
                {
                    normalizedOffset = 2 - normalizedOffset;
                }
            }
            else if (value.SpreadMethod == GradientSpreadMethod.Repeat)
            {
                normalizedOffset = offset % 1;
                if (normalizedOffset < 0)
                {
                    normalizedOffset += 1;
                }
            }
            else
            {
                if (offset <= 0)
                {
                    return new GradientStop(firstStop.Color, firstStop.Offset);
                }
                if (offset >= 1)
                {
                    return new GradientStop(lastStop.Color, lastStop.Offset);
                }
                normalizedOffset = offset;
            }

            GradientStop lowerStop = stops[0];
            GradientStop upperStop = stops[stops.Count - 1];

            for (int i = 0; i < stops.Count - 1; i++)
            {
                if (stops[i].Offset <= normalizedOffset && stops[i + 1].Offset >= normalizedOffset)
                {
                    lowerStop = stops[i];
                    upperStop = stops[i + 1];
                    break;
                }
            }

            if (Math.Abs(normalizedOffset - lowerStop.Offset) < double.Epsilon)
            {
                return new GradientStop(lowerStop.Color, lowerStop.Offset);
            }
            if (Math.Abs(normalizedOffset - upperStop.Offset) < double.Epsilon)
            {
                return new GradientStop(upperStop.Color, upperStop.Offset);
            }

            double range = upperStop.Offset - lowerStop.Offset;
            double fraction = (normalizedOffset - lowerStop.Offset) / range;

            Color interpolatedColor = InterpolateColor(lowerStop.Color, upperStop.Color, fraction);

            return new GradientStop(interpolatedColor, normalizedOffset);
        }
        private static Tuple<GradientStop, GradientStop> DefaultStopPair()
        {
            var transparent = new GradientStop(Colors.Transparent, 0.0);
            return Tuple.Create(transparent, transparent);
        }
        private static List<object?> ApplyEdgeCases(List<object?> result, int steps, object start, object end)
        {
            if (steps == 0) return [end];
            if (steps > 1)
            {
                result[0] = start;
                result[result.Count - 1] = end;
            }
            return result;
        }
    }
}
