using MinimalisticWPF.TransitionSystem;

namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IExecutableTransition
    {
        public TransitionScheduler TransitionScheduler { get; }
        public Task Start(object? target = null);
        public void Stop();
    }
}
