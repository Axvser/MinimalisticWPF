﻿using MinimalisticWPF.StructuralDesign.Animator;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF.TransitionSystem.Basic.BrushTransition
{
    public struct ValueRadialBrush(
        Point gradientOrigin, Point center,
        double radiusX, double radiusY,
        BrushMappingMode mappingMode,
        GradientSpreadMethod spreadMethod,
        IEnumerable<Tuple<Color, double>> stops) : IInterpolable
    {
        public static ValueRadialBrush Empty { get; } = new ValueRadialBrush();

        public static ValueRadialBrush FromRadialGradientBrush(RadialGradientBrush brush)
        {
            return new ValueRadialBrush(
                brush.GradientOrigin,
                brush.Center,
                brush.RadiusX,
                brush.RadiusY,
                brush.MappingMode,
                brush.SpreadMethod,
                brush.GradientStops.Select(s => Tuple.Create(s.Color, s.Offset)));
        }
        public RadialGradientBrush ToRadialGradientBrush()
        {
            var brush = new RadialGradientBrush
            {
                GradientOrigin = GradientOrigin,
                Center = Center,
                RadiusX = RadiusX,
                RadiusY = RadiusY,
                MappingMode = MappingMode,
                SpreadMethod = SpreadMethod
            };
            foreach (var stop in Stops)
            {
                brush.GradientStops.Add(new GradientStop(stop.Item1, stop.Item2));
            }
            return brush;
        }

        public Point GradientOrigin { get; set; } = gradientOrigin;
        public Point Center { get; set; } = center;
        public double RadiusX { get; set; } = radiusX;
        public double RadiusY { get; set; } = radiusY;

        public BrushMappingMode MappingMode { get; set; } = mappingMode;
        public GradientSpreadMethod SpreadMethod { get; set; } = spreadMethod;

        IEnumerable<Tuple<Color, double>> Stops { get; set; } = stops;

        public List<object?> Interpolate(object? current, object? target, int steps)
        {
            var value = current is ValueRadialBrush rb1 ? rb1 : Empty;
            var endBrush = target is ValueRadialBrush rb2 ? rb2 : Empty;

            Size size = new(100, 100); // 示例尺寸，实际应用中应动态获取

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

        private static RadialGradientBrush InterpolateBrushes(
            ValueRadialBrush start, ValueRadialBrush end,
            List<Tuple<Color, double>> startStops, List<Tuple<Color, double>> endStops,
            double ratio, List<double> offsets)
        {
            // 插值关键几何属性
            Point newGradientOrigin = new(
                start.GradientOrigin.X + (end.GradientOrigin.X - start.GradientOrigin.X) * ratio,
                start.GradientOrigin.Y + (end.GradientOrigin.Y - start.GradientOrigin.Y) * ratio);

            Point newCenter = new(
                start.Center.X + (end.Center.X - start.Center.X) * ratio,
                start.Center.Y + (end.Center.Y - start.Center.Y) * ratio);

            double newRadiusX = start.RadiusX + (end.RadiusX - start.RadiusX) * ratio;
            double newRadiusY = start.RadiusY + (end.RadiusY - start.RadiusY) * ratio;

            // 插值颜色停止点
            var stops = new List<Tuple<Color, double>>();
            for (int i = 0; i < offsets.Count; i++)
            {
                stops.Add(Tuple.Create(
                    InterpolateColor(startStops[i].Item1, endStops[i].Item1, ratio),
                    offsets[i]));
            }

            return new RadialGradientBrush()
            {
                GradientOrigin = newGradientOrigin,
                Center = newCenter,
                RadiusX = newRadiusX,
                RadiusY = newRadiusY,
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

        public static ValueRadialBrush CreateEquivalent(ValueRadialBrush start, ValueRadialBrush end, Size size)
        {
            return (start.MappingMode == end.MappingMode, start.SpreadMethod == end.SpreadMethod) switch
            {
                (true, true) => start,
                (true, false) => new ValueRadialBrush(
                    start.GradientOrigin, start.Center,
                    start.RadiusX, start.RadiusY,
                    end.MappingMode, end.SpreadMethod,
                    ConvertStops(start.SpreadMethod, end.SpreadMethod, start.Stops)),

                (false, true) => ConvertGeometry(start, end.MappingMode, size),
                (false, false) => ConvertGeometry(start, end.MappingMode, size)
                    .WithSpreadMethod(end.SpreadMethod, ConvertStops(start.SpreadMethod, end.SpreadMethod, start.Stops))
            };
        }

        private static ValueRadialBrush ConvertGeometry(ValueRadialBrush source, BrushMappingMode targetMode, Size size)
        {
            return new ValueRadialBrush(
                ConvertPoint(source.GradientOrigin, targetMode, size),
                ConvertPoint(source.Center, targetMode, size),
                ConvertRadius(source.RadiusX, targetMode, size.Width),
                ConvertRadius(source.RadiusY, targetMode, size.Height),
                targetMode,
                source.SpreadMethod,
                source.Stops);
        }

        private static double ConvertRadius(double radius, BrushMappingMode targetMode, double sizeDimension)
        {
            return targetMode == BrushMappingMode.Absolute
                ? radius * sizeDimension
                : radius / sizeDimension;
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

        private ValueRadialBrush WithSpreadMethod(GradientSpreadMethod method, IEnumerable<Tuple<Color, double>> stops)
        {
            return new ValueRadialBrush(
                this.GradientOrigin, this.Center,
                this.RadiusX, this.RadiusY,
                this.MappingMode, method,
                stops);
        }
    }
}