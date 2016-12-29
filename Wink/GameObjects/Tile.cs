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
        Chest,
        Door,
        Inventory,
        End
    }

    [Serializable]
    public class Tile : SpriteGameObject
    {
        public const int TileWidth = 65;
        public const int TileHeight = 65;

        protected TileType type;
        protected bool passable;

        public Point TilePosition {
            get
            {
                return new Point(
                    (int)Position.X / TileWidth, 
                    (int)Position.Y / TileHeight
                );
            }
        }

        public Tile(string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "", float cameraSensitivity = 1) : base(assetname, layer, id, 0, cameraSensitivity)
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

        public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (debugTags.ContainsKey("ExitConnectionPoint"))
            {
                string[] coord = debugTags["ExitConnectionPoint"].Split(':')[1].Split(',');
                TileField tf = parent as TileField;
                Tile t = tf.Get(int.Parse(coord[0]), int.Parse(coord[1])) as Tile;
                if (t != null)
                {
                    Line.DrawLine(spriteBatch, camera.CalculateScreenPosition(this) + Center, camera.CalculateScreenPosition(t) + Center, Color.Red);
                }

            }
            base.DrawDebug(gameTime, spriteBatch, camera);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (TileType == TileType.Normal)
            {
                Action onClick = () =>
                {
                    Player player = GameWorld.Find(p => p is Player) as Player;
                    PlayerMoveEvent pme = new PlayerMoveEvent(player, this);
                    Server.Send(pme);
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            }
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
    }
}
