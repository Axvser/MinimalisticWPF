using System.Windows.Input;
using System.Windows;
using MinimalisticWPF.HotKey;

public static class LocalHotKey
{
    public static void Register(IInputElement target, KeyEventHandler keyevent, params Key[] keys)
    {
        var hashset = new HashSet<Key>();
        foreach (var key in keys)
        {
            hashset.Add(key);
        }
        var injector = new LocalHotKeyInjector(target, hashset, keyevent);
        if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
        {

            injectorSet.Add(injector);
        }
        else
        {
            LocalHotKeyInjector.Injectors.Add(target, [injector]);
        }
    }
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

    public static int Unregister(IInputElement target, params Key[] keys)
    {
        int count = 0;

        var hashset = new HashSet<Key>();

        foreach (var key in keys)
        {
            hashset.Add(key);
        }

        if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
        {
            List<LocalHotKeyInjector> removed = [];
            foreach (var injector in injectorSet)
            {
                if (hashset.IsSupersetOf(injector._targetKeys))
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
    public static int Unregister(IInputElement target, ICollection<HashSet<Key>> keysgroup)
    {
        int count = 0;

        if (LocalHotKeyInjector.Injectors.TryGetValue(target, out var injectorSet))
        {
            foreach (var keys in keysgroup)
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

    public static void RegisterMainWindow(KeyEventHandler keyevent, params Key[] keys)
    {
        var hashset = new HashSet<Key>();
        foreach (var key in keys)
        {
            hashset.Add(key);
        }
        var injector = new LocalHotKeyInjector(Application.Current.MainWindow, hashset, keyevent);
        if (LocalHotKeyInjector.Injectors.TryGetValue(Application.Current.MainWindow, out var injectorSet))
        {
            injectorSet.Add(injector);
        }
        else
        {
            LocalHotKeyInjector.Injectors.Add(Application.Current.MainWindow, [injector]);
        }
    }
    public static void RegisterMainWindow(HashSet<Key> keys, KeyEventHandler keyevent)
    {
        var injector = new LocalHotKeyInjector(Application.Current.MainWindow, keys, keyevent);
        if (LocalHotKeyInjector.Injectors.TryGetValue(Application.Current.MainWindow, out var injectorSet))
        {

            injectorSet.Add(injector);
        }
        else
        {
            LocalHotKeyInjector.Injectors.Add(Application.Current.MainWindow, [injector]);
        }
    }

    public static int UnregisterMainWindow(params Key[] keys)
    {
        int count = 0;

        var hashset = new HashSet<Key>();

        foreach (var key in keys)
        {
            hashset.Add(key);
        }

        if (LocalHotKeyInjector.Injectors.TryGetValue(Application.Current.MainWindow, out var injectorSet))
        {
            List<LocalHotKeyInjector> removed = [];
            foreach (var injector in injectorSet)
            {
                if (hashset.IsSupersetOf(injector._targetKeys))
                {
                    Application.Current.MainWindow.KeyDown -= injector.Receiver;
                    Application.Current.MainWindow.KeyUp -= injector.ReleaseReceiver;
                    Application.Current.MainWindow.MouseLeave -= injector.MouseLeave;
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

            if (!injectorSet.Any()) LocalHotKeyInjector.Injectors.Remove(Application.Current.MainWindow);
        }

        return count;
    }
    public static int UnregisterMainWindow(HashSet<Key> keys)
    {
        int count = 0;

        if (LocalHotKeyInjector.Injectors.TryGetValue(Application.Current.MainWindow, out var injectorSet))
        {
            List<LocalHotKeyInjector> removed = [];
            foreach (var injector in injectorSet)
            {
                if (keys.IsSupersetOf(injector._targetKeys))
                {
                    Application.Current.MainWindow.KeyDown -= injector.Receiver;
                    Application.Current.MainWindow.KeyUp -= injector.ReleaseReceiver;
                    Application.Current.MainWindow.MouseLeave -= injector.MouseLeave;
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

            if (!injectorSet.Any()) LocalHotKeyInjector.Injectors.Remove(Application.Current.MainWindow);
        }

        return count;
    }
    public static int UnregisterMainWindow()
    {
        var count = 0;

        if (LocalHotKeyInjector.Injectors.TryGetValue(Application.Current.MainWindow, out var injectorSet))
        {
            foreach (var injector in injectorSet)
            {
                Application.Current.MainWindow.KeyDown -= injector.Receiver;
                Application.Current.MainWindow.KeyUp -= injector.ReleaseReceiver;
                Application.Current.MainWindow.MouseLeave -= injector.MouseLeave;
            }
            if (LocalHotKeyInjector.Injectors.Remove(Application.Current.MainWindow))
            {
                count++;
            }
        }

        return count;
    }
}
