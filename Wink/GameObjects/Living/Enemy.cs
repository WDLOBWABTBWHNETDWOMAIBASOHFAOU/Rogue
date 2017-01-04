using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class Enemy : Living
    {
        private Bar<Enemy> hpBar;

        public Enemy(int layer, string id = "Enemy") : base(layer, id)
        {
            AddHPBar();
        }

        public override bool BlocksTile
        {
            get { return Health > 0; }
        }

        protected override void InitAnimation(string idleColor = "empty:64:64:10:Magenta")
        {
            base.InitAnimation("empty:64:64:10:Purple");
            PlayAnimation("idle");
        }

        public void GoTo(Player player)
        {
            TileField tf = player.GameWorld.Find("TileField") as TileField;
            
            int dx = (int)Math.Abs(player.Tile.Position.X - Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - Tile.Position.Y);
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            
            if (withinReach)
            {
                Attack(player);
                actionPoints--;
            }
            else
            {
                PathFinder pf = new PathFinder(tf);
                List<Tile> path = pf.ShortestPath(Tile, player.Tile);
                if (path.Count > 0)
                {
                    MoveTo(path[0]);
                    actionPoints--;
                }
                else
                {
                    Idle();
                }
            }
        }

        private void Idle()
        {
            //TODO: implement idle behaviour (right now for if there is no path to the player, later for if it can't see the player.)
            actionPoints--;
        }

        private void AddHPBar()
        {
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

            //Healthbar
            hpBar = new Bar<Enemy>(this, e => e.Health, MaxHealth, textfieldFont, Color.Red, 2, "HealthBar", 1.0f, 1f, false);
            hpBar.Parent = this;
            hpBar.Position = new Vector2((Width - hpBar.Width - origin.X) / 2, -Height);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isTurn && Health > 0)
            {
                GoTo(GameWorld.Find(p => p is Player) as Player);
            }
            else
            {
                ActionPoints = 0;
            }
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            if (Health < MaxHealth && visible)
            {
                hpBar.Draw(gameTime, spriteBatch, camera);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (Health > 0)
            {
                Action onClick = () => 
                {
                    // correct player when in multiplayer?
                    Player player = GameWorld.Find(p => p is Player) as Player;
                    AttackEvent aE = new AttackEvent(player, this);
                    Server.Send(aE);
                };
            
                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }
    }
}