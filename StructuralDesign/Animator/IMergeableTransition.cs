﻿using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IMergeableTransition
    {
        public ITransitionMeta Merge(ICollection<ITransitionMeta> metas);

#if NET
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
#endif
    }
}
