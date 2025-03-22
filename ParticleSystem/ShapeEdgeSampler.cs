using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalisticWPF.ParticleSystem
{
    public class ShapeEdgeSampler
    {
        private readonly Shape _shape;
        private readonly FrameworkElement _relativeTo;
        private readonly double _totalLength;
        private readonly PathGeometry _flattenedGeometry;

        public ShapeEdgeSampler(Shape shape, FrameworkElement relativeTo)
        {
            _shape = shape ?? throw new ArgumentNullException(nameof(shape));
            _relativeTo = relativeTo ?? throw new ArgumentNullException(nameof(relativeTo));

            _flattenedGeometry = shape.RenderedGeometry.GetFlattenedPathGeometry();
            _totalLength = CalculateTotalLength(_flattenedGeometry);
        }

        public IEnumerable<Point> GetUniformPointsOnEdge(int sampleCount)
        {
            if (sampleCount < 1) throw new ArgumentOutOfRangeException(nameof(sampleCount));

            IEnumerable<Point> localPoints;

            switch (_shape)
            {
                case Ellipse ellipse:
                    localPoints = GetUniformPointsOnEllipse(ellipse, sampleCount);
                    break;
                case Rectangle rect:
                    localPoints = GetUniformPointsOnRectangle(rect, sampleCount);
                    break;
                default:
                    localPoints = GetUniformPointsOnPath(sampleCount);
                    break;
            }

            var transform = _shape.TransformToVisual(_relativeTo);
            return localPoints.Select(p => transform.Transform(p));
        }

        public Point GetRandomPointOnEdge(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            double targetLength = random.NextDouble() * _totalLength;
            Point localPoint = GetPointAtLength(_flattenedGeometry, targetLength);

            var transform = _shape.TransformToVisual(_relativeTo);
            return transform.Transform(localPoint);
        }

        private double CalculateTotalLength(PathGeometry geometry)
        {
            double length = 0;
            foreach (var figure in geometry.Figures)
            {
                Point start = figure.StartPoint;
                foreach (var segment in figure.Segments)
                {
                    if (segment is PolyLineSegment polyLine)
                    {
                        foreach (var point in polyLine.Points)
                        {
                            length += (point - start).Length;
                            start = point;
                        }
                    }
                }
            }
            return length;
        }

        private Point GetPointAtLength(PathGeometry geometry, double targetLength)
        {
            double accumulated = 0;
            foreach (var figure in geometry.Figures)
            {
                Point start = figure.StartPoint;
                foreach (var segment in figure.Segments)
                {
                    if (segment is PolyLineSegment polyLine)
                    {
                        foreach (var point in polyLine.Points)
                        {
                            Vector delta = point - start;
                            double segmentLength = delta.Length;
                            if (accumulated + segmentLength >= targetLength)
                            {
                                double ratio = (targetLength - accumulated) / segmentLength;
                                return start + delta * ratio;
                            }
                            accumulated += segmentLength;
                            start = point;
                        }
                    }
                }
            }
            return new Point();
        }

        private IEnumerable<Point> GetUniformPointsOnEllipse(Ellipse ellipse, int sampleCount)
        {
            double width = ellipse.ActualWidth;
            double height = ellipse.ActualHeight;
            double cx = width / 2;
            double cy = height / 2;
            double rx = cx;
            double ry = cy;

            return Enumerable.Range(0, sampleCount)
                .Select(i => 2 * Math.PI * i / sampleCount)
                .Select(theta => new Point(
                    cx + rx * Math.Cos(theta),
                    cy + ry * Math.Sin(theta)));
        }

        private IEnumerable<Point> GetUniformPointsOnRectangle(Rectangle rect, int sampleCount)
        {
            double width = rect.ActualWidth;
            double height = rect.ActualHeight;
            double perimeter = 2 * (width + height);

            var sideWeights = new[]
            {
                (Side: 0, Length: height), // 左边
                (Side: 1, Length: width),  // 上边
                (Side: 2, Length: height), // 右边
                (Side: 3, Length: width)   // 下边
            };

            var points = new List<Point>();
            foreach (var (side, length) in sideWeights)
            {
                int pointsOnSide = (int)Math.Round(sampleCount * length / perimeter);
                points.AddRange(GetPointsOnRectSide(side, width, height, pointsOnSide));
            }

            while (points.Count > sampleCount) points.RemoveAt(points.Count - 1);
            while (points.Count < sampleCount) points.Add(points.Last());

            return points;
        }

        private IEnumerable<Point> GetPointsOnRectSide(int side, double w, double h, int count)
        {
            if (count <= 0) yield break;

            double step = 1.0 / (count + 1);
            for (int i = 1; i <= count; i++)
            {
                double t = i * step;
                yield return side switch
                {
                    0 => new Point(0, h * t),        // 左边：从上到下
                    1 => new Point(w * t, 0),        // 上边：从左到右
                    2 => new Point(w, h * t),        // 右边：从上到下
                    _ => new Point(w * (1 - t), h)  // 下边：从右到左
                };
            }
        }

        private IEnumerable<Point> GetUniformPointsOnPath(int sampleCount)
        {
            double stepLength = _totalLength / sampleCount;

            return Enumerable.Range(0, sampleCount)
                .Select(i => GetPointAtLength(_flattenedGeometry, i * stepLength));
        }
    }
}