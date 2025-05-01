using System.Reflection;
using System.Windows;
using MinimalisticWPF.TransitionSystem;

namespace MinimalisticWPF.StructuralDesign.Move
{
    public interface IMoveMeta
    {
        public List<Point> Anchors { get; }
        public TransitionParams TransitionParams { get; set; }
        public List<List<Tuple<PropertyInfo, List<object?>>>> GetNormalFrames(Point offset, int frameCount);
    }
}
