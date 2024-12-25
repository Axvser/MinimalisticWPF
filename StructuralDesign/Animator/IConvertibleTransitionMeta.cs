using MinimalisticWPF.Animator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// Instances are allowed to be converted into three types that describe how transitions are performed
    /// </summary>
    public interface IConvertibleTransitionMeta
    {
        /// <summary>
        /// The data has the lowest level . Its creation and use are usually performed by libraries
        /// </summary>
        State ToState();
        /// <summary>
        /// The data level is second only to TransitionMeta . It is the most common data dependency for startup animations
        /// </summary>
        TransitionBoard<T> ToTransitionBoard<T>() where T : class;
        /// <summary>
        /// The data level is the highest , and it supports functions such as merging and recalculation
        /// </summary>
        TransitionMeta ToTransitionMeta();
    }
}
