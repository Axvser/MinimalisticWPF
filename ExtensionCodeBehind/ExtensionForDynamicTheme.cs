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
        public static T ApplyGlobalTheme<T>(this T source) where T : IThemeApplied
        {
            DynamicTheme.Awake();
            if (!DynamicTheme.GlobalInstance.Contains(source))
            {
                DynamicTheme.GlobalInstance.Add(source);
            }
            return source;
        }
        public static T ApplyTheme<T>(this T source, Type attributeType, TransitionParams param) where T : class
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
            return source;
        }
    }
}
