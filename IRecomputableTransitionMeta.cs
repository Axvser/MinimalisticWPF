using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// The transition units can recompute frames based on the new FPS
    /// </summary>
    public interface IRecomputableTransitionMeta
    {
        /// <summary>
        /// Recalculate frames based on FPS
        /// </summary>
        public List<List<Tuple<PropertyInfo, List<object?>>>> RecomputeFrames(int fps);
    }
}
