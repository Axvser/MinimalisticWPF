using MinimalisticWPF.StructuralDesign.Theme;

namespace MinimalisticWPF.Theme
{
    /// <summary>
    /// ✨ View >>> Under the bright theme, builds a new value for the specified property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class Light(params object?[] param) : Attribute, IThemeAttribute
    {
        public object?[] Parameters { get; set; } = param;
    }
}