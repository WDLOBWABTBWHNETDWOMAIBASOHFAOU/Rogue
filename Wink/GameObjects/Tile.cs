using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    public enum TileType
    {
        Background,
        Floor,
        Wall
    }

    [Serializable]
    public class Tile : SpriteGameObject, IGameObjectContainer
    {
        public const int TileWidth = 64;
        public const int TileHeight = 64;

        protected TileType type;
        protected bool passable;

        protected GameObjectList onTile;

        //Dictionary containing what Living objects saw this tile this tick and at what distance;
        protected Dictionary<Living, float> seenBy;

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
            seenBy = new Dictionary<Living, float>();
            onTile = new GameObjectList();
            onTile.Parent = this;
            type = tp;
            visible = false;

            if (sprite != null)
                origin = new Vector2(0, sprite.Height - TileHeight);
        }

        #region Serialization
        public Tile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (TileType)info.GetValue("type", typeof(TileType));
            passable = info.GetBoolean("passable");
            onTile = info.GetValue("onTile", typeof(GameObjectList)) as GameObjectList;
            seenBy = info.GetValue("seenBy", typeof(Dictionary<Living, float>)) as Dictionary<Living, float>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", type);
            info.AddValue("passable", passable);
            info.AddValue("onTile", onTile);
            info.AddValue("seenBy", seenBy);
            base.GetObjectData(info, context);
        }
        #endregion

        public void SeenBy(Living viewer, float distance)
        {
            if (seenBy.ContainsKey(viewer))
                seenBy[viewer] = distance;
            else
                seenBy.Add(viewer, distance);

            if (viewer is Player)
                Visible = true;
        }

        public override void Replace(GameObject replacement)
        {
            if (onTile != null && onTile.GUID == replacement.GUID)
                onTile = replacement as GameObjectList;

            base.Replace(replacement);
        }

        public void Remove(GameObject go)
        {
            onTile.Children.Remove(go);
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
            seenBy.Clear();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (type != TileType.Background)
                base.Draw(gameTime, spriteBatch, camera);
            
            if (Visible)
            {
                foreach (GameObject go in onTile.Children)
                    if (!(go is Living) || seenBy.Count(obj => obj.Key is Player) > 0)
                        onTile.Draw(gameTime, spriteBatch, camera);

                Texture2D blackTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.Black);
                float min = 0.75f;
                foreach (KeyValuePair<Living, float> kvp in seenBy)
                {
                    if (kvp.Key is Player)
                    {
                        float p = kvp.Value / kvp.Key.ViewDistance;
                        min = p < min ? p : min;
                    }
                }
                Rectangle drawBox = new Rectangle(camera.CalculateScreenPosition(this).ToPoint(), new Point(sprite.Width, sprite.Height));
                Color drawColor = new Color(Color.White, min);
                spriteBatch.Draw(blackTex, null, drawBox, sprite.SourceRectangle, origin, 0.0f, new Vector2(scale), drawColor, SpriteEffects.None, 0.0f);
            }
        }

        public override void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            if (debugTags.ContainsKey("ExitConnectionPoint"))
            {
                string[] coord = debugTags["ExitConnectionPoint"].Split(',');
                TileField tf = parent as TileField;
                Tile t = tf.Get(int.Parse(coord[0]), int.Parse(coord[1])) as Tile;
                if (t != null)
                    Line.DrawLine(spriteBatch, camera.CalculateScreenPosition(this), camera.CalculateScreenPosition(t), Color.Red);
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
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
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
