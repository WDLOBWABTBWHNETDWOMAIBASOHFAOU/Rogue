using Microsoft.Xna.Framework;

namespace Wink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Treehugger : GameEnvironment
    {
        private static Treehugger instance;

        private Treehugger()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public static Treehugger Instance
        {
            get {
                if (instance == null)
                    instance = new Treehugger();
                return instance;
            }
        }

        public new static void ApplyResolutionSettings(bool fullscreen = false)
        {
            (Instance as GameEnvironment).ApplyResolutionSettings(fullscreen);
        }

        public static void QuitGame()
        {

            Instance.Exit();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            screen = new Point(1920, 1080);
            windowSize = new Point(960, 540);
            FullScreen = false;

            gameStateManager.AddGameState("gameSetupState", new GameSetupState());
            gameStateManager.AddGameState("playingState", new PlayingState());
            gameStateManager.AddGameState("gameOverState", new GameOverState());
            gameStateManager.AddGameState("mainMenuState", new MainMenuState());
            gameStateManager.AddGameState("multiplayerMenu", new MultiplayerMenu());
            gameStateManager.AddGameState("optionsMenu", new OptionsMenu());
            gameStateManager.SwitchTo("mainMenuState");

            //AssetManager.PlayMusic("Sounds/snd_music");
        }

        protected override void UnloadContent()
        {
            gameStateManager.ResetAll();
        }
    }
}
