#if NET5_0_OR_GREATER

namespace MinimalisticWPF
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ObjectPoolAttribute() : Attribute
    {

    }
}

#endif