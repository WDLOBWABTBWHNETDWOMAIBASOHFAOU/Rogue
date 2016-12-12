using Microsoft.Xna.Framework;

namespace Wink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : GameEnvironment
    {

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            screen = new Point(1440, 825);
            //windowSize = new Point(1024, 586);
            windowSize = screen;
            FullScreen = false;

            //This needs to happen when switched to the playingstate(based on what menu option is chosen)
            PlayingState ps = new PlayingState();
            ps.InitializeGameMode(PlayingState.GameMode.Singleplayer);

            gameStateManager.AddGameState("playingState", ps);
            gameStateManager.SwitchTo("playingState");

            /*gameStateManager.AddGameState("titleMenu", new TitleMenuState());
            gameStateManager.AddGameState("helpState", new HelpState());
            gameStateManager.AddGameState("levelMenu", new LevelMenuState());
            gameStateManager.AddGameState("gameOverState", new GameOverState());
            gameStateManager.AddGameState("levelFinishedState", new LevelFinishedState());
            */

            //AssetManager.PlayMusic("Sounds/snd_music");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}
