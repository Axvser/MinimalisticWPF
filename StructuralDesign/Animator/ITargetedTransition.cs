namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface ITransitionWithTarget
    {
        public WeakReference<object>? TransitionApplied { get; set; }
    }
}
