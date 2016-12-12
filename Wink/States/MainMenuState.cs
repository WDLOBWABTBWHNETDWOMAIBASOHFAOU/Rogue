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
            const int buffer = 50;
            const int buttonWidth = 300;
            int leftx = (GameEnvironment.Screen.X - (buttonWidth * 2 + buffer)) / 2;
            int rightx = leftx + buttonWidth + buffer;

            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("default");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

            singlePlayerButton = new Button("button", "Singleplayer", textfieldFont, Color.Black);
            singlePlayerButton.Position = new Vector2(leftx, 300);
            Add(singlePlayerButton);

            multiPlayerButton = new Button("button", "Multiplayer", textfieldFont, Color.Black);
            multiPlayerButton.Position = new Vector2(rightx, 300);
            Add(multiPlayerButton);

            helpButton = new Button("button", "Help", textfieldFont, Color.Black);
            helpButton.Position = new Vector2(leftx, 375);
            Add(helpButton);

            optionsButton = new Button("button", "Options", textfieldFont, Color.Black);
            optionsButton.Position = new Vector2(rightx, 375);
            Add(optionsButton);

            highscoreButton = new Button("button", "High Scores", textfieldFont, Color.Black);
            highscoreButton.Position = new Vector2(leftx, 450);
            Add(highscoreButton);

            quitButton = new Button("button", "Quit", textfieldFont, Color.Black);
            quitButton.Position = new Vector2(rightx, 450);
            Add(quitButton);
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
