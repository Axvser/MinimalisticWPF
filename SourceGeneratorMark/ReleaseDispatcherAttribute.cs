using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] when the setter in ViewModel invoked,it will check if the instance should be released → ObjectPool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ReleaseDispatcherAttribute() : Attribute
    {

    }
}
