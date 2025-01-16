using System.Windows.Media;

namespace MinimalisticWPF.Extension
{
    public static class RGBExtension
    {
        public static RGB ToRGB(this Color color)
        {
            return RGB.FromColor(color);
        }
        public static RGB ToRGB<T>(this T brush) where T : Brush
        {
            return RGB.FromBrush(brush);
        }
    }
}
