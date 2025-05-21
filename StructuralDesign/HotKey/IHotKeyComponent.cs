using MinimalisticWPF.HotKey;
using System.Windows.Input;

namespace MinimalisticWPF.StructuralDesign.HotKey
{
    public interface IHotKeyComponent
    {
        public uint RecordedModifiers { get; set; }
        public uint RecordedKey { get; set; }
        public event EventHandler<HotKeyEventArgs> HotKeyInvoked;
        public void InvokeHotKey();
        public void CoverHotKey();
    }
}
