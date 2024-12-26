using MinimalisticWPF.StructuralDesign.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class Light : Attribute, IThemeAttribute
    {
        public Light(params object?[] param)
        {
            Parameters = param;
        }
        public Light(BrushTags noselected)
        {
            BrushPackage = noselected;
            ispackagebrush = true;
        }

        private bool ispackagebrush = false;

        internal BrushTags BrushPackage { get; set; }

        public object?[]? Parameters { get; set; }
        public object? Value => ispackagebrush ? LightBrushes.Selector.Select(BrushPackage) : null;
    }
}
