﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] allows the class to participate in the global dynamic theme switching
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DynamicThemeAttribute : Attribute
    {
        public DynamicThemeAttribute() { }
    }
}