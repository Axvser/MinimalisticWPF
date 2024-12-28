using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] Generate a number of constructors based on the difference in the parameter list.The logic inside this method will be executed inside the constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ConstructorAttribute : Attribute
    {
        public ConstructorAttribute() { }
    }
}
