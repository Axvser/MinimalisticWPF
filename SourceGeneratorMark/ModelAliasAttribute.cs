using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ ViewModel >> An alias in the model
    /// </summary>
    /// <param name="alias"></param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ModelAliasAttribute(string alias) : Attribute
    {
        public string Alias { get; internal set; } = alias;
    }
}
