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
        Button back;

        class OnlineTab : GameObjectList
        {   
        }

        class LANTab : GameObjectList
        {
            Button connectButton;

            public LANTab()
            {
                Point screen = GameEnvironment.Screen;
                SpriteFont buttonFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");

                //Create a text field for the ip address
                TextField ipAddress = new TextField(buttonFont, Color.Red);
                ipAddress.Position = new Vector2((screen.X - ipAddress.Width) / 2, screen.Y / 2);
                ipAddress.Editable = true;
                Add(ipAddress);

                //Create a button to start connecting.
                connectButton = new Button("empty:300:50:25:Green", "Connect", buttonFont, Color.White);
                connectButton.Position = new Vector2((screen.X - connectButton.Width) / 2, screen.Y - 300);
                Add(connectButton);
            }

            public override void HandleInput(InputHelper inputHelper)
            {
                base.HandleInput(inputHelper);

                if (connectButton.Pressed)
                {
                    //Connect here...
                }
            }
        }

        public MultiplayerMenu()
        {
            SpriteFont tabTitleFont = GameEnvironment.AssetManager.GetFont("TextFieldFont");
            
            //Define two tabs
            TabField.Tab onlineTab = new TabField.Tab("Online", new OnlineTab());
            TabField.Tab lanTab = new TabField.Tab("LAN", new LANTab());

            Point screen = GameEnvironment.Screen;
            //Make a TabField and add the two tabs.
            TabField tabField = new TabField(new List<TabField.Tab> { lanTab, onlineTab }, Color.Black, tabTitleFont, screen.X, screen.Y - 200);
            Add(tabField);

            //Create a button to go back to the main menu.
            back = new Button("empty:150:50:25:Magenta", "Back", tabTitleFont, Color.White);
            back.Position = new Vector2(100, screen.Y - 100);
            Add(back);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (back.Pressed)
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            }
        }
    }
}
