#if NET

namespace MinimalisticWPF
{
    /// <summary>
    /// 🧰 > Enables the target to support aspect-oriented programming based on dynamic proxy
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AspectOrientedAttribute() : Attribute
    {

    }
}

#endif