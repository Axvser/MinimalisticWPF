using MinimalisticWPF.Animator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public static class ExtensionForFrameworkElement
    {
        public static T TransitionToParentSize<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newWidth = Parent.Width * rate;
            var newHeight = Parent.Height * rate;
            element.Transition()
                .SetProperty(x => x.Width, newWidth)
                .SetProperty(x => x.Height, newHeight)
                .SetParams(set)
                .Start();
            return element;
        }
        public static T TransitionToParentHeight<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newHeight = Parent.Height * rate;
            element.Transition()
                .SetProperty(x => x.Height, newHeight)
                .SetParams(set)
                .Start();
            return element;
        }
        public static T TransitionToParentWidth<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newWidth = Parent.Width * rate;
            element.Transition()
                .SetProperty(x => x.Width, newWidth)
                .SetParams(set)
                .Start();
            return element;
        }
    }
}
