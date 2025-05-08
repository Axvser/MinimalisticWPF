using System.Windows;

namespace MinimalisticWPF.StructuralDesign.Move
{
    public interface IExecutableMove
    {
        public void Start(FrameworkElement target);
        public void Stop(FrameworkElement target);
    }
}
