using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface ITargetedTransition
    {
        public object? Target { get; set; }
    }
}
