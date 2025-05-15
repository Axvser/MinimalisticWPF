using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.HotKey
{
    internal class LocalHotKeyInjector
    {
        internal static readonly Dictionary<UIElement, HashSet<LocalHotKeyInjector>> Injectors = [];

        internal UIElement _target;

        internal HashSet<Key> _pressedKeys = [];

        internal HashSet<Key> _targetKeys = [];

        internal KeyEventHandler keyEventHandler;

        internal LocalHotKeyInjector(UIElement target, HashSet<Key> keys, KeyEventHandler keyevent)
        {
            _target = target;
            _targetKeys = keys;
            keyEventHandler = keyevent;
            target.Focusable = true;
            target.AddHandler(UIElement.PreviewKeyDownEvent, Receiver, true);
            target.AddHandler(UIElement.PreviewKeyUpEvent, ReleaseReceiver, true);
            target.AddHandler(UIElement.LostFocusEvent, ClearValues, true);
        }

        internal void Invoke(KeyEventArgs e)
        {
            if (_pressedKeys.IsSupersetOf(_targetKeys))
            {
                keyEventHandler.Invoke(_target, e);
                _pressedKeys.Clear();
            }
        }
        internal void Receiver(object sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.Key == Key.System ? e.SystemKey : e.Key);

            Invoke(e);
        }
        internal void ReleaseReceiver(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.Key == Key.System ? e.SystemKey : e.Key);

            Invoke(e);
        }
        internal void ClearValues(object sender, RoutedEventArgs e)
        {
            _pressedKeys.Clear();
        }
    }
}
