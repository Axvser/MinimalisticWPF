using MinimalisticWPF.Animator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ITransitionMeta Merge(ICollection<ITransitionMeta> metas);

        internal static ITransitionMeta MergeMetas(ICollection<ITransitionMeta> metas)
        {
            var state = new State()
            {
                StateName = Transition.TempName
            };
            foreach (var meta in metas)
            {
                state.Merge(meta);
            }
            return state;
        }
    }
}
