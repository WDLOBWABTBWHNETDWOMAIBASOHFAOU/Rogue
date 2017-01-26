using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public StatScreen(Player player) : base(350, 300)
        {


            SpriteFont font = GameEnvironment.AssetManager.GetFont("Arial20");

            string Characterclassvalue = "Class";

            Color StatsColor = Color.Blue;

            TextGameObject Character = new TextGameObject("Arial26", cameraSensitivity: 0, layer: 5, id: "Chacacter");
            Character.Text = "Character Attributes  ";
            Character.Color = Color.Black;
            Add(Character);

            TextGameObject Characterclass = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Chacacterclass");
            Characterclass.Text = "Character Class   : " + Characterclassvalue; // later als
            Characterclass.Color = Color.Black;
            Characterclass.Position = new Vector2(0, 315 - 9 * font.MeasureString(Characterclass.Text).Y);
            Add(Characterclass);




            TextGameObject Strenght = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Strenght");
            Strenght.Text = "Strenght       " + player.Strength;
            Strenght.Color = StatsColor;
            Strenght.Position = new Vector2(0, 300 - 7 * font.MeasureString(Strenght.Text).Y);
            Add(Strenght);

            TextGameObject Dexterity = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Dexterity");
            Dexterity.Text = "Dexterity      " + player.Dexterity;
            Dexterity.Color = StatsColor;
            Dexterity.Position = new Vector2(0, 300 - 6 * font.MeasureString(Dexterity.Text).Y);
            Add(Dexterity);

            TextGameObject Intelligence = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Intelligence");
            Intelligence.Text = "Intelligence  " + player.Intelligence;
            Intelligence.Color = StatsColor;
            Intelligence.Position = new Vector2(0, 300 - 5 * font.MeasureString(Intelligence.Text).Y);
            Add(Intelligence);

            TextGameObject Vitality = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Vitality");
            Vitality.Text = "Vitality        " + player.Vitality;
            Vitality.Color = StatsColor;
            Vitality.Position = new Vector2(0, 300 - 4 * font.MeasureString(Vitality.Text).Y);
            Add(Vitality);

            TextGameObject Wisdom = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Wisdom");
            Wisdom.Text = "Wisdom      " + player.Wisdom;
            Wisdom.Color = StatsColor;
            Wisdom.Position = new Vector2(0, 300 - 3 * font.MeasureString(Wisdom.Text).Y);
            Add(Wisdom);

            TextGameObject Luck = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Luck");
            Luck.Text = "Luck           " + player.Luck;
            Luck.Color = StatsColor;
            Luck.Position = new Vector2(0, 300 - 2 * font.MeasureString(Luck.Text).Y);
            Add(Luck);


            if (player.freeStatPoints>0)
            { 
            TextGameObject PointsLeft = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "PointsLeft");
            PointsLeft.Text = "Points Left To Spend :" + player.freeStatPoints;
            PointsLeft.Color = Color.Black;
            PointsLeft.Position = new Vector2(0, 300 - font.MeasureString(PointsLeft.Text).Y);
            Add(PointsLeft);




            Button BStrenght = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BStrenght.Position = new Vector2(0 + font.MeasureString(Strenght.Text).X, 300 - 7 * font.MeasureString(Strenght.Text).Y);
            BStrenght.Action = () =>
            {
                StatIncreaseEvent SIES = new StatIncreaseEvent(player, Stat.strength);
                Server.Send(SIES);
            };
            Add(BStrenght);

         

            Button BDexterity = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BDexterity.Position = new Vector2(0 + font.MeasureString(Dexterity.Text).X, 300 - 6 * font.MeasureString(Dexterity.Text).Y);
            BDexterity.Action = () =>
            {
                StatIncreaseEvent SIED = new StatIncreaseEvent(player, Stat.dexterity);
                Server.Send(SIED);
            };
            Add(BDexterity);


            Button BIntelligence = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BIntelligence.Position = new Vector2(0 + font.MeasureString(Intelligence.Text).X, 300 - 5 * font.MeasureString(Intelligence.Text).Y);
            BIntelligence.Action = () =>
            {
                StatIncreaseEvent SIEI = new StatIncreaseEvent(player, Stat.intelligence);
                Server.Send(SIEI);
            };
            Add(BIntelligence);


            Button BVitality = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BVitality.Position = new Vector2(0 + font.MeasureString(Vitality.Text).X, 300 - 4 * font.MeasureString(Vitality.Text).Y);
            BVitality.Action = () =>
            {
                StatIncreaseEvent SIEV = new StatIncreaseEvent(player, Stat.vitality);
                Server.Send(SIEV);
            };
            Add(BVitality);


            Button BWisdom = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BWisdom.Position = new Vector2(0 + font.MeasureString(Wisdom.Text).X, 300 - 3 * font.MeasureString(Wisdom.Text).Y);
            BWisdom.Action = () =>
            {
                StatIncreaseEvent SIEW = new StatIncreaseEvent(player, Stat.wisdom);
                Server.Send(SIEW);
            };
            Add(BWisdom);


            Button BLuck = new Button("inventory/slot", "  +", font, Color.Blue, 1, "", 0, 1);
            BLuck.Position = new Vector2(0 + font.MeasureString(Luck.Text).X, 300 - 2 * font.MeasureString(Luck.Text).Y);
            BLuck.Action = () =>
            {
                StatIncreaseEvent SIEL = new StatIncreaseEvent(player, Stat.luck);
                Server.Send(SIEL);
            };
            Add(BLuck);
        }
        }

    }
}
