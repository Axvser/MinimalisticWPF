using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MinimalisticWPF
{
    /// <summary>
    /// Transition execution unit
    /// <para>StartTransition()</para>
    /// <para>StopTransition()</para>
    /// </summary>
    public interface IExecutableTransition
    {
        /// <summary>
        /// Starting transitions
        /// </summary>
        public void StartTransition();
        /// <summary>
        /// Terminating a transition
        /// </summary>
        /// <param name="IsUnsafeStoped">Whether to terminate the Usafe transition</param>
        public void StopTransition(bool IsUnsafeStoped = false);
    }
}
