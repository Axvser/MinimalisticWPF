namespace MinimalisticWPF.StructuralDesign.Animator
{
    public interface ICompilableTransition
    {
        public IExecutableTransition Compile();
    }
}
