using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    public static class BezierCurve
    {
        public static List<Point> Generate(int precision, ICollection<Point> controlPoints)
        {
            List<Point> curvePoints = new List<Point>(precision + 1);

            for (int i = 0; i <= precision; i++)
            {
                double t = (double)i / precision;
                Point pointOnCurve = CalculateBezierPoint(t, controlPoints);
                curvePoints.Add(pointOnCurve);
            }

            return curvePoints;
        }

        private static Point CalculateBezierPoint(double t, ICollection<Point> controlPoints)
        {
            var arr = controlPoints.ToArray();
            int n = arr.Length - 1;
            Point point = new Point(0, 0);

            for (int i = 0; i <= n; i++)
            {
                double binomialCoefficient = BinomialCoefficient(n, i);
                double term = binomialCoefficient * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
                point.X += term * arr[i].X;
                point.Y += term * arr[i].Y;
            }

            return point;
        }

        private static double BinomialCoefficient(int n, int k)
        {
            if (k < 0 || k > n)
                return 0;

            if (k == 0 || k == n)
                return 1;

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
