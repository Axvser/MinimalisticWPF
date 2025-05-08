using MinimalisticWPF.TransitionSystem;
using System.Reflection;
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
