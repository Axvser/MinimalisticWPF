using MinimalisticWPF.Animator;
using System.Reflection;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// The basic data needed to describe the transition effect
    /// </summary>
    public interface ITransitionMeta
    {
        /// <summary>
        /// A table of parameters describing the details of the animation
        /// </summary>
        public TransitionParams TransitionParams { get; set; }
        /// <summary>
        /// Final value of property
        /// </summary>
        public State PropertyState { get; set; }
        /// <summary>
        /// Ordered frames
        /// </summary>
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; }
    }
}
