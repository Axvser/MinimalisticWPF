using MinimalisticWPF.StructuralDesign.Theme;

namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ View >>> Under the dark theme, builds a new value for the specified property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class Dark(params object?[] param) : Attribute, IThemeAttribute
    {
        public object?[] Parameters { get; set; } = param;
    }
}
