﻿using System;
using System.Collections.Generic;

namespace MinimalisticWPF
{
    public enum SetterValidations : int
    {
        None = 0,
        Compare = 1,
        Custom = 2
    }

    /// <summary>
    /// [ Source Generator ] The corresponding observable Property is automatically generated
    /// </summary>
    /// <param name="SetterValidation">Select how the property is validated when updates occur</param>
    /// <param name="CanOverride">Select whether the property is overwritable</param>
    /// <param name="CanHover">Select whether to generate hover properties.If enabled, an initial value must be set for the field, and this initial value will affect the initial value set for the dependent property</param>
    /// <param name="Cascades">Changes to this property are shared with other properties</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ObservableAttribute(SetterValidations SetterValidation = SetterValidations.Compare, bool CanOverride = false, bool CanHover = false, params string[] Cascades) : Attribute
    {
        public SetterValidations SetterValidation { get; private set; } = SetterValidation; 
        public bool CanOverride { get; private set; } = CanOverride;
        public bool CanHover { get; private set; } = CanHover;
        public string[] Cascades { get; private set; } = Cascades;
    }
}
