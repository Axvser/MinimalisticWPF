using MinimalisticWPF.StructuralDesign.HotKey;

namespace MinimalisticWPF.HotKey
{
    internal class InvisibleHotkeyComponent(uint modifierKeys, uint triggerKeys) : IHotKeyComponent
    {
        public uint RecordedModifiers { get; set; } = modifierKeys;
        public uint RecordedKey { get; set; } = triggerKeys;

        public virtual event HotKeyEventHandler? HotKeyInvoked;

        public void InvokeHotKey()
        {
            HotKeyInvoked?.Invoke(null, new HotKeyEventArgs(RecordedModifiers, RecordedKey));
        }
        public void CoverHotKey()
        {

        }
    }
}
