using MinimalisticWPF.HotKey;

namespace MinimalisticWPF.StructuralDesign.HotKey
{
    public interface IHotKeyComponent
    {
        public uint RecordedModifiers { get; set; }
        public uint RecordedKey { get; set; }
        public event HotKeyEventHandler? HotKeyInvoked;
        public void InvokeHotKey();
        public void CoverHotKey();
    }
}
