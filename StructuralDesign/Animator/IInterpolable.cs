namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface IInterpolable
    {
        public List<object?> Interpolate(object? current, object? target, int steps);
    }
}
