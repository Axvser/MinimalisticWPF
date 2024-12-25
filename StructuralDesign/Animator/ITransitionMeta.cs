using MinimalisticWPF.Animator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// Ordered animation frames
        /// </summary>
        public List<List<Tuple<PropertyInfo, List<object?>>>> FrameSequence { get; set; }
    }
}
