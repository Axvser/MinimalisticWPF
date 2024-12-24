using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// A transition unit is allowed to perform merge operations with other transition units
    /// </summary>
    public interface IMergeableTransition
    {
        /// <summary>
        /// Merges multiple transitions with itself
        /// </summary>
        public TransitionMeta Merge<T>(ICollection<T> metas) where T : ITransitionMeta, IMergeableTransition, IRecomputableTransitionMeta;
    }
}
