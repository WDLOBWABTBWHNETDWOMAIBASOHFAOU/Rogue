using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    class SkillBar : GameObjectList
    {
        GameObjectList skills;

        public int Width {get { return skills.Children.Count* (Tile.TileWidth); } }

        public SkillBar(GameObjectList skills) : base()
        {
            this.skills = skills;
            for (int i = 0; i < skills.Children.Count; i++)
            {
                skills.Children[i].Position = new Vector2(i * Tile.TileWidth, 0);
            }
            Add(skills);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            
            //draw highlight on active skill
            foreach (ItemSlot s in skills.Children)
            {
                if (s.SlotItem == (GameWorld.Find(Player.LocalPlayerName)as Player).CurrentSkill && s.SlotItem != null)
                {
                    //draw highlight
                    float highlightStrenght = 0.4f;
                    Texture2D redTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.Blue);
                    Rectangle drawhBox = new Rectangle(camera.CalculateScreenPosition(s).ToPoint(), new Point(s.Sprite.Width, s.Sprite.Width));
                    Color drawRColor = new Color(Color.White, highlightStrenght);
                    spriteBatch.Draw(redTex, null, drawhBox, s.Sprite.SourceRectangle, s.Origin, 0.0f, null, drawRColor, SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}