using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class GameOverState : GameObjectList
    {
        //TODO: (see below)
        //GameOver overlay
        //show score / acievements / unlocked content

        Button mainMenuButton;
        Button restartButton;
        Button quitButton;

        GameSetupState.GameMode gameMode;
        public GameSetupState.GameMode GameMode { get { return gameMode; } set { gameMode = value; } }

        public GameOverState()
        {
            const int buffer = 50;
            const int buttonWidth = 300;
            int leftx = (GameEnvironment.Screen.X - (buttonWidth * 2 + buffer)) / 2;
            int rightx = leftx + buttonWidth + buffer;
            int centerx = (GameEnvironment.Screen.X - buttonWidth ) / 2;
            int buttonOffSett= 75;

            SpriteFont defaultFont = GameEnvironment.AssetManager.GetFont("Arial12");
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

            TextGameObject gameOverMessage = new TextGameObject("Arial26");
            gameOverMessage.Text = "GameOver";
            gameOverMessage.Color = Color.Red;
            gameOverMessage.Position = new Vector2(centerx, 200);
            Add(gameOverMessage);

            mainMenuButton = new Button("button", "MainMenu", textfieldFont, Color.Black);
            mainMenuButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            };
            mainMenuButton.Position = new Vector2(centerx, 300);
            Add(mainMenuButton);

            restartButton = new Button("button", "Restart", textfieldFont, Color.Black);
            restartButton.Action = () =>
            {
                GameSetupState gss = GameEnvironment.GameStateManager.GetGameState("gameSetupState") as GameSetupState;
                gss.InitializeGameMode(gameMode);
                GameEnvironment.GameStateManager.SwitchTo("gameSetupState");
            };
            restartButton.Position = new Vector2(centerx, mainMenuButton.Position.Y + buttonOffSett);
            Add(restartButton);

            quitButton = new Button("button", "Quit", textfieldFont, Color.Black);
            quitButton.Action = () =>
            {
                Treehugger.QuitGame();
            };
            quitButton.Position = new Vector2(centerx, restartButton.Position.Y + buttonOffSett);
            Add(quitButton);

        }
    }
}
