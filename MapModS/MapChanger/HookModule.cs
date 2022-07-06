namespace MapChanger
{
    /// <summary>
    /// Generic class for creating hooks when entering a save and destroying them when quitting back to menu.
    /// </summary>
    public abstract class HookModule
    {
        public abstract void Hook();
        public abstract void Unhook();
    }
}
