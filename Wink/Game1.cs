﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : GameEnvironment
    {
        private static Game1 instance;

        private Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public static Game1 Instance
        {
            get {
                if (instance == null)
                    instance = new Game1();
                return instance;
            }
        }

        public new static void ApplyResolutionSettings(bool fullscreen = false)
        {
            (Instance as GameEnvironment).ApplyResolutionSettings(fullscreen);
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
            gameStateManager.AddGameState("optionsMenu", new OptionsMenu());
            gameStateManager.SwitchTo("mainMenuState");

            //AssetManager.PlayMusic("Sounds/snd_music");
        }

        protected override void UnloadContent()
        {
        }
    }
}
