using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public abstract partial class Living : AnimatedGameObject
    {
        private int timeleft;
        private bool startTimer;
        public bool isTurn;
        public Vector2 realPosition;
        public Vector2 vel;
        public bool moving;

        protected string idleAnimation, moveAnimation, dieAnimation;
        private string dieSound;
        
        public Living(int layer = 0, string id = "", float scale = 1.0f) : base(layer, id, scale)
        {
            SetStats();
            InitAnimation();
            timeleft = 1000;
            moving = false;
            vel = Vector2.Zero;
        }

        public Living(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            timeleft = info.GetInt32("timeleft");
            startTimer = info.GetBoolean("startTimer");
            isTurn = info.GetBoolean("isTurn");

            idleAnimation = info.GetString("idleAnimation");
            moveAnimation = info.GetString("moveAnimation");
            dieAnimation = info.GetString("dieAnimation");
            dieSound = info.GetString("dieSound");

            manaPoints = info.GetInt32("manaPoints");
            healthPoints = info.GetInt32("healthPoints");
            actionPoints = info.GetInt32("actionPoints");
            baseAttack = info.GetInt32("baseAttack");
            strength = info.GetInt32("strength");
            dexterity = info.GetInt32("dexterity");
            intelligence = info.GetInt32("intelligence");
            creatureLevel = info.GetInt32("creatureLevel");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("timeleft", timeleft);
            info.AddValue("startTimer", startTimer);
            info.AddValue("isTurn", isTurn);

            info.AddValue("idleAnimation", idleAnimation);
            info.AddValue("moveAnimation", moveAnimation);
            info.AddValue("dieAnimation", dieAnimation);
            info.AddValue("dieSound", dieSound);
            
            info.AddValue("manaPoints", manaPoints);
            info.AddValue("healthPoints", healthPoints);
            info.AddValue("actionPoints", actionPoints);
            info.AddValue("baseAttack", baseAttack);
            info.AddValue("strength", strength);
            info.AddValue("dexterity", dexterity);
            info.AddValue("intelligence", intelligence);
            info.AddValue("creatureLevel", creatureLevel);
        }

        protected virtual void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {
            //General animations
            idleAnimation = idleColor;
            moveAnimation = "empty:65:65:10:DarkBlue";
            dieAnimation = "empty:65:65:10:LightBlue";
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!moving && position != realPosition) realPosition = position;
            if (moving)
            {
                if (!closeEnough(0.01))
                {
                    position += vel;
                }
                else
                {
                    position = realPosition;
                    vel = Vector2.Zero;
                    moving = false;
                }
            }
            if (healthPoints <= 0)
            {
                startTimer = true;
                DeathFeedback("die", dieSound);
                if (startTimer)
                {
                    if (timeleft <= 0)
                        Death();
                    else
                        timeleft -= gameTime.TotalGameTime.Seconds;
                }
            }
        }
        private bool closeEnough(double margin)
        {
            bool a, b, c, d;
            a = (position.X + vel.X < realPosition.X + margin);
            b = (position.X + vel.X > realPosition.X - margin);
            c = (position.Y + vel.Y < realPosition.Y + margin);
            d = (position.Y + vel.Y > realPosition.Y - margin);
            return (a && b && c && d);
        }

        public void MoveTo(Tile tile)
        {
            float TileX = (tile.TilePosition.X + 1) * Tile.TileWidth;
            float TileY = (tile.TilePosition.Y + 1) * Tile.TileHeight;
            if (!moving)
            {
                position = realPosition;
            }
            if (realPosition.X - TileX <= Tile.TileWidth && realPosition.X - TileX >= -Tile.TileWidth*2)
            {
                if (realPosition.Y - TileY <= Tile.TileHeight && realPosition.Y - TileY >= -Tile.TileHeight)
                {
                    moving = true;
                    realPosition.X = TileX - 0.5f * Tile.TileWidth;
                    realPosition.Y = TileY;
                    vel = (realPosition - position) / 10;
                }
            }
        }
    }
}
