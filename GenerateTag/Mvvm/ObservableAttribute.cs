using System;
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
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ObservableAttribute : Attribute
    {
        public ObservableAttribute(SetterValidations setterValidation = SetterValidations.Compare) { }
    }
}
