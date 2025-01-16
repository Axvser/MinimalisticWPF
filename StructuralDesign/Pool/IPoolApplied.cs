using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.StructuralDesign.Pool
{
    public interface IPoolApplied
    {
        public void OnReusing();
        public void OnReused();
        public bool CanRelease();
        public void OnReleasing();
        public void OnReleased();
    }
}
