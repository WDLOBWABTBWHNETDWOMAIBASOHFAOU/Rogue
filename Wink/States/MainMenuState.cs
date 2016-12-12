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

            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("default");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            singlePlayerButton = new Button("empty:"+ buttonWidth + ":50:25:Green", "Singleplayer", textfieldFont, Color.Red);
            singlePlayerButton.Position = new Vector2(leftx, 300);
            Add(singlePlayerButton);

            multiPlayerButton = new Button("empty:"+ buttonWidth + ":50:25:Green", "Multiplayer", textfieldFont, Color.Red);
            multiPlayerButton.Position = new Vector2(rightx, 300);
            Add(multiPlayerButton);

            helpButton = new Button("empty:" + buttonWidth + ":50:25:Green", "Help", textfieldFont, Color.Red);
            helpButton.Position = new Vector2(leftx, 375);
            Add(helpButton);

            optionsButton = new Button("empty:" + buttonWidth + ":50:25:Green", "Options", textfieldFont, Color.Red);
            optionsButton.Position = new Vector2(rightx, 375);
            Add(optionsButton);

            highscoreButton = new Button("empty:" + buttonWidth + ":50:25:Green", "High Scores", textfieldFont, Color.Red);
            highscoreButton.Position = new Vector2(leftx, 450);
            Add(highscoreButton);

            quitButton = new Button("empty:" + buttonWidth + ":50:25:Green", "Quit", textfieldFont, Color.Red);
            quitButton.Position = new Vector2(rightx, 450);
            Add(quitButton);

            /*
            TextField tf = new TextField(textfieldFont, Color.Red);
            tf.Position = new Vector2(50, 50);
            Add(tf);
            */
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
            else if (multiPlayerButton.Pressed)
            {
                GameEnvironment.GameStateManager.SwitchTo("multiplayerMenu");
            }
            else if (helpButton.Pressed)
            {

            }
            else if (optionsButton.Pressed)
            {

            }
            else if (highscoreButton.Pressed)
            {

            }
            else if (quitButton.Pressed)
            {
                
            }
        }
    }
}
