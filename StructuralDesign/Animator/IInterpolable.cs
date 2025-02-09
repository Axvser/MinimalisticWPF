using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IInterpolable
    {
        public List<object?> Interpolate(object? current, object? target, int steps);
    }
}
