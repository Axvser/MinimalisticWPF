﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] enables the target to support aspect-oriented programming based on dynamic proxy
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AspectOrientedAttribute : Attribute
    {
        public AspectOrientedAttribute() { }
    }
}