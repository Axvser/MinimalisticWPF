using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// Instances can generate read-only animations
    /// </summary>
    public interface ICompilableTransition
    {
        /// <summary>
        /// If the transition is compiled, only the start or end methods are accessible. Modifying the original data does not affect the compiled result
        /// </summary>
        /// <returns>IExecutableTransition</returns>
        public IExecutableTransition Compile();
    }
}
