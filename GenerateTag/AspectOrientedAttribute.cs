using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AspectOrientedAttribute : Attribute
    {
        public AspectOrientedAttribute() { }
    }
}
