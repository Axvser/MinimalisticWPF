using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class BrushBuilder
    {
        public static TempLinearColorBrush Linear() => new();
        public static TempRadialColorBrush Radial() => new();
        public static TempSolidColorBrush Solid() => new();
    }

    public class TempLinearColorBrush : TempGradientBrush
    {
        private bool _isMappingModeManuallySet;
        public Point StartPoint { get; private set; } = new Point(0, 0);
        public Point EndPoint { get; private set; } = new Point(1, 1);
        public TempLinearColorBrush Repeat()
        {
            SpreadMethod = GradientSpreadMethod.Repeat;
            return this;
        }
        public TempLinearColorBrush Reflect()
        {
            SpreadMethod = GradientSpreadMethod.Reflect;
            return this;
        }
        public TempLinearColorBrush Pad()
        {
            SpreadMethod = GradientSpreadMethod.Pad;
            return this;
        }
        public TempLinearColorBrush Start(double x, double y)
        {
            StartPoint = new Point(x, y);
            UpdateAutoMappingMode();
            return this;
        }
        public TempLinearColorBrush End(double x, double y)
        {
            EndPoint = new Point(x, y);
            UpdateAutoMappingMode();
            return this;
        }
        public TempLinearColorBrush Stop(Color color, double offset)
        {
            GradientStops.Add(new GradientStop(color, offset));
            return this;
        }
        public TempLinearColorBrush Stop(RGB rgb, double offset)
        {
            GradientStops.Add(new GradientStop(rgb.Color, offset));
            return this;
        }
        public TempLinearColorBrush Stop(string colorStr, double offset)
        {
            GradientStops.Add(new GradientStop(RGB.FromString(colorStr).Color, offset));
            return this;
        }
        public TempLinearColorBrush Stop(Brush brush, double offset, double alpha)
        {
            GradientStops.Add(new GradientStop(RGB.FromBrush(brush).Color, offset));
            return this;
        }
        public LinearGradientBrush Build()
        {
            return new LinearGradientBrush([.. GradientStops], StartPoint, EndPoint)
            {
                MappingMode = MappingMode,
                SpreadMethod = SpreadMethod
            };
        }
        private void UpdateAutoMappingMode()
        {
            if (_isMappingModeManuallySet) return;
            bool isAbsolute = StartPoint.X > 1 || StartPoint.Y > 1 || EndPoint.X > 1 || EndPoint.Y > 1;
            MappingMode = isAbsolute ? BrushMappingMode.Absolute : BrushMappingMode.RelativeToBoundingBox;
        }
    }
    public class TempRadialColorBrush : TempGradientBrush
    {
        public Point CenterPoint { get; private set; } = new Point(0.5, 0.5);
        public Point GradientOrigin { get; private set; } = new Point(0.5, 0.5);
        public double RadiusX { get; private set; } = 0.5;
        public double RadiusY { get; private set; } = 0.5;
        public TempRadialColorBrush Repeat()
        {
            SpreadMethod = GradientSpreadMethod.Repeat;
            return this;
        }
        public TempRadialColorBrush Reflect()
        {
            SpreadMethod = GradientSpreadMethod.Reflect;
            return this;
        }
        public TempRadialColorBrush Pad()
        {
            SpreadMethod = GradientSpreadMethod.Pad;
            return this;
        }
        public TempRadialColorBrush Center(double x, double y)
        {
            CenterPoint = new Point(x, y);
            return this;
        }
        public TempRadialColorBrush Origin(double x, double y)
        {
            GradientOrigin = new Point(x, y);
            return this;
        }
        public TempRadialColorBrush Radius(double x, double y)
        {
            RadiusX = x;
            RadiusY = y;
            return this;
        }
        public TempRadialColorBrush Stop(Color color, double offset)
        {
            GradientStops.Add(new GradientStop(color, offset));
            return this;
        }
        public TempRadialColorBrush Stop(RGB rgb, double offset)
        {
            GradientStops.Add(new GradientStop(rgb.Color, offset));
            return this;
        }
        public TempRadialColorBrush Stop(string colorStr, double offset)
        {
            GradientStops.Add(new GradientStop(RGB.FromString(colorStr).Color, offset));
            return this;
        }
        public TempRadialColorBrush Stop(Brush brush, double offset, double alpha)
        {
            GradientStops.Add(new GradientStop(RGB.FromBrush(brush).Color, offset));
            return this;
        }
        public RadialGradientBrush Build() => new()
        {
            Center = CenterPoint,
            GradientOrigin = GradientOrigin,
            RadiusX = RadiusX,
            RadiusY = RadiusY,
            GradientStops = new GradientStopCollection(GradientStops),
            SpreadMethod = SpreadMethod,
            MappingMode = MappingMode
        };
    }
    public abstract class TempGradientBrush
    {
        public BrushMappingMode MappingMode { get; protected set; }
        public GradientSpreadMethod SpreadMethod { get; protected set; }
        public List<GradientStop> GradientStops { get; protected set; } = [];
    }
    public class TempSolidColorBrush
    {
        private Color _color = Colors.Transparent;
        public TempSolidColorBrush Color(Color color)
        {
            _color = color;
            return this;
        }
        public TempSolidColorBrush Color(string hex) => Color(RGB.FromString(hex).Color);
        public SolidColorBrush Build() => new(_color);
    }
}