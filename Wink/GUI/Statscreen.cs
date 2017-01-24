using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wink;

namespace Wink
{
    public class StatScreen : Window
    {
        public StatScreen(Player player) : base(300, 300)
        {
            int strenghtvalue = 20000000;


            TextGameObject emptyLine = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 3, id: "emptyLine :" + this) : base(300, 350);
            emptyLine.Text = " Fire";
            emptyLine.Color = Color.Blue;
            
           
            

            TextGameObject HAHAHAtest = new TextGameObject("Arial12", positiontext= new Vector2(350,350) ,cameraSensitivity: 0, layer: 7, id: "HAHAHA" + this):base (400,400);
            HAHAHAtest.Text = "         you mama";
            HAHAHAtest.Color = Color.Black;
            Position = new Vector2(2, 2);
            HAHAHAtest.Position = new Vector2(2, 2);

            TextGameObject Strenght = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 5, id: "Buhahahlol");
            Strenght.Text = "Strenght";
            Strenght.Color = Color.Blue;

            TextGameObject armorinfo = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "ArmorValueTypeInfo." + this);
            //Gebruik TextGameObject om hier de stats zoals player.Dexterity aan dit window toe te voegen.
            Add(Strenght);
            Add(HAHAHAtest);
            Add(emptyLine);
            
        }

    }
}
