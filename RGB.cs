using System.Windows.Media;

namespace MinimalisticWPF
{
    public class RGB
    {
        public RGB() { R = 0; G = 0; B = 0; A = 0; }
        public RGB(int r, int g, int b)
        {
            R = Math.Clamp(r, 0, 255);
            G = Math.Clamp(g, 0, 255);
            B = Math.Clamp(b, 0, 255);
            A = 255;
        }
        public RGB(int r, int g, int b, int a)
        {
            R = Math.Clamp(r, 0, 255);
            G = Math.Clamp(g, 0, 255);
            B = Math.Clamp(b, 0, 255);
            A = Math.Clamp(a, 0, 255);
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }

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
        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }
    }
}
