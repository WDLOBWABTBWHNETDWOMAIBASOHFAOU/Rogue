using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wink
{
    public class StatScreen : Window
    {
        public StatScreen(Player player) : base(350, 300)
        {
            SpriteFont font = GameEnvironment.AssetManager.GetFont("Arial20");
            string characterClassValue = player.PlayerType.ToString(); //TODO: option; make it so that based on the 2 (maybe 3) heighest stats a class name is selected
            Color statsColor = Color.Blue;

            TextGameObject character = new TextGameObject("Arial26", cameraSensitivity: 0, layer: 5, id: "Chacacter");
            character.Text = "Character Attributes  ";
            character.Color = Color.Black;
            Add(character);

            TextGameObject characterClass = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: "Chacacterclass");
            characterClass.Text = "Character Class: " + characterClassValue;
            characterClass.Color = Color.Black;
            characterClass.Position = new Vector2(0, 315 - 9 * font.MeasureString(characterClass.Text).Y);
            Add(characterClass);
            
            Stat[] stats = (Stat[])Enum.GetValues(typeof(Stat));
            for (int i = 0; i < stats.Length; i++)
            {
                Stat s = stats[i];
                TextGameObject statText = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: s.ToString());
                statText.Text = s.ToString();
                statText.Color = Color.Blue;
                statText.Position = new Vector2(0, 75 +  i * font.MeasureString(statText.Text).Y);
                Add(statText);
                
                TextGameObject statValueText = new TextGameObject("Arial20", cameraSensitivity: 0, layer: 5, id: s.ToString());
                statValueText.Text = player.GetStat(s).ToString();
                statValueText.Color = Color.Blue;
                statValueText.Position = new Vector2(200, 75 + i * font.MeasureString(statValueText.Text).Y);
                Add(statValueText);

                if (player.freeStatPoints > 0)
                {
                    Button BStrenght = new Button("small_button", "+", font, Color.Blue, 1, "", 0, 1);
                    BStrenght.Position = new Vector2(250, 75 + i * font.MeasureString(statValueText.Text).Y);
                    BStrenght.Action = () =>
                    {
                        StatIncreaseEvent SIES = new StatIncreaseEvent(player, s);
                        Server.Send(SIES);
                    };
                    Add(BStrenght);
                }
            }
        }
    }
}
