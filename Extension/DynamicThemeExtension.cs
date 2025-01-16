using MinimalisticWPF.Theme;
using MinimalisticWPF.TransitionSystem;

namespace MinimalisticWPF.Extension
{
    public static class DynamicThemeExtension
    {
        public static void ApplyGlobalTheme(this object source)
        {
            DynamicTheme.Awake();
            DynamicTheme.GlobalInstance.Add(source);
        }
        public static void ApplyTheme(this object source, Type attributeType, TransitionParams? param)
        {
            DynamicTheme.Awake();
            var type = source.GetType();
            if (DynamicTheme.TransitionSource.TryGetValue(type, out var statedic))
            {
                if (statedic.TryGetValue(attributeType, out var state))
                {
                    source.BeginTransition(state, param ?? TransitionParams.Theme);
                }
            }
        }
    }
}
