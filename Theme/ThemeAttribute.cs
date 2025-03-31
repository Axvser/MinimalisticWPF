using MinimalisticWPF.StructuralDesign.Theme;

namespace MinimalisticWPF.Theme
{
    /// <summary>
    /// ✨ View >>> Derived classes can make properties change to a specified value under a particular topic
    /// </summary>
    /// <param name="param">The parameters needed to build the new value</param>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class ThemeAttribute(params object?[] param) : Attribute, IThemeAttribute
    {
        public object?[] Parameters { get; set; } = param;
    }
}
