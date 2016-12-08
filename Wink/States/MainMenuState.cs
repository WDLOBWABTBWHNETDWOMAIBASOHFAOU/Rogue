using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class MainMenuState : GameObjectList
    {
        Button singlePlayerButton;
        Button multiPlayerButton;

        Button helpButton;
        Button optionsButton;
        Button highscoreButton;
        Button quitButton;

        public MainMenuState()
        {
            int buffer = 50;
            int buttonWidth = 300;
            int leftx = (GameEnvironment.Screen.X - (buttonWidth * 2 + buffer)) / 2;
            int rightx = leftx + buttonWidth + buffer;

            singlePlayerButton = new Button("empty:"+ buttonWidth + ":50:25:Blue");
            singlePlayerButton.Position = new Vector2(leftx, 300);
            Add(singlePlayerButton);

            multiPlayerButton = new Button("empty:"+ buttonWidth + ":50:25:Green");
            multiPlayerButton.Position = new Vector2(rightx, 300);
            Add(multiPlayerButton);

            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");
            TextField tf = new TextField(defaultFont, Color.Red);
            tf.Position = new Vector2(50, 50);
            Add(tf);
            
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (singlePlayerButton.Pressed)
            {
                PlayingState ps = GameEnvironment.GameStateManager.GetGameState("playingState") as PlayingState;
                ps.InitializeGameMode(PlayingState.GameMode.Singleplayer);
                GameEnvironment.GameStateManager.SwitchTo("playingState");
            }
        }
    }
}
