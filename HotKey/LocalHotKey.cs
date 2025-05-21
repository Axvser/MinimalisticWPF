using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.HotKey
{
    /// <summary>
    /// 🧰 > Local hotkey registration
    /// <para><see cref="Register"/></para>
    /// <para><see cref="Unregister(UIElement)"/></para>
    /// <para><see cref="Unregister(UIElement, HashSet{Key})"/></para>
    /// </summary>
    public static class LocalHotKey
    {
        private static readonly ConditionalWeakTable<UIElement, HashSet<LocalHotKeyInjector>> _injectors = new();

        public static void Register(UIElement target, HashSet<Key> keys, EventHandler<KeyEventArgs> handler)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var injector = new LocalHotKeyInjector(target, keys, handler);
            var injectorSet = _injectors.GetOrCreateValue(target);
            injectorSet.Add(injector);
        }
        public static int Unregister(UIElement target, HashSet<Key> keys)
        {
            if (_injectors.TryGetValue(target, out var injectorSet))
            {
                int removed = injectorSet.RemoveWhere(i => i._targetKeys.SetEquals(keys));
                if (injectorSet.Count == 0) _injectors.Remove(target);
                return removed;
            }
            return 0;
        }
        public static int Unregister(UIElement target)
        {
            if (_injectors.TryGetValue(target, out var injectorSet))
            {
                int count = injectorSet.Count;
                foreach (var injector in injectorSet) injector.Dispose();
                _injectors.Remove(target);
                return count;
            }
            return 0;
        }
    }
}