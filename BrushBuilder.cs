using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class BrushBuilder
    {
        public static TempLinearColorBrush Linear() => new();
        public static LinearGradientBrush Linear(GradientStopCollection stops) => new(stops);
        public static LinearGradientBrush Linear(GradientStopCollection stops, double angle) => new(stops, angle);
        public static LinearGradientBrush Linear(Color startColor, Color endColor, double angle) => new(startColor, endColor, angle);
        public static LinearGradientBrush Linear(GradientStopCollection stops, Point startPoint, Point endPoint) => new(stops, startPoint, endPoint);
        public static LinearGradientBrush Linear(Color startColor, Color endColor, Point startPoint, Point endPoint) => new(startColor, endColor, startPoint, endPoint);

        public static TempRadialColorBrush Radial() => new();
        public static RadialGradientBrush Radial(GradientStopCollection stops) => new(stops);
        public static RadialGradientBrush Radial(Color startColor, Color endColor) => new(startColor, endColor);

        public static TempSolidColorBrush Solid() => new();
    }

    public class TempLinearColorBrush : TempGradientBrush
    {
        private bool _isMappingModeManuallySet;
        public Point StartPoint { get; private set; } = new Point(0, 0);
        public Point EndPoint { get; private set; } = new Point(1, 1);
        public TempLinearColorBrush Absolute()
        {
            MappingMode = BrushMappingMode.Absolute;
            _isMappingModeManuallySet = true;
            return this;
        }
        public TempLinearColorBrush RelativeToBoundingBox()
        {
            MappingMode = BrushMappingMode.RelativeToBoundingBox;
            _isMappingModeManuallySet = true;
            return this;
        }
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

        private bool _isMappingModeManuallySet;

        public TempRadialColorBrush Absolute()
        {
            MappingMode = BrushMappingMode.Absolute;
            _isMappingModeManuallySet = true;
            return this;
        }
        public TempRadialColorBrush RelativeToBoundingBox()
        {
            MappingMode = BrushMappingMode.RelativeToBoundingBox;
            _isMappingModeManuallySet = true;
            return this;
        }
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
            UpdateAutoMappingMode();
            return this;
        }
        public TempRadialColorBrush Origin(double x, double y)
        {
            GradientOrigin = new Point(x, y);
            UpdateAutoMappingMode();
            return this;
        }
        public TempRadialColorBrush Radius(double radius)
        {
            RadiusX = RadiusY = radius;
            UpdateAutoMappingMode();
            return this;
        }
        public TempRadialColorBrush Radius(double x, double y)
        {
            RadiusX = x;
            RadiusY = y;
            UpdateAutoMappingMode();
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

        private void UpdateAutoMappingMode()
        {
            if (_isMappingModeManuallySet) return;
            bool isAbsolute = CenterPoint.X > 1 || CenterPoint.Y > 1 ||
                             GradientOrigin.X > 1 || GradientOrigin.Y > 1 ||
                             RadiusX > 1 || RadiusY > 1;
            MappingMode = isAbsolute ? BrushMappingMode.Absolute : BrushMappingMode.RelativeToBoundingBox;
        }

        public RadialGradientBrush Build()
        {
            return new RadialGradientBrush(new GradientStopCollection(GradientStops))
            {
                Center = CenterPoint,
                GradientOrigin = GradientOrigin,
                RadiusX = RadiusX,
                RadiusY = RadiusY,
                MappingMode = MappingMode,
                SpreadMethod = SpreadMethod
            };
        }
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