using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] This Attribute enables the class to support aspect-oriented programming based on dynamic proxy
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AspectOrientedAttribute : Attribute
    {
        public AspectOrientedAttribute() { }
    }
}
