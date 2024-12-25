using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.Animator
{
    public static class LinearInterpolation
    {
        public static List<object?> DoubleComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            var d1 = start as double?;
            var d2 = end as double?;
            d1 = d1 == null || d1 == double.NaN ? 0 : d1;
            d2 = d2 == null || d2 == double.NaN ? 0 : d2;
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

            return result;
        }
        public static List<object?> BrushComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            var color1 = (start as SolidColorBrush)?.Color;
            var color2 = (end as SolidColorBrush)?.Color;
            color1 ??= new Color();
            color2 ??= new Color();

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (var i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;
                var r = (byte)(color1.Value.R + t * (color2.Value.R - color1.Value.R));
                var g = (byte)(color1.Value.G + t * (color2.Value.G - color1.Value.G));
                var b = (byte)(color1.Value.B + t * (color2.Value.B - color1.Value.B));
                var a = (byte)(color1.Value.A + t * (color2.Value.A - color1.Value.A));
                result.Add(new SolidColorBrush(Color.FromArgb(a, r, g, b)));
            }

            return result;
        }
        public static List<object?> TransformComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            Matrix matrix1 = ((Transform)(start ?? new TransformGroup())).Value;
            Matrix matrix2 = ((Transform)(end ?? new TransformGroup())).Value;

            if (steps == 0)
            {
                result.Add(end);
                return result;
            }

            for (int i = 0; i < steps; i++)
            {
                var t = (double)(i + 1) / steps;

                double m11 = matrix1.M11 + t * (matrix2.M11 - matrix1.M11);
                double m12 = matrix1.M12 + t * (matrix2.M12 - matrix1.M12);
                double m21 = matrix1.M21 + t * (matrix2.M21 - matrix1.M21);
                double m22 = matrix1.M22 + t * (matrix2.M22 - matrix1.M22);
                double offsetX = matrix1.OffsetX + t * (matrix2.OffsetX - matrix1.OffsetX);
                double offsetY = matrix1.OffsetY + t * (matrix2.OffsetY - matrix1.OffsetY);

                var interpolatedMatrixStr = $"{m11},{m12},{m21},{m22},{offsetX},{offsetY}";
                var transform = Transform.Parse(interpolatedMatrixStr);
                result.Add(transform);
            }

            return result;
        }
        public static List<object?> PointComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            var point1 = start as Point? ?? new Point(0, 0);
            var point2 = end as Point? ?? new Point(0, 0);

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

            return result;
        }
        public static List<object?> ThicknessComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            var thickness1 = start as Thickness? ?? new Thickness(0);
            var thickness2 = end as Thickness? ?? new Thickness(0);

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

            return result;
        }
        public static List<object?> CornerRadiusComputing(object? start, object? end, int steps)
        {
            List<object?> result = new(steps);

            var radius1 = start as CornerRadius? ?? new CornerRadius(0);
            var radius2 = end as CornerRadius? ?? new CornerRadius(0);

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

            return result;
        }
    }
}
