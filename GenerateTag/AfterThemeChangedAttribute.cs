using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] Declaring this feature for a no-argument constructor allows the no-argument function to be called after the theme changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AfterThemeChangedAttribute : Attribute
    {
        public AfterThemeChangedAttribute() { }
    }
}
