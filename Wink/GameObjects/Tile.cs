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

        public virtual void OnClick(Server server, LocalClient sender)
        {
            if( TileType == TileType.Normal)
            {
                PlayerMoveEvent pme = new PlayerMoveEvent(sender);
                pme.Player = sender.Player;
                pme.Tile = this;

                server.Send(pme);
            }
            else if(TileType == TileType.Inventory)
            {
                PickupEvent puEvent = new PickupEvent(sender);
                puEvent.player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                InventoryBox target = Parent as InventoryBox;

                if (target.itemGrid.Get((int)position.X, (int)position.Y) == null)
                {
                    Item newItem = new EmptyItem("empty:65:65:10:Gray");
                    newItem.Position = Position;
                    puEvent.item = newItem;
                }
                else
                {
                    puEvent.item = target.itemGrid.Get((int)position.X, (int)position.Y) as Item;
                }
                puEvent.target = target.itemGrid;
                server.Send(puEvent);
            }
        }
    }
}
