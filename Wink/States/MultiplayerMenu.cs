using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class MultiplayerMenu : GameObjectList
    {
        private class OnlineTab : GameObjectList
        {   
            public OnlineTab()
            {
                Point screen = GameEnvironment.Screen;
                Table availableHosts = new Table();
                availableHosts.AddColumn();
                availableHosts.AddColumn();

                availableHosts.AddRow(new List<object>() { "test", "192.168.153.1" });
                availableHosts.AddRow(new List<object>() { "test2", "192.168.153.2" });

                availableHosts.Position = new Vector2((screen.X - availableHosts.Width)/2, 50);
                Add(availableHosts);
            }
        }

        private class LANTab : GameObjectList
        {
            Button connectButton;
            TextField ipAddress;
            TextField userName;
            Button hostButton;

            public LANTab()
            {
                Point screen = GameEnvironment.Screen;
                SpriteFont arial26 = GameEnvironment.AssetManager.GetFont("Arial26");

                //Create a text field for the ip address
                ipAddress = new TextField(arial26, Color.Red);
                ipAddress.Position = new Vector2((screen.X - ipAddress.Width) / 2, 50);
                ipAddress.Editable = true;
                Add(ipAddress);

                //Create a text field for the user name
                userName = new TextField(arial26, Color.Red);
                userName.Position = new Vector2((screen.X - userName.Width) / 2, 125);
                userName.Editable = true;
                Add(userName);

                //Create a button to start connecting.
                connectButton = new Button("button", "Connect", arial26, Color.Black);
                connectButton.Action = () =>
                {
                    GameEnvironment.GameSettingsManager.SetValue("user_name", userName.Text);
                    GameEnvironment.GameSettingsManager.SetValue("server_ip_address", ipAddress.Text);
                    GameSetupState gss = GameEnvironment.GameStateManager.GetGameState("gameSetupState") as GameSetupState;
                    gss.InitializeGameMode(GameSetupState.GameMode.MultiplayerClient);
                    GameEnvironment.GameStateManager.SwitchTo("gameSetupState");
                };
                //Create a button to start hosting.
                hostButton = new Button("button", "Host a Game", arial26, Color.Black);
                hostButton.Action = () =>
                {
                    GameEnvironment.GameSettingsManager.SetValue("user_name", userName.Text);
                    GameSetupState gss = GameEnvironment.GameStateManager.GetGameState("gameSetupState") as GameSetupState;
                    gss.InitializeGameMode(GameSetupState.GameMode.MultiplayerHost);
                    GameEnvironment.GameStateManager.SwitchTo("gameSetupState");
                };
                
                int x = (screen.X - hostButton.Width - connectButton.Width - 25) / 2;

                hostButton.Position = new Vector2(x + connectButton.Width + 25, 200);
                Add(hostButton);

                connectButton.Position = new Vector2(x, 200);
                Add(connectButton);
            }
        }

        Button backButton;

        public MultiplayerMenu()
        {
            SpriteFont arial26 = GameEnvironment.AssetManager.GetFont("Arial26");
            
            //Define two tabs
            TabField.Tab onlineTab = new TabField.Tab("Online", new OnlineTab());
            TabField.Tab lanTab = new TabField.Tab("LAN", new LANTab());

            Point screen = GameEnvironment.Screen;
            //Make a TabField and add the two tabs.
            TabField tabField = new TabField(new List<TabField.Tab> { lanTab, onlineTab }, Color.Black, arial26, screen.X, screen.Y - 200);
            Add(tabField);

            //Create a button to go back to the main menu.
            backButton = new Button("button", "Back", arial26, Color.Black);
            backButton.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            };
            backButton.Position = new Vector2(100, screen.Y - 100);
            Add(backButton);
        }
    }
}
