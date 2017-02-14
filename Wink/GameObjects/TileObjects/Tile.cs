using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public class Tile : SpriteGameObject, IGameObjectContainer, IDeserializationCallback
    {
        public const int TileWidth = 64;
        public const int TileHeight = 48;

        protected TileType type;
        protected bool passable;
        protected bool showNormalReach;
        protected bool showSKillReach;

        protected GameObjectList onTile;

        //Dictionary containing what Living objects saw this tile this tick and at what distance;
        protected Dictionary<Living, float> seenBy;

        public Dictionary<Living, float> SeenBy
        {
            get { return seenBy; }
        }
        public GameObjectList OnTile
        {
            get { return onTile; }
        }
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
        private Dictionary<string, float> tempSeenBy;
        private ILocal tempLocal;
        public Tile(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            type = (TileType)info.GetValue("type", typeof(TileType));
            passable = info.GetBoolean("passable");
            onTile = info.TryGUIDThenFull<GameObjectList>(context, "onTile");

            tempLocal = context.GetVars().Local;
            tempSeenBy = info.GetValue("seenByGUIDs", typeof(Dictionary<string, float>)) as Dictionary<string, float>;
            seenBy = info.GetValue("seenBy", typeof(Dictionary<Living, float>)) as Dictionary<Living, float>;
        }

        public void OnDeserialization(object sender)
        {
            tempSeenBy.OnDeserialization(sender);
            seenBy.OnDeserialization(sender);
            foreach (KeyValuePair<string, float> kvp in tempSeenBy)
            {
                Living l = (tempLocal.GetGameObjectByGUID(Guid.Parse(kvp.Key)) ??
                    onTile.Find(obj => obj.GUID == Guid.Parse(kvp.Key))) as Living;
                seenBy.Add(l, kvp.Value);
            }
            
            tempSeenBy = null;
            tempLocal = null;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationHelper.Variables v = context.GetVars();
            info.AddValue("type", type);
            info.AddValue("passable", passable);

            if (v.FullySerializeEverything || v.FullySerialized.Contains(onTile.GUID))
                info.AddValue("onTile", onTile);
            else
                info.AddValue("onTileGUID", OnTile.GUID.ToString());

            //Serialization of seenBy is split up so only Living objects that need to be fully serialized are.
            Dictionary<string, float> guidBasedSeenBy = new Dictionary<string, float>();
            Dictionary<Living, float> fullSeenBy = new Dictionary<Living, float>();
            foreach (KeyValuePair<Living, float> kvp in seenBy)
            {
                if (v.FullySerializeEverything || v.FullySerialized.Contains(kvp.Key.GUID))
                    fullSeenBy.Add(kvp.Key, kvp.Value);
                else
                    guidBasedSeenBy.Add(kvp.Key.GUID.ToString(), kvp.Value);
            }
            info.AddValue("seenBy", fullSeenBy);
            info.AddValue("seenByGUIDs", guidBasedSeenBy);
            base.GetObjectData(info, context);
        }
        #endregion

        public void IsSeenBy(Living viewer, float distance)
        {
            if (seenBy.ContainsKey(viewer))
                seenBy[viewer] = distance;
            else
                seenBy.Add(viewer, distance);

            if (viewer is Player)
                Visible = true;

            foreach (GameObject go in onTile.Children)
            {
                if (!(go is Living) || seenBy.Count(obj => obj.Key is Player) > 0)
                    go.Visible = true;
                else
                    go.Visible = false;
            }
        }

        public override void Replace(GameObject replacement)
        {
            if (onTile.GUID == replacement.GUID)
                onTile = replacement as GameObjectList;

            onTile.Replace(replacement);

            Living toRemove = null;
            foreach (Living l in seenBy.Keys)
            {
                if (l != replacement && l.GUID == replacement.GUID)
                {
                    toRemove = l;
                    seenBy.Add(replacement as Living, seenBy[l]);
                    break;
                }
            }

            if (toRemove != null)
                seenBy.Remove(toRemove);

            base.Replace(replacement);
        }

        public void RemoveImmediatly(GameObject go)
        {
            onTile.RemoveImmediatly(go);
        }

        public void Remove(GameObject go)
        {
            onTile.Remove(go);
        }

        public bool IsEmpty()
        {
            return onTile.Children.Count == 0;
        }

        public virtual bool PutOnTile<T>(T tileObject) where T : GameObject, ITileObject
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
            float bmin = 0.75f;
            foreach (KeyValuePair<Living, float> kvp in seenBy)
            {
                if (kvp.Key is Player)
                {
                    if (!(type == TileType.Wall && kvp.Key.GlobalPosition.Y < GlobalPosition.Y))
                    { 
                        float p = kvp.Value / kvp.Key.ViewDistance;
                        bmin = p < bmin ? p : bmin;
                    }
                }
            }
            bmin = 1 - bmin;
            DrawColor = new Color(bmin, bmin, bmin);

            if (type != TileType.Background)
                base.Draw(gameTime, spriteBatch, camera);
            
            if (Visible)
            {
                foreach (GameObject go in onTile.Children)
                {
                    if (go is SpriteGameObject)
                    {
                        SpriteGameObject sgo = go as SpriteGameObject;
                        sgo.DrawColor = DrawColor;
                    }
                }
                
                onTile.Draw(gameTime, spriteBatch, camera);

                #region HighlightReach
                foreach (KeyValuePair<Living, float> kvp in seenBy)
                {
                    if (kvp.Key is Player)
                    {
                        //highlight tiles that are whithin reach for the local player
                        //current key is the localplayer and the tile is not a wall or the key's tile
                        if ( kvp.Key.Id == Player.LocalPlayerName && !(TileType == TileType.Wall || kvp.Key.Tile == this))
                        {

                            // whithin normal attack reach of the local player
                            if (showNormalReach  && kvp.Value <= kvp.Key.Reach + 0.5f)
                            {
                                //draw highlight
                                float highlightStrenght = 0.2f;
                                Texture2D redTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.Red);
                                Rectangle drawhBox = new Rectangle(camera.CalculateScreenPosition(this).ToPoint(), new Point(sprite.Width, sprite.Width));
                                Color drawRColor = new Color(Color.White, highlightStrenght);
                                spriteBatch.Draw(redTex, null, drawhBox, sprite.SourceRectangle, origin, 0.0f, new Vector2(scale), drawRColor, SpriteEffects.None, 0.0f);
                            }

                            // whithin skillreach of the local player
                            if (showSKillReach && kvp.Key.CurrentSkill!=null && kvp.Value <= kvp.Key.CurrentSkill.SkillReach + 0.5f)
                            {
                                //draw highlight
                                float highlightStrenght = 0.2f;
                                Texture2D redTex = GameEnvironment.AssetManager.GetSingleColorPixel(Color.Blue);
                                Rectangle drawhBox = new Rectangle(camera.CalculateScreenPosition(this).ToPoint(), new Point(sprite.Width, sprite.Width));
                                Color drawRColor = new Color(Color.White, highlightStrenght);
                                spriteBatch.Draw(redTex, null, drawhBox, sprite.SourceRectangle, origin, 0.0f, new Vector2(scale), drawRColor, SpriteEffects.None, 0.0f);
                            }
                        }
                    }
                }
                #endregion
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

            showNormalReach = false;
            showSKillReach = false;

            if (inputHelper.IsKeyDown(Keys.Q))
                showNormalReach = true;
            if (inputHelper.IsKeyDown(Keys.E))
                showSKillReach = true;

            if (TileType == TileType.Floor)
            {
                Action onLeftClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    PlayerMoveEvent pme = new PlayerMoveEvent(player, this);
                    Server.Send(pme);
                };

                 Action onRightClick = () =>
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    SkillEvent SkE = new SkillEvent(player, this);
                    Server.Send(SkE);
                };
                
                inputHelper.IfMouseLeftButtonPressedOn(this, onLeftClick);
            }

            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            if (del.Invoke(onTile))
                result.Add(onTile);

            result.AddRange(onTile.FindAll(del));
            return result;
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            if (del.Invoke(onTile))
                return onTile;

            return onTile.Find(del);
        }

        public TileType TileType
        {
            get { return type; }
        }

        public override string ToString()
        {
            string result = TilePosition.ToString();
            if (onTile.Children.Count > 0)
            {
                foreach (GameObject go in onTile.Children)
                {
                    result += " " + go + ", ";
                }
                result = result.Substring(0, result.Length - 2);
            }
            return result;
        }
    }
}
