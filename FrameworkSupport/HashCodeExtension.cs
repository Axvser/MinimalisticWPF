#if NETFRAMEWORK

namespace MinimalisticWPF
{
    public static class HashCodeExtensions
    {
        public static int Combine(int h1, int h2)
        {
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }

        public static int Combine(int h1, int h2, int h3)
        {
            return Combine(Combine(h1, h2), h3);
        }

        public static int Combine(int h1, int h2, int h3, int h4)
        {
            return Combine(Combine(h1, h2, h3), h4);
        }

        public static int Combine(params int[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentException("At least one value must be provided.");

            int hash = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                hash = Combine(hash, values[i]);
            }
            return hash;
        }
    }
}

#endif