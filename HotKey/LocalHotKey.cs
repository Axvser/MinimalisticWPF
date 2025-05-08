using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.HotKey
{
    /// <summary>
    /// 🧰 > Local hotkey registration
    /// <para>Core</para>
    /// <para>- <see cref="Register"/></para>
    /// <para>- <see cref="Unregister(IInputElement,HashSet{Key})"/></para>
    /// <para>- <see cref="Unregister(IInputElement)"/></para>
    /// </summary>
    public static class LocalHotKey
    {
        public static void Register(IInputElement target, HashSet<Key> keys, KeyEventHandler keyevent)
        {
            var injector = new LocalHotKeyInjector(target, keys, keyevent);
            if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
            {

                injectorSet.Add(injector);
            }
            else
            {
                LocalHotKeyInjector.Injectors.Add(target, [injector]);
            }
        }
        public static int Unregister(IInputElement target, HashSet<Key> keys)
        {
            int count = 0;

            if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
            {
                List<LocalHotKeyInjector> removed = [];
                foreach (var injector in injectorSet)
                {
                    if (keys.IsSupersetOf(injector._targetKeys))
                    {
                        target.KeyDown -= injector.Receiver;
                        target.KeyUp -= injector.ReleaseReceiver;
                        target.MouseLeave -= injector.MouseLeave;
                        removed.Add(injector);
                    }
                }

                foreach (var injector in removed)
                {
                    if (injectorSet.Remove(injector))
                    {
                        count++;
                    }
                }

                if (!injectorSet.Any()) LocalHotKeyInjector.Injectors.Remove(target);
            }

            return count;
        }
        public static int Unregister(IInputElement target)
        {
            int count = 0;

            if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
            {
                foreach (var injector in injectorSet)
                {
                    target.KeyDown -= injector.Receiver;
                    target.KeyUp -= injector.ReleaseReceiver;
                    target.MouseLeave -= injector.MouseLeave;
                    count++;
                }
                LocalHotKeyInjector.Injectors.Remove(target);
            }

            return count;
        }
    }
}
