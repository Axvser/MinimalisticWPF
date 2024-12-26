using MinimalisticWPF.Animator;
using MinimalisticWPF.StructuralDesign.Theme;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalisticWPF
{
    public static class ExtensionForDynamicTheme
    {
        public static void ApplyGlobalTheme(this object source)
        {
            DynamicTheme.Awake();
            DynamicTheme.GlobalInstance.Add(source);
        }
        public static void ApplyTheme(this object source, Type attributeType, TransitionParams param)
        {
            DynamicTheme.Awake();
            var type = source.GetType();
            if (DynamicTheme.TransitionSource.TryGetValue(type, out var statedic))
            {
                if (statedic.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, param);
                }
            }
        }
    }
}
