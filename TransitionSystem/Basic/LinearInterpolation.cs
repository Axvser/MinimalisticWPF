using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.TransitionSystem.Basic
{
    public static class LinearInterpolation
    {
        public static List<object?> DoubleComputing(object? start, object? end, int steps)
        {
            if (start is not double d1 || end is not double d2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type double");
            }

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
            if (start is not Brush brush1 || end is not Brush brush2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type Color");
            }

            var rgb1 = RGB.FromBrush(brush1);
            var rgb2 = RGB.FromBrush(brush2);

#pragma warning disable CS8602 // RGB 静态方法已处理异常
            return rgb1.Interpolate(rgb1, rgb2, steps).Select(rgb => (object?)((rgb as RGB).Brush)).ToList();
#pragma warning restore CS8602 
        }
        public static List<object?> TransformComputing(object? start, object? end, int steps)
        {
            if (start is not Transform matrix1 || end is not Transform matrix2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type Transform");
            }

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
            if (start is not Point point1 || end is not Point point2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type Point");
            }

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
            if (start is not Thickness thickness1 || end is not Thickness thickness2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type Thickness");
            }

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
            if (start is not CornerRadius radius1 || end is not CornerRadius radius2)
            {
                throw new ArgumentException("TransitionSystem O01 : Both current and target must be of type CornerRadius");
            }

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
    }
}
