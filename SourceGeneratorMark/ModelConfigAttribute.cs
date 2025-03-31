using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ ViewModel >> Map the view model to the model
    /// </summary>
    /// <param name="modelName">name of model</param>
    /// <param name="nameSpace">namespace of model</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModelConfigAttribute(string modelName, string nameSpace = "") : Attribute
    {
        public string ModelName { get; internal set; } = modelName;
        public string NameSpace { get; internal set; } = nameSpace;
    }
}
