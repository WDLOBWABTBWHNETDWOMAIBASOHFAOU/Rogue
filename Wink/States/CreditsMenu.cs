using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class CreditsMenu : GameObjectList
    {
        Button back;

        public CreditsMenu()
        {
            Point screen = GameEnvironment.Screen;

            SpriteFont textFieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
            TextGameObject CreditsStuff = new TextGameObject("Arial26", cameraSensitivity: 0, layer: 5, id: "CreditsStuff");
            CreditsStuff.Text = "Sjors Derkson - Art director: Sprites and sound\nLuuk Hekkers - GitHub coordinator: Rings and enemies\nDuur Klop: Bosses, level and tiles\nDavid Kook: Players\nNick Mulder: Pathfinder, Trailer video and menus\nStefan Schouten - Project leader: Multiplayer, procedural generation, website, gui, onclick method and debugmode\nStijn Schroevers: Living, Inventory system, items and skills";
            CreditsStuff.Color = Color.White;
            Add(CreditsStuff);

            //Create a button to go back to the main menu.
            back = new Button("button", "Back", textFieldFont, Color.Black);
            back.Action = () =>
            {
                GameEnvironment.GameStateManager.SwitchTo("mainMenuState");
            };
            back.Position = new Vector2(100, screen.Y - 100);
            Add(back);
        }
    }
}
