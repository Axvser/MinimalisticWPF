using MinimalisticWPF.StructuralDesign.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    public static class MoveBehaviorExtension
    {
        public static void BeginMove(this FrameworkElement target, IExecutableTraction tractionMeta)
        {
            tractionMeta.Tractive(target);
        }
    }
}
