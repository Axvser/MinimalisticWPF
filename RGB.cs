using System.Windows.Media;

namespace MinimalisticWPF
{
    public class RGB
    {
        public RGB() { }
        public RGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
        public RGB(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        private int r = 0;
        private int g = 0;
        private int b = 0;
        private int a = 255;

#if NET5_0_OR_GREATER
        public int R
        {
            get => r;
            set => r = Math.Clamp(value, 0, 255);
        }
        public int G
        {
            get => g;
            set => g = Math.Clamp(value, 0, 255);
        }
        public int B
        {
            get => b;
            set => b = Math.Clamp(value, 0, 255);
        }
        public int A
        {
            get => a;
            set => a = Math.Clamp(value, 0, 255);
        }
#endif
#if NET471_OR_GREATER
        public int R
        {
            get => r;
            set => r = value.Clamp(0, 255);
        }
        public int G
        {
            get => g;
            set => g = value.Clamp(0, 255);
        }
        public int B
        {
            get => b;
            set => b = value.Clamp(0, 255);
        }
        public int A
        {
            get => a;
            set => a = value.Clamp(0, 255);
        }
#endif

        public Color Color => Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
        public SolidColorBrush SolidColorBrush => new(Color);
        public Brush Brush => SolidColorBrush;

        public static RGB FromString(string color)
        {
            var original = (Color)ColorConverter.ConvertFromString(color);
            return new RGB(original.R, original.G, original.B, original.A);
        }
        public static RGB FromColor(Color color)
        {
            return new RGB(color.R, color.G, color.B, color.A);
        }
        public static RGB FromBrush(Brush brush)
        {
            var color = (Color)ColorConverter.ConvertFromString(brush.ToString());
            return new RGB(color.R, color.G, color.B, color.A);
        }

        public override string ToString()
        {
            return Color.ToString();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is RGB rgb) return rgb.R == R && rgb.G == G && rgb.B == B && rgb.A == A;
            if (obj is Color color) return color.R == R && color.G == G && color.B == B && color.A == A;
            if (obj is Brush brush) return Equals(FromBrush(brush));
            if (obj is string text) return Equals(FromString(text));
            return false;
        }
#if NET5_0_OR_GREATER
        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }
#endif

#if NET471_OR_GREATER
        public override int GetHashCode()
        {
            return HashCodeExtensions.Combine(R, G, B, A);
        }
#endif
    }
}
