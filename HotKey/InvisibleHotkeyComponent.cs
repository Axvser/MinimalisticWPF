using MinimalisticWPF.StructuralDesign.HotKey;

namespace MinimalisticWPF.HotKey
{
    internal class InvisibleHotkeyComponent(uint modifierKeys, uint triggerKeys) : IHotKeyComponent
    {
        public uint VirtualModifiers { get; set; } = modifierKeys;
        public uint VirtualKeys { get; set; } = triggerKeys;

        private event HotKeyEventHandler? handlers;
        public virtual event HotKeyEventHandler Handler
        {
            add { handlers += value; }
            remove { handlers -= value; }
        }

        public void Invoke()
        {
            handlers?.Invoke(null, new HotKeyEventArgs(VirtualModifiers, VirtualKeys));
        }
        public void Covered()
        {

        }
    }
}
