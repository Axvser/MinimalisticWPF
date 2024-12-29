using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Theme
{
    public interface IThemeApplied
    {
        public bool IsThemeChanging { get; set; }
        public Type? CurrentTheme { get; set; }
        public void OnThemeChanging(Type? oldTheme, Type newTheme);
        public void OnThemeChanged(Type? oldTheme, Type newTheme);
    }
}
