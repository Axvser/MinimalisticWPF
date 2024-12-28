using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] This Attribute allows the class to participate in the global dynamic theme switching
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DynamicThemeAttribute : Attribute
    {
        public DynamicThemeAttribute() { }
    }
}
