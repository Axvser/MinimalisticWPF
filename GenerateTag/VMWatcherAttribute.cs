using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalisticWPF
{
    /// <summary>
    /// [ Source Generator ] Make the method listen for changes to the Property in ViewModel
    /// <para>Requirements</para>
    /// <para>1.The method name needs to include the name of the property or the name of the field that the property corresponds to</para>
    /// <para>2.Contains a unique argument ( WatcherEventArgs e ) to accept the old and new values</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class VMWatcherAttribute : Attribute
    {
        public VMWatcherAttribute() { }
    }
}
