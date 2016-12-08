using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public enum TileType
    {
        Background,
        Normal,
        Wall
    }

    public class Tile : SpriteGameObject, ClickableGameObject
    {
        public const int TileWidth = 65;
        public const int TileHeight = 65;

        protected TileType type;

        public Point TilePosition { get { return new Point((int)Position.X / TileWidth, (int)Position.Y / TileHeight); } }

        public Tile() : base("")
        {

        }

        public Tile(string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "") : base(assetname, layer, id)
        {
            type = tp;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (type == TileType.Background)
            {
                return;
            }
            base.Draw(gameTime, spriteBatch, camera);
        }

        public TileType TileType
        {
            get { return type; }
        }

        public virtual void OnClick(Server server)
        {
            PlayerMoveEvent pme = new PlayerMoveEvent();
            pme.Player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            pme.Tile = this;

            float TileX = (TilePosition.X + 1) * TileWidth;
            float TileY = (TilePosition.Y + 1) * TileHeight;

            if (pme.Player.Position.X - TileX <= TileWidth && pme.Player.Position.X - TileX >= -TileWidth * 2)
            {
                if (pme.Player.Position.Y - TileY <= TileHeight && pme.Player.Position.Y - TileY >= -TileHeight)
                {
                    server.Send(pme);                    
                }
            }
        }
    }
}
