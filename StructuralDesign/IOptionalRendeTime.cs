using MinimalisticWPF.MoveBehavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign
{
    public interface IOptionalRendeTime
    {
        public RenderTimes RenderTime { get; set; }
    }
}
