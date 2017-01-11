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

        protected GameObjectList onTile;

        public Point TilePosition {
            get
            {
                return new Point(
                    (int)Position.X / TileWidth, 
                    (int)Position.Y / TileHeight
                );
            }
        }

        public bool Blocked
        {
            get
            {
                foreach (GameObject go in onTile.Children)
                {
                    if ((go as ITileObject).BlocksTile)
                        return true;
                }
                return false;
            }
        }

        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        public Tile(string assetname = "", TileType tp = TileType.Background, int layer = 0, string id = "", float cameraSensitivity = 1) : base(assetname, layer, id, 0, cameraSensitivity)
        {
            onTile = new GameObjectList();
            onTile.Parent = this;

            type = tp;

            if (sprite != null)
            {
                origin = new Vector2(0, sprite.Height - TileHeight);
            }
        }

        #region Serialization
        public Tile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (TileType)info.GetValue("type", typeof(TileType));
            passable = info.GetBoolean("passable");
            onTile = info.GetValue("onTile", typeof(GameObjectList)) as GameObjectList;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", type);
            info.AddValue("passable", passable);
            info.AddValue("onTile", onTile);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (onTile != null && onTile.GUID == replacement.GUID)
                onTile = replacement as GameObjectList;

            base.Replace(replacement);
        }

        public void Remove(GameObject go)
        {
            onTile.Remove(go);
        }

        public bool IsEmpty()
        {
            return onTile.Children.Count == 0;
        }

        public bool PutOnTile<T>(T tileObject) where T : GameObject, ITileObject
        {
            if (Passable && !Blocked)
            {
                tileObject.Position = tileObject.PointInTile.ToVector2();
                onTile.Add(tileObject);
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            onTile.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (type != TileType.Background)
                base.Draw(gameTime, spriteBatch, camera);
            
            onTile.Draw(gameTime, spriteBatch, camera);
        }

        public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            onTile.DrawDebug(gameTime, spriteBatch, camera);

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
            onTile.HandleInput(inputHelper);

            if (TileType == TileType.Floor)
            {
                Action onClick = () =>
                {
                    Player player = GameWorld.Find("player_" + Environment.MachineName) as Player;
                    PlayerMoveEvent pme = new PlayerMoveEvent(player, this);
                    Server.Send(pme);
                };

                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            }

            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            return onTile.FindAll(del);
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            return onTile.Find(del);
        }

        public TileType TileType
        {
            get { return type; }
        }
    }
}
