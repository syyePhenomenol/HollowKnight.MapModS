namespace MapChanger
{
    /// <summary>
    /// Base class for creating hooks when entering a save and destroying them when quitting back to menu.
    /// </summary>
    internal abstract class HookModule
    {
        internal abstract void Hook();
        internal abstract void Unhook();
    }
}
