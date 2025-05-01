using MinimalisticWPF.StructuralDesign.Theme;

namespace MinimalisticWPF.SourceGeneratorMark
{
    /// <summary>
    /// ✨ View >>> Adds a theme-animation behavior for the specified property in the View layer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ThemeAttribute(string propertyName, Type themeType, params object?[] themeArguments) : Attribute, IThemeAttribute
    {
        public string PropertyName => propertyName;
        public Type ThemeType => themeType;
        public object?[] Parameters => themeArguments;
    }
}