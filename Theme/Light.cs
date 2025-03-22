﻿using MinimalisticWPF.StructuralDesign.Theme;

namespace MinimalisticWPF.Theme
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class Light(params object?[] param) : Attribute, IThemeAttribute
    {
        public object?[] Parameters { get; set; } = param;
    }
}