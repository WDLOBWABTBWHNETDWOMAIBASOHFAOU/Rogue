using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wink
{
    public enum TileType
    {
        Background,
        Normal,
        Wall,
        Inventory
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
