using MinimalisticWPF.WeakDelegate;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MinimalisticWPF.HotKey
{
    internal sealed class LocalHotKeyInjector : IDisposable
    {
        internal readonly WeakReference<UIElement> _targetWeakRef;
        internal readonly HashSet<Key> _pressedKeys = new();
        internal readonly HashSet<Key> _targetKeys;

        private readonly WeakDelegate<EventHandler<KeyEventArgs>> _handlers = new();
        internal event EventHandler<KeyEventArgs> KeyEventHandler
        {
            add => _handlers.AddHandler(value);
            remove => _handlers.RemoveHandler(value);
        }

        internal LocalHotKeyInjector(UIElement target, HashSet<Key> keys, EventHandler<KeyEventArgs> handler)
        {
            _targetWeakRef = new WeakReference<UIElement>(target);
            _targetKeys = keys;
            KeyEventHandler += handler;

            if (TryGetTarget(out var element) && element is not null)
            {
                element.Focusable = true;
                element.AddHandler(UIElement.PreviewKeyDownEvent, (KeyEventHandler)OnPreviewKeyDown, true);
                element.AddHandler(UIElement.PreviewKeyUpEvent, (KeyEventHandler)OnPreviewKeyUp, true);
                element.AddHandler(UIElement.LostKeyboardFocusEvent, (RoutedEventHandler)OnLostFocus, true);
            }
        }

        private bool TryGetTarget(out UIElement? target) => _targetWeakRef.TryGetTarget(out target);

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TryGetTarget(out _))
            {
                _pressedKeys.Add(e.Key == Key.System ? e.SystemKey : e.Key);
                CheckInvoke(e);
            }
        }
        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (TryGetTarget(out _))
            {
                _pressedKeys.Remove(e.Key == Key.System ? e.SystemKey : e.Key);
                CheckInvoke(e);
            }
        }
        private void OnLostFocus(object sender, RoutedEventArgs e) => _pressedKeys.Clear();
        private void CheckInvoke(KeyEventArgs e)
        {
            if (_pressedKeys.IsSupersetOf(_targetKeys))
            {
                if (TryGetTarget(out var target))
                {
                    var handlers = _handlers.GetInvocationList();
                    handlers?.Invoke(target, e);
                    _pressedKeys.Clear();
                }
            }
        }

        public void Dispose()
        {
            if (TryGetTarget(out var target) && target is not null)
            {
                target.RemoveHandler(UIElement.PreviewKeyDownEvent, (KeyEventHandler)OnPreviewKeyDown);
                target.RemoveHandler(UIElement.PreviewKeyUpEvent, (KeyEventHandler)OnPreviewKeyUp);
                target.RemoveHandler(UIElement.LostKeyboardFocusEvent, (RoutedEventHandler)OnLostFocus);
            }
            GC.SuppressFinalize(this);
        }
    }
}