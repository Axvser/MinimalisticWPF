using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Move
{
    public interface IMoveMeta
    {
        public List<Point> Anchors { get; }
        public TransitionParams TransitionParams { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offset, int frameCount);
    }
}
