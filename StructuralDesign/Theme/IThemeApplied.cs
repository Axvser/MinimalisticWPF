using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Theme
{
    public interface IThemeApplied
    {
        public Type NowTheme { get; set; }
        public void BeforeThemeChanged();
        public void AfterThemeChanged();
    }
}
