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
    class HelpMenu : GameObjectList
    {
        Button back;

        public HelpMenu()
        {
            Point screen = GameEnvironment.Screen;

            SpriteFont textFieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
            TextGameObject HelpStuff = new TextGameObject("Arial26", cameraSensitivity: 0, layer: 5, id: "HelpStuff");
            HelpStuff.Text = "Use the WASD keys or the Middle Mouse Button to control the camera.\nA class is chosen by clicking on the class name and using the arrow keys to change the class, press Select Hero to confirm.\nThe I key opens your inventory, the C key opens your skill window.\nYou move your character by clicking on adjacent tiles, you can move diagonally.\nYou attack enemies by clicking on them using the Left Mouse Button.\nPressing Q shows your weapon range, E shows your skill range.\nYou need to equip your skills in the upper hotbar, select them using the number keys.\nUse skill by clicking on the target using the Right Mouse Button.\nYou can drink potions by pressing on them in your inventory using the Right Mouse Button.\nYou can finish a level by walking onto the staircase.";
            HelpStuff.Color = Color.White;
            Add(HelpStuff);


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
