using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF.TransitionSystem.Basic
{
    internal class TransitionAtom
    {
        public event Action? Updated;
        public int Duration { get; set; } = 0;
        public void Invoke()
        {
            Updated?.Invoke();
        }
    }
}
