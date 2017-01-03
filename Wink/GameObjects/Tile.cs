using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    public enum TileType
    {
        Background,
        Floor,
        Wall,
        End
    }

    [Serializable]
    public class Tile : SpriteGameObject, IGameObjectContainer
    {
        public const int TileWidth = 64;
        public const int TileHeight = 64;

        protected TileType type;
        protected bool passable;

        protected GameObject onTile;

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
            if (sprite != null)
            {
                origin = new Vector2(0, sprite.Height - TileHeight);
            }
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

        public void EmptyTile()
        {
            onTile = null;
        }

        public bool IsEmpty()
        {
            return onTile == null;
        }

        public bool PutOnTile(Living living)
        {
            if (onTile == null)
            {
                onTile = living;
                onTile.Parent = this;
                onTile.Position = living.PointInTile.ToVector2();
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (onTile != null)
                onTile.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (type != TileType.Background)
                base.Draw(gameTime, spriteBatch, camera);

            if (onTile != null)
                onTile.Draw(gameTime, spriteBatch, camera);
        }

        public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (onTile != null) onTile.DrawDebug(gameTime, spriteBatch, camera);
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
            if (onTile != null)
                onTile.HandleInput(inputHelper);

            if (TileType == TileType.Floor)
            {
                Action onClick = () =>
                {
                    Player player = GameWorld.Find(p => p is Player) as Player;
                    PlayerMoveEvent pme = new PlayerMoveEvent(player, this);
                    Server.Send(pme);
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            }

            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            GameObject single = Find(del);
            if (single != null)
                result.Add(single);
            return result;
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            if (onTile != null && del.Invoke(onTile))
            {
                return onTile;
            }
            else
            {
                return null;
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
