using MinimalisticWPF.MoveBehavior;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalisticWPF
{
    /// <summary>
    /// ✨ View >>> Enable the control to have a complete life cycle
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MonoBehaviourAttribute(double MilliSeconds) : Attribute
    {
        public double TimeSpan { get; private set; } = Math.Abs(MilliSeconds);
    }
}
