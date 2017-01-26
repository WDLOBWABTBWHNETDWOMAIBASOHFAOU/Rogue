using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
     class MiniButton : Button


     {



         public MiniButton(string assetName, string text, SpriteFont font, Color color, int layer = 2, string id = "", int sheetIndex = 0, float scale = 1) : base(assetName, text, font, color, layer, id, sheetIndex, scale)
         {
             this.text = text;
             this.font = font;
             this.color = Color.Black;
         }


         public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
         {
             base.Draw(gameTime, spriteBatch, camera);

             Vector2 stringSize = font.MeasureString(text);
             float x = (GlobalPosition.X + Width / 2) - stringSize.X / 10;
             float y = (GlobalPosition.Y + Height / 2) - stringSize.Y / 10;

             spriteBatch.DrawString(font, text, new Vector2(x / 10, y / 10), color);
         }


         public override int Width
         {
             get
             {
                 return (int)(sprite.Width * scale / 10);
             }
         }

         public override int Height
         {
             get
             {
                 return (int)(sprite.Height * scale / 10);
             }
         }

         public override Rectangle BoundingBox
         {
             get
             {
                 int left = 0;
                 int top = 0;
                 return new Rectangle(left, top, Width, Height);
             }
         }

     }
     
}
