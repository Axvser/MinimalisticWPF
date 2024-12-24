using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// Allows a class to be an animatable property
    /// </summary>
    public interface IInterpolable
    {
        /// <summary>
        /// When the state machine schedules a transition, it will get an instance of the custom class from this property
        /// </summary>
        public object CurrentValue { get; set; }
        /// <summary>
        /// Describe how a custom class interpolates
        /// </summary>
        public List<object?> Interpolate(object? current, object? target, int steps);
    }
}
