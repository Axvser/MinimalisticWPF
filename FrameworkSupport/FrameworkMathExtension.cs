﻿#if NETFRAMEWORK

namespace MinimalisticWPF
{
    public static class FrameworkMathExtension
    {
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
    }
}

#endif