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

        protected static Treehugger Instance
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

        public static void ExitGame()
        {
            Instance.Exit();
        }

        public static void RunGame()
        {
            Instance.Run();
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

            /* 
             * Since deserialization happens on a separate thread we need to
             * pre-load the transparent sprites, so concurrent drawing doesn't overwrite the sprite.
             */
            AssetManager.GetSprite("*test-wall-sprite2@10x5");
        }

        protected override void UnloadContent()
        {
            gameStateManager.ResetAll();
        }
    }
}
