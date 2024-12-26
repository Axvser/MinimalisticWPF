using System;
using System.Collections.Generic;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] The Property in ViewModel will be generated automiclly
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class VMPropertyAttribute : Attribute
    {
        public VMPropertyAttribute() { }
    }
}
