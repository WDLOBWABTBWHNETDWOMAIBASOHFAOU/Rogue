using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class TextTile : Tile
    {
        protected string text;
        protected TextBox textBox;
        public TextTile(string text, string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "", float cameraSensitivity = 1) : base(assetname, tp, layer, id, cameraSensitivity)
        {
            this.text = text;
            textBox = new TextBox(this.text, 1, "HUD/emptybar", "Arial26", TilePosition.X * TileWidth, TilePosition.Y * TileHeight, new Vector2(TilePosition.X * TileWidth, TileHeight * TilePosition.Y), 1);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            textBox.Draw(gameTime, spriteBatch, camera);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            textBox.Update(gameTime);
        }
    }
}
