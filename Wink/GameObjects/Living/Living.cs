
using Microsoft.Xna.Framework;

namespace Wink
{
    public abstract partial class Living : AnimatedGameObject
    {
        protected Level level;
        private int timeleft;
        private bool startTimer;

        protected string idleAnimation, moveAnimation, dieAnimation;
        string dieSound;
        
        public Living(Level level, int layer = 0, string id = "", float scale = 1.0f) : base(layer, id, scale)
        {
            this.level = level;
            SetStats();
            healthPoints = MaxHP();
            manaPoints = MaxManaPoints();
            InitAnimation();
            timeleft = 1000;
        }

        protected virtual void InitAnimation()
        {//General animations
            idleAnimation = "empty:65:65:10:Magenta";
            moveAnimation = "empty:65:65:10:DarkBlue";
            dieAnimation = "empty:65:65:10:DarkRed";
            LoadAnimation(idleAnimation, "idle", true);
            LoadAnimation(moveAnimation, "move", true, 0.05f);
            LoadAnimation(dieAnimation, "die", false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(healthPoints <= 0)
            {
                startTimer = true;
                DeathFeedback("die",dieSound);
                if (startTimer)
                {
                    if (timeleft <= 0)
                        Death();
                    else
                        timeleft -= gameTime.TotalGameTime.Seconds;
                }
            }
        }


        public void MoveTo(Tile tile)
        {
            float TileX = (tile.TilePosition.X + 1) * Tile.TileWidth;
            float TileY = (tile.TilePosition.Y + 1) * Tile.TileHeight;

            if (position.X - TileX <= Tile.TileWidth && position.X - TileX >= -Tile.TileWidth*2)
            {
                if (position.Y - TileY <= Tile.TileHeight && position.Y - TileY >= -Tile.TileHeight)
                {
                    position.X = TileX - 0.5f * Tile.TileWidth;
                    position.Y = TileY;
                }
            }            
        }
    }
}
