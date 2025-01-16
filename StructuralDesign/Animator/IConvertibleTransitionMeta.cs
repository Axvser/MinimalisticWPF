using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IConvertibleTransitionMeta
    {
        State ToState();
        TransitionBoard<T> ToTransitionBoard<T>() where T : class;
        TransitionMeta ToTransitionMeta();
    }
}
