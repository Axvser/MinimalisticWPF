using MinimalisticWPF.TransitionSystem;
using MinimalisticWPF.TransitionSystem.Basic;
using System.Reflection;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface ITransitionMeta
    {
        public TransitionParams TransitionParams { get; set; }
        public State PropertyState { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; }
    }
}
