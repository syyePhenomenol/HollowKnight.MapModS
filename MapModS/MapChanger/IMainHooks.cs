namespace MapChanger
{
    /// <summary>
    /// Interface for doing stuff when entering a save when quitting back to menu.
    /// </summary>
    public interface IMainHooks
    {
        void OnEnterGame();
        void OnQuitToMenu();
    }
}
