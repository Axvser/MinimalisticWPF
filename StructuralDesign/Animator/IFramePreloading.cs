using MinimalisticWPF.Animator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// Allows the instance to preload the frames used to describe the transition effect
    /// </summary>
    public interface IFramePreloading
    {
        /// <summary>
        /// A sequence of frames is precomputed to accelerate animation construction
        /// </summary>
        public void PreLoad(TransitionParams transitionParams);
    }
}
