using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    /// <summary>
    /// Transition execution unit
    /// </summary>
    public interface IExecutableTransition
    {
        /// <summary>
        /// The scheduler that manages this execution unit
        /// </summary>
        public StateMachine Machine { get; }
        /// <summary>
        /// Starting transitions
        /// </summary>
        public void Start();
        /// <summary>
        /// Terminating a transition
        /// </summary>
        /// <param name="IsUnsafeStoped">Whether to terminate the Usafe transition</param>
        public void Stop(bool IsUnsafeStoped = false);
    }
}
