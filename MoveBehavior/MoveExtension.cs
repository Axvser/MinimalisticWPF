using MinimalisticWPF.StructuralDesign.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.MoveBehavior
{
    public enum RenderTimes
    {
        DesignTime,
        RunTime,
        AnyTime,
        None
    }

    public static class MoveBehaviorExtension
    {
        internal static PropertyInfo RenderTransformPropertyInfo = typeof(UIElement).GetProperty("RenderTransform") ?? throw new ArgumentNullException("Can not get RenderTransform PropertyInfo");

        public static void BeginMove(this FrameworkElement target, IExecutableMove tractionMeta)
        {
            tractionMeta.Start(target);
        }
    }
}
