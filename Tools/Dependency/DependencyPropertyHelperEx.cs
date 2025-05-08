using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.Tools.Dependency
{
    public static class DependencyPropertyHelperEx
    {
        public static bool IsPropertySetInXaml(DependencyObject obj, DependencyProperty dp)
        {
            var valueSource = DependencyPropertyHelper.GetValueSource(obj, dp);
            return valueSource.BaseValueSource switch
            {
                BaseValueSource.Local => true,
                BaseValueSource.Style => true,
                BaseValueSource.ImplicitStyleReference => true,
                _ => false
            };
        }
    }
}
