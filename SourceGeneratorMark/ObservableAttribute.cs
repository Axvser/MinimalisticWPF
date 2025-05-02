namespace MinimalisticWPF.SourceGeneratorMark
{
    public enum Validations : int
    {
        None = 0,
        Compare = 1,
        CustomIntercept = 2
    }

    /// <summary>
    /// ✨ ViewModel >> The corresponding observable Property is automatically generated
    /// </summary>
    /// <param name="Validate">Select how the property is validated when updates occur</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ObservableAttribute(Validations Validate = Validations.Compare) : Attribute
    {
        public Validations SetterValidation { get; private set; } = Validate;
    }
}