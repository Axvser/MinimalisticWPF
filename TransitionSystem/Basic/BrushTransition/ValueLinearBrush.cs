using MinimalisticWPF.StructuralDesign.Animator;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.TransitionSystem.Basic.BrushTransition
{
    public struct ValueLinearBrush(
        Point start, Point end,
        BrushMappingMode mappingMode,
        GradientSpreadMethod spreadMethod,
        IEnumerable<Tuple<Color, double>> stops) : IInterpolable
    {
        public static ValueLinearBrush Empty { get; } = new ValueLinearBrush();

        public static ValueLinearBrush FromLinearGradientBrush(LinearGradientBrush brush)
        {
            return new ValueLinearBrush(
                brush.StartPoint,
                brush.EndPoint,
                brush.MappingMode,
                brush.SpreadMethod,
                brush.GradientStops.Select(s => Tuple.Create(s.Color, s.Offset)));
        }
        public LinearGradientBrush ToLinearGradientBrush()
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = Start,
                EndPoint = End,
                MappingMode = MappingMode,
                SpreadMethod = SpreadMethod
            };
            foreach (var stop in Stops)
            {
                brush.GradientStops.Add(new GradientStop(stop.Item1, stop.Item2));
            }
            return brush;
        }

        public Point Start { get; set; } = start;
        public Point End { get; set; } = end;

        public BrushMappingMode MappingMode { get; set; } = mappingMode;
        public GradientSpreadMethod SpreadMethod { get; set; } = spreadMethod;

        IEnumerable<Tuple<Color, double>> Stops { get; set; } = stops;

        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            var value = current is ValueLinearBrush rb1 ? rb1 : Empty;
            var endBrush = target is ValueLinearBrush rb2 ? rb2 : Empty;

            // 假设size通过某种方式获取，这里使用默认尺寸示例
            Size size = new(100, 100);

            var equivalentStart = CreateEquivalent(value, endBrush, size);
            var equivalentEnd = CreateEquivalent(endBrush, value, size);

            var startStops = equivalentStart.Stops.OrderBy(s => s.Item2).ToList();
            var endStops = equivalentEnd.Stops.OrderBy(s => s.Item2).ToList();

            var allOffsets = startStops.Select(s => s.Item2)
                .Concat(endStops.Select(s => s.Item2))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var startInterpolated = InterpolateStopsAtOffsets(startStops, allOffsets);
            var endInterpolated = InterpolateStopsAtOffsets(endStops, allOffsets);

            var result = new List<object?>();
            for (int i = 0; i <= steps; i++)
            {
                double ratio = (double)i / steps;
                var interpolated = InterpolateBrushes(equivalentStart, equivalentEnd, startInterpolated, endInterpolated, ratio, allOffsets);
                result.Add(interpolated);
            }
            return result;
        }

        private static LinearGradientBrush InterpolateBrushes(
            ValueLinearBrush start, ValueLinearBrush end,
            List<Tuple<Color, double>> startStops, List<Tuple<Color, double>> endStops,
            double ratio, List<double> offsets)
        {
            Point newStart = new(
                start.Start.X + (end.Start.X - start.Start.X) * ratio,
                start.Start.Y + (end.Start.Y - start.Start.Y) * ratio);

            Point newEnd = new(
                start.End.X + (end.End.X - start.End.X) * ratio,
                start.End.Y + (end.End.Y - start.End.Y) * ratio);

            var stops = new List<Tuple<Color, double>>();
            for (int i = 0; i < offsets.Count; i++)
            {
                stops.Add(Tuple.Create(
                    InterpolateColor(startStops[i].Item1, endStops[i].Item1, ratio),
                    offsets[i]));
            }

            return new LinearGradientBrush()
            {
                StartPoint = newStart,
                EndPoint = newEnd,
                MappingMode = start.MappingMode,
                SpreadMethod = start.SpreadMethod,
                GradientStops = [.. stops.Select(s => new GradientStop(s.Item1, s.Item2))]
            };
        }

        private static List<Tuple<Color, double>> InterpolateStopsAtOffsets(
            List<Tuple<Color, double>> stops, List<double> targetOffsets)
        {
            if (!stops.Any()) return [];

            var sorted = stops.OrderBy(s => s.Item2).ToList();
            var results = new List<Tuple<Color, double>>();

            foreach (var offset in targetOffsets)
            {
                if (offset <= sorted[0].Item2)
                {
                    results.Add(Tuple.Create(sorted[0].Item1, offset));
                }
                else if (offset >= sorted[sorted.Count - 1].Item2)
                {
                    results.Add(Tuple.Create(sorted[sorted.Count - 1].Item1, offset));
                }
                else
                {
                    for (int i = 0; i < sorted.Count - 1; i++)
                    {
                        if (sorted[i].Item2 <= offset && offset <= sorted[i + 1].Item2)
                        {
                            double localRatio = (offset - sorted[i].Item2) /
                                (sorted[i + 1].Item2 - sorted[i].Item2);
                            results.Add(Tuple.Create(
                                InterpolateColor(sorted[i].Item1, sorted[i + 1].Item1, localRatio),
                                offset));
                            break;
                        }
                    }
                }
            }
            return results;
        }

        private static Color InterpolateColor(Color a, Color b, double ratio)
        {
            return Color.FromArgb(
                (byte)(a.A + (b.A - a.A) * ratio),
                (byte)(a.R + (b.R - a.R) * ratio),
                (byte)(a.G + (b.G - a.G) * ratio),
                (byte)(a.B + (b.B - a.B) * ratio));
        }

        public static ValueLinearBrush CreateEquivalent(ValueLinearBrush start, ValueLinearBrush end, Size size)
        {
            return (start.MappingMode == end.MappingMode, start.SpreadMethod == end.SpreadMethod) switch
            {
                (true, true) => start,
                (true, false) => new ValueLinearBrush(
                    start.Start, start.End,
                    end.MappingMode, end.SpreadMethod,
                    ConvertStops(start.SpreadMethod, end.SpreadMethod, start.Stops)),

                (false, true) => new ValueLinearBrush(
                    ConvertPoint(start.Start, end.MappingMode, size),
                    ConvertPoint(start.End, end.MappingMode, size),
                    end.MappingMode, end.SpreadMethod,
                    start.Stops),

                (false, false) => new ValueLinearBrush(
                    ConvertPoint(start.Start, end.MappingMode, size),
                    ConvertPoint(start.End, end.MappingMode, size),
                    end.MappingMode, end.SpreadMethod,
                    ConvertStops(start.SpreadMethod, end.SpreadMethod, start.Stops))
            };
        }

        private static IEnumerable<Tuple<Color, double>> ConvertStops(
            GradientSpreadMethod from, GradientSpreadMethod to, IEnumerable<Tuple<Color, double>> stops)
        {
            // 转换逻辑需要相应修改为使用double类型Offset
            // 示例PadToRepeat修改
            return (from, to) switch
            {
                (GradientSpreadMethod.Pad, GradientSpreadMethod.Repeat) =>
                    PadToRepeat(stops),
                _ => stops
            };
        }

        private static IEnumerable<Tuple<Color, double>> PadToRepeat(IEnumerable<Tuple<Color, double>> stops)
        {
            var stopList = stops.OrderBy(s => s.Item2).ToList();
            if (!stopList.Any()) yield break;

            foreach (var stop in stopList)
                yield return stop;

            if (stopList.Last().Item2 < 1.0)
                yield return Tuple.Create(stopList.Last().Item1, 1.0);
        }

        private static Point ConvertPoint(Point p, BrushMappingMode targetMode, Size size)
        {
            return targetMode switch
            {
                BrushMappingMode.Absolute => new Point(
                    p.X * size.Width, p.Y * size.Height),
                _ => new Point(
                    p.X / size.Width, p.Y / size.Height)
            };
        }
    }
}