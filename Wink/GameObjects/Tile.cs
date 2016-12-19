using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public enum TileType
    {
        Background,
        Normal,
        Wall,
        Door,
        Inventory
    }

    [Serializable]
    public class Tile : SpriteGameObject, ClickableGameObject
    {
        public const int TileWidth = 65;
        public const int TileHeight = 65;

        protected TileType type;
        protected bool passable;

        public Point TilePosition { get { return new Point((int)Position.X / TileWidth, (int)Position.Y / TileHeight); } }

        public Tile(string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "") : base(assetname, layer, id)
        {
            type = tp;
        }

        public Tile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (TileType)info.GetValue("type", typeof(TileType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", type);
            base.GetObjectData(info, context);
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

        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        public virtual void OnClick(Server server)
        {
            if( TileType == TileType.Normal)
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
            else if(TileType == TileType.Inventory)
            {
                PickupEvent PuE = new PickupEvent();
                PuE.player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                InventoryBox target = this.parent as InventoryBox;

                if (target.itemGrid.Get((int)position.X, (int)position.Y) == null)
                {
                    Item newItem = new EmptyItem("empty:65:65:10:Gray");
                    newItem.Position = this.Position;
                    PuE.item = newItem;
                }
                else
                {
                    PuE.item = target.itemGrid.Get((int)position.X, (int)position.Y) as Item;
                }
                PuE.target = target.itemGrid;
                server.Send(PuE);
            }
        }
    }
}
