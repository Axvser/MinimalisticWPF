using MinimalisticWPF.StructuralDesign.HotKey;
using MinimalisticWPF.WeakTypes;

namespace MinimalisticWPF.HotKey
{
    internal class InvisibleHotkeyComponent(uint modifierKeys, uint triggerKeys) : IHotKeyComponent
    {
        public uint RecordedModifiers { get; set; } = modifierKeys;
        public uint RecordedKey { get; set; } = triggerKeys;

        private readonly WeakDelegate<EventHandler<HotKeyEventArgs>> _handlers = new();
        public event EventHandler<HotKeyEventArgs> HotKeyInvoked
        {
            add => _handlers.AddHandler(value);
            remove => _handlers.RemoveHandler(value);
        }

        public void InvokeHotKey()
        {
            var handler = _handlers.GetInvocationList();
            handler?.Invoke(this, new HotKeyEventArgs(RecordedModifiers, RecordedKey));
        }
        public void CoverHotKey()
        {

        }
    }
}
