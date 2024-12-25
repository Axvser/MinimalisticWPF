using System;
using System.Collections.Generic;

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class VMPropertyAttribute : Attribute
    {
        public VMPropertyAttribute() { }
    }
}
