﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Theme
{
    public interface IThemeAttribute
    {
        /// <summary>
        /// Arguments to construct new value.
        /// </summary>
        object?[] Parameters { get; }
    }
}
