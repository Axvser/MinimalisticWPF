namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ View >>>  Apply this attribute to a control can generate dependency properties accroding to it's ViewModel
    /// </summary>
    /// <param name="Name">the name of viewmodel's type</param>
    /// <param name="NameSpaceValidation">the namespace of viewmodel</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DataContextConfigAttribute(string Name, string NameSpaceValidation = "") : Attribute
    {
        public string Name { get; private set; } = Name;
        public string NameSpaceValidation { get; private set; } = NameSpaceValidation;
    }
}