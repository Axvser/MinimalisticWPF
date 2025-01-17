using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Pool
{
    public interface IPoolApplied
    {
        public void RunReusing();
        public void RunReused();
        public bool RunCanRelease();
        public void RunReleasing();
        public void RunReleased();
    }
}
