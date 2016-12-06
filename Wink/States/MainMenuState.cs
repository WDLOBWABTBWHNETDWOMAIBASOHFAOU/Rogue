using Microsoft.Xna.Framework;
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

        public MainMenuState()
        {
            singlePlayerButton = new Button("empty:300:50:25:Blue");
            singlePlayerButton.Position = new Vector2((GameEnvironment.Screen.X - singlePlayerButton.Width) / 2, 540);
            Add(singlePlayerButton);

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
