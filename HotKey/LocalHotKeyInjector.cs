using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.HotKey
{
    internal class LocalHotKeyInjector
    {
        internal static readonly Dictionary<IInputElement, HashSet<LocalHotKeyInjector>> Injectors = [];

        internal IInputElement _target;

        internal HashSet<Key> _pressedKeys = [];

        internal HashSet<Key> _targetKeys = [];

        internal KeyEventHandler keyEventHandler;

        internal LocalHotKeyInjector(IInputElement target, HashSet<Key> keys, KeyEventHandler keyevent)
        {
            _target = target;
            _targetKeys = keys;
            keyEventHandler = keyevent;
            target.KeyDown += Receiver;
            target.KeyUp += ReleaseReceiver;
            target.MouseLeave += MouseLeave;
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
        internal void MouseLeave(object sender, RoutedEventArgs e)
        {
            _pressedKeys.Clear();
        }
    }
}
