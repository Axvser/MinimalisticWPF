using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    internal static class BezierCurve
    {
        public static List<Point> Generate(int precision, ICollection<Point> controlPoints)
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            int controlPointCount = controlPoints.Count;
            if (controlPointCount == 0)
                throw new ArgumentException("Control points collection must not be empty.", nameof(controlPoints));

            List<Point> curvePoints = new List<Point>(precision + 1);

            if (controlPointCount == 1)
            {
                Point singlePoint = controlPoints.First();
                curvePoints.AddRange(Enumerable.Repeat(singlePoint, precision + 1));
                return curvePoints;
            }

            // 生成高精度点以近似曲线
            List<Point> highPrecisionPoints = GenerateHighPrecision(controlPoints, precision);
            List<double> cumulativeLengths = ComputeCumulativeLengths(highPrecisionPoints);
            double totalLength = cumulativeLengths.Last();

            if (totalLength <= 0)
            {
                // 所有点重合，返回起始点
                Point firstPoint = controlPoints.First();
                curvePoints.AddRange(Enumerable.Repeat(firstPoint, precision + 1));
                return curvePoints;
            }

            // 计算等距点
            curvePoints.Add(highPrecisionPoints.First()); // 确保起点准确

            if (precision > 0)
            {
                double step = totalLength / precision;
                for (int i = 1; i < precision; i++)
                {
                    double targetDistance = i * step;
                    Point p = FindPointAtDistance(highPrecisionPoints, cumulativeLengths, targetDistance);
                    curvePoints.Add(p);
                }
            }

            curvePoints.Add(highPrecisionPoints.Last()); // 确保终点准确

            return curvePoints;
        }

        private static List<Point> GenerateHighPrecision(ICollection<Point> controlPoints, int precision)
        {
            List<Point> points = new List<Point>(precision + 1);
            for (int i = 0; i <= precision; i++)
            {
                double t = (double)i / precision;
                points.Add(CalculateBezierPoint(t, controlPoints));
            }
            return points;
        }

        private static List<double> ComputeCumulativeLengths(List<Point> points)
        {
            List<double> lengths = new List<double>(points.Count) { 0 };
            double currentLength = 0;
            for (int i = 1; i < points.Count; i++)
            {
                Point prev = points[i - 1];
                Point current = points[i];
                currentLength += DistanceBetween(prev, current);
                lengths.Add(currentLength);
            }
            return lengths;
        }

        private static Point FindPointAtDistance(List<Point> points, List<double> cumulativeLengths, double targetDistance)
        {
            // 二分查找找到目标距离所在区间
            int index = Array.BinarySearch(cumulativeLengths.ToArray(), targetDistance);
            if (index < 0) index = ~index;

            if (index <= 0) return points.First();
            if (index >= cumulativeLengths.Count) return points.Last();

            // 线性插值
            double segmentStart = cumulativeLengths[index - 1];
            double segmentEnd = cumulativeLengths[index];
            double t = (targetDistance - segmentStart) / (segmentEnd - segmentStart);
            t = Math.Max(0, Math.Min(1, t));

            Point p1 = points[index - 1];
            Point p2 = points[index];

            return new Point(
                p1.X + t * (p2.X - p1.X),
                p1.Y + t * (p2.Y - p1.Y)
            );
        }

        private static double DistanceBetween(Point a, Point b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static Point CalculateBezierPoint(double t, ICollection<Point> controlPoints)
        {
            Point[] arr = controlPoints.ToArray();
            int n = arr.Length - 1;
            Point point = new Point(0, 0);

            for (int i = 0; i <= n; i++)
            {
                double coefficient = BinomialCoefficient(n, i);
                double term = coefficient * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
                point.X += term * arr[i].X;
                point.Y += term * arr[i].Y;
            }
            return point;
        }

        private static double BinomialCoefficient(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;

            k = Math.Min(k, n - k);
            double result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= (n - k + i) / (double)i;
            }
            return result;
        }
    }
}