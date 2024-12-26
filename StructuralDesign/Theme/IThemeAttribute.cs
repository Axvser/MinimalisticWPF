using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Theme
{
    public interface IThemeAttribute
    {
        /// <summary>
        /// Arguments to construct new values. It's no longer limited to Brush.
        /// </summary>
        object?[]? Parameters { get; }
        /// <summary>
        /// The theme is usually about Brush, and Value is the value that the context should get for Brush, or if it's empty, it represents something else, and a new value needs to be constructed using a number of arguments
        /// </summary>
        object? Value { get; }
    }
}
