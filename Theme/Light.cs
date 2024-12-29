using MinimalisticWPF.StructuralDesign.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class Light : Attribute, IThemeAttribute
    {
        public Light(params object?[] param)
        {
            Parameters = param;
        }

        public object?[] Parameters { get; set; }
    }
}
