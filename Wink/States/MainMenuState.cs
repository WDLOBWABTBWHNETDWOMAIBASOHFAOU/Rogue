using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("Arial12");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

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
                GameSetupState ps = GameEnvironment.GameStateManager.GetGameState("gameSetupState") as GameSetupState;
                ps.InitializeGameMode(GameSetupState.GameMode.Singleplayer);
                GameEnvironment.GameStateManager.SwitchTo("gameSetupState");
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
                GameEnvironment.GameStateManager.SwitchTo("optionsMenu");
            }
            else if (highscoreButton.Pressed)
            {

            }
            else if (quitButton.Pressed)
            {
                Game1.QuitGame();
            }
        }
    }
}
