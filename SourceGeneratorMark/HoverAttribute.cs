namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ View >>> Adds a hover-animation behavior for the specified property in the View layer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class HoverAttribute(params string[] propertyNames) : Attribute
    {
        public string[] PropertyNames { get; private set; } = propertyNames;
    }
}