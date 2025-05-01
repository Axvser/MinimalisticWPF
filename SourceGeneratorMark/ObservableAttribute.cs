public enum SetterValidations : int
{
    None = 0,
    Compare = 1,
    Custom = 2
}

/// <summary>
/// ✨ ViewModel >> The corresponding observable Property is automatically generated
/// </summary>
/// <param name="SetterValidation">Select how the property is validated when updates occur</param>
/// <param name="Cascades">Changes to this property are shared with other properties</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ObservableAttribute(SetterValidations SetterValidation = SetterValidations.Compare, params string[] Cascades) : Attribute
{
    public SetterValidations SetterValidation { get; private set; } = SetterValidation;
    public string[] Cascades { get; private set; } = Cascades;
}