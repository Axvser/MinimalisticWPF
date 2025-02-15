#if NET471_OR_GREATER

namespace MinimalisticWPF
{
    public static class HashCodeExtensions
    {
        public static int Combine(int h1, int h2)
        {
            unchecked
            {
                return ((h1 << 5) + h1) ^ h2;
            }
        }

        public static int Combine(int h1, int h2, int h3)
        {
            return Combine(Combine(h1, h2), h3);
        }

        public static int Combine(int h1, int h2, int h3, int h4)
        {
            return Combine(Combine(h1, h2, h3), h4);
        }
    }
}

#endif