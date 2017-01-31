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
        Button creditsButton;
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
            singlePlayerButton.Action = () =>
            {
                GameSetupState gss = GameEnvironment.GameStateManager.GetGameState("gameSetupState") as GameSetupState;
                gss.InitializeGameMode(GameSetupState.GameMode.Singleplayer);
                GameEnvironment.GameStateManager.SwitchTo("gameSetupState");
            };
            singlePlayerButton.Position = new Vector2(leftx, 300);
            Add(singlePlayerButton);

            multiPlayerButton = new Button("button", "Multiplayer", textfieldFont, Color.Black);
            multiPlayerButton.Position = new Vector2(rightx, 300);
            multiPlayerButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("multiplayerMenu");
            };
            Add(multiPlayerButton);

            helpButton = new Button("button", "Help", textfieldFont, Color.Black);
            helpButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("helpMenu");
            };
            helpButton.Position = new Vector2(leftx, 375);
            Add(helpButton);

            optionsButton = new Button("button", "Options", textfieldFont, Color.Black);
            optionsButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("optionsMenu");
            };
            optionsButton.Position = new Vector2(rightx, 375);
            Add(optionsButton);

            creditsButton = new Button("button", "Credits", textfieldFont, Color.Black);
            creditsButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("creditsMenu");
            };
            creditsButton.Position = new Vector2(leftx, 450);
            Add(creditsButton);

            quitButton = new Button("button", "Quit", textfieldFont, Color.Black);
            quitButton.Action = () =>
            {
                Treehugger.ExitGame();
            };
            quitButton.Position = new Vector2(rightx, 450);
            Add(quitButton);
        }
    }
}
