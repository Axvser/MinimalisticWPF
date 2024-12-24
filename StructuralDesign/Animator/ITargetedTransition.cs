using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface ITransitionWithTarget
    {
        /// <summary>
        /// The effect only applies to the specified object
        /// </summary>
        public object? Target { get; set; }
    }
}
