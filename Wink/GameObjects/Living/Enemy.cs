using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class Enemy : Living, IGUIGameObject
    {
        private Bar<Enemy> hpBar;

        public Enemy(int layer, string id = "Enemy") : base(layer, id)
        {
        }

        public Enemy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
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

        public override void MoveTo(Tile t)
        {
            base.MoveTo(t);
            PositionHPBar();
        }

        private void PositionHPBar()
        {
            if (hpBar != null)
                hpBar.Position = Tile.GlobalPosition - new Vector2(Math.Abs(Tile.Width - hpBar.Width) / 2, 0);
        }

        public void InitGUI()
        {
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

            //Healthbar
            hpBar = new Bar<Enemy>(this, e => e.Health, MaxHealth, textfieldFont, Color.Red, 2, "HealthBar", 1.0f, 1f, false);
            PositionHPBar();

            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Add(hpBar);
        }

        public void CleanupGUI()
        {
            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Remove(hpBar);
        }
    }
}