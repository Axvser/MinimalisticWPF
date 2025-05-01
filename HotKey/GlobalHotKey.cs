using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MinimalisticWPF.StructuralDesign.HotKey;
using MinimalisticWPF.HotKey;
#if NETFRAMEWORK
using MinimalisticWPF.FrameworkSupport;
#endif

public static class GlobalHotKey
{
    [DllImport("user32.dll")]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private static IntPtr WindowhWnd = IntPtr.Zero;
    private static HwndSource? source;
    public static bool IsAwaked { get; private set; } = false;

    private static Dictionary<int, IHotKeyComponent> Components { get; set; } = [];
    private static ConcurrentQueue<Tuple<uint, uint, ICollection<HotKeyEventHandler>>> WaitToBeRegisteredInvisible { get; set; } = [];
    private static ConcurrentQueue<Tuple<uint, uint, IHotKeyComponent>> WaitToBeRegisteredVisual { get; set; } = [];

    internal const int WM_HOTKEY = 0x0312;
    private static IntPtr WhileKeyInvoked(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_HOTKEY:
                int id = wParam.ToInt32();

                if (Components.TryGetValue(id, out var component))
                {
                    component.Invoke();
                }

                handled = true;
                break;

        }
        return IntPtr.Zero;
    }

    public static void Awake()
    {
        if (IsAwaked) return;

        WindowhWnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
        if (WindowhWnd != IntPtr.Zero)
        {
            source = HwndSource.FromHwnd(WindowhWnd);
            source.AddHook(new HwndSourceHook(WhileKeyInvoked));
            IsAwaked = true;
            while (WaitToBeRegisteredInvisible.TryDequeue(out var meta))
            {
                Register(meta.Item1, meta.Item2, [.. meta.Item3]);
            }
            while (WaitToBeRegisteredVisual.TryDequeue(out var meta))
            {
#if NETFRAMEWORK
                    var hash = HashCodeExtensions.Combine(meta.Item1, meta.Item2);
#elif NET
                var hash = HashCode.Combine(meta.Item1, meta.Item2);
#endif
                RegisterHotKey(WindowhWnd, hash, meta.Item1, meta.Item2);
                if (Components.TryGetValue(hash, out _))
                {
                    Components[hash] = meta.Item3;
                }
                else
                {
                    Components.Add(hash, meta.Item3);
                }
            }
        }
    }
    public static void Dispose()
    {
        if (!IsAwaked) return;

        foreach (var component in Components)
        {
            UnregisterHotKey(WindowhWnd, component.Key);
        }
        Components.Clear();
        source?.RemoveHook(new HwndSourceHook(WhileKeyInvoked));
        source?.Dispose();
        IsAwaked = false;
    }

    public static int Register(IHotKeyComponent component)
    {
        if (component.VirtualKeys == 0x0000 || component.VirtualModifiers == 0x0000) return -1;

        if (IsAwaked)
        {
#if NETFRAMEWORK
                var id = HashCodeExtensions.Combine(component.VirtualModifiers, component.VirtualKeys);
#elif NET
            var id = HashCode.Combine(component.VirtualModifiers, component.VirtualKeys);
#endif
            UnregisterHotKey(WindowhWnd, id);
            if (Components.TryGetValue(id, out var same))
            {
                Components.Remove(id);
                same.Covered();
            }
            if (RegisterHotKey(WindowhWnd, id, component.VirtualModifiers, component.VirtualKeys))
            {
                Components.Add(id, component);
                return id;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            WaitToBeRegisteredVisual.Enqueue(Tuple.Create(component.VirtualModifiers, component.VirtualKeys, component));
            return 0;
        }
    }
    public static int Register(uint modifiers, uint triggers, params HotKeyEventHandler[] handlers)
    {
        if (modifiers == 0x0000 || triggers == 0x0000) return -1;

        if (IsAwaked)
        {
#if NETFRAMEWORK
                var id = HashCodeExtensions.Combine(modifiers, triggers);
#elif NET
            var id = HashCode.Combine(modifiers, triggers);
#endif
            Unregister(modifiers, triggers);
            var reg = RegisterHotKey(WindowhWnd, id, modifiers, triggers);

            if (reg)
            {
                var component = new InvisibleHotkeyComponent(modifiers, triggers);
                foreach (var handler in handlers)
                {
                    component.Handler += handler;
                }
                Components.Add(id, component);
            }

            return reg ? id : -1;
        }
        else
        {
            WaitToBeRegisteredInvisible.Enqueue(Tuple.Create(modifiers, triggers, handlers as ICollection<HotKeyEventHandler>));
            return 0;
        }
    }
    public static int Register(VirtualModifiers modifierKeys, VirtualKeys triggerKeys, params HotKeyEventHandler[] handlers)
    {
        return Register((uint)modifierKeys, (uint)triggerKeys, handlers);
    }

    public static bool Unregister(uint modifiers, uint triggers)
    {
#if NETFRAMEWORK
            var id = HashCodeExtensions.Combine(modifiers, triggers);
#elif NET
        var id = HashCode.Combine(modifiers, triggers);
#endif
        var ureg = UnregisterHotKey(WindowhWnd, id);
        if (Components.TryGetValue(id, out _))
        {
            Components.Remove(id);
        }
        return ureg;
    }
    public static bool Unregister(VirtualModifiers modifierKeys, VirtualKeys triggerKeys)
    {
        return Unregister((uint)modifierKeys, (uint)triggerKeys);
    }
}
