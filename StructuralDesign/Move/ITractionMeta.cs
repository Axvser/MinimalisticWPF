using MinimalisticWPF.TransitionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Move
{
    public interface ITractionMeta
    {
        public int Accuracy { get; set; }
        public List<Point> Anchors { get; }
        public TransitionParams TransitionParams { get; set; }
    }
}
