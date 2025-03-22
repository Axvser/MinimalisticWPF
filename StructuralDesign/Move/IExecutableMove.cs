using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Move
{
    public interface IExecutableMove
    {
        public void Start(FrameworkElement target);
        public void Stop(FrameworkElement target);
    }
}
