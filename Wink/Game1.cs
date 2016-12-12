using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            windowSize = new Point(1024, 586);
            FullScreen = false;

            //This needs to happen when switched to the playingstate(based on what menu option is chosen)
            PlayingState ps = new PlayingState();
            ps.InitializeGameMode(PlayingState.GameMode.Singleplayer);
            gameStateManager.AddGameState("playingState", ps);

            gameStateManager.AddGameState("mainMenuState", new MainMenuState());
            
            gameStateManager.AddGameState("multiplayerMenu", new MultiplayerMenu());

            gameStateManager.SwitchTo("mainMenuState");

            //AssetManager.PlayMusic("Sounds/snd_music");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}
