namespace MinimalisticWPF.StructuralDesign.HotKey
{
    public interface IHotKeyComponent
    {
        public uint VirtualModifiers { get; set; }
        public uint VirtualKeys { get; set; }
        public event HotKeyEventHandler Handler;
        public void Invoke();
        public void Covered();
    }
}
