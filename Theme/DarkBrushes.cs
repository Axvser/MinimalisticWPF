using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public sealed class DarkBrushes
    {
        internal DarkBrushes() { }
        public static DarkBrushes Selector { get; } = new DarkBrushes();

        public static Brush Default = Brushes.Transparent;

        public static Brush H1 = "#ecf0f1".ToBrush();
        public static Brush H2 = "#bdc3c7".ToBrush();
        public static Brush H3 = "#95a5a6".ToBrush();
        public static Brush H4 = "#7f8c8d".ToBrush();
        public static Brush H5 = "#6c7a89".ToBrush();

        public static Brush P1 = "#ecf0f1".ToBrush();
        public static Brush P2 = "#bdc3c7".ToBrush();
        public static Brush P3 = "#95a5a6".ToBrush();
        public static Brush P4 = "#7f8c8d".ToBrush();
        public static Brush P5 = "#6c7a89".ToBrush();

        private static RGB Background = new RGB(30,30,30,255);
        public static Brush B1 = Background.Brush;
        public static Brush B2 = Background.SubA(200).Brush;
        public static Brush B3 = Background.SubA(155).Brush;
        public static Brush B4 = Background.SubA(100).Brush;
        public static Brush B5 = Background.SubA(55).Brush;

        public static Brush E1 = Brushes.Cyan;
        public static Brush E2 = Brushes.Violet;
        public static Brush E3 = Brushes.Tomato;
        public static Brush E4 = Brushes.Lime;
        public static Brush E5 = Brushes.LightYellow;

        public Brush Select(BrushTags brushenum)
        {
            switch (brushenum)
            {
                case BrushTags.Default:
                    return Default;
                case BrushTags.H1:
                    return H1;
                case BrushTags.H2:
                    return H2;
                case BrushTags.H3:
                    return H3;
                case BrushTags.H4:
                    return H4;
                case BrushTags.H5:
                    return H5;
                case BrushTags.P1:
                    return P1;
                case BrushTags.P2:
                    return P2;
                case BrushTags.P3:
                    return P3;
                case BrushTags.P4:
                    return P4;
                case BrushTags.P5:
                    return P5;
                case BrushTags.B1:
                    return B1;
                case BrushTags.B2:
                    return B2;
                case BrushTags.B3:
                    return B3;
                case BrushTags.B4:
                    return B4;
                case BrushTags.B5:
                    return B5;
                case BrushTags.E1:
                    return E1;
                case BrushTags.E2:
                    return E2;
                case BrushTags.E3:
                    return E3;
                case BrushTags.E4:
                    return E4;
                case BrushTags.E5:
                    return E5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(brushenum), brushenum, null);
            }
        }
    }
}
