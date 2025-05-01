#if NETFRAMEWORK
using MinimalisticWPF;
using MinimalisticWPF.StructuralDesign.Animator;
using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
namespace MinimalisticWPF.FrameworkSupport
{
    internal class MetaMergeExtension
    {
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
#endif