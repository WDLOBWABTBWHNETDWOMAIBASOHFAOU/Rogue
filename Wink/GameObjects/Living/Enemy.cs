using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class Enemy : Living
    {
        Bar<Enemy> hpBar;

        public Enemy(int layer, string id = "Enemy") : base(layer, id)
        {
            AddHPBar();
        }

        public void InitPosition()
        {
            TileField grid = GameWorld.Find("TileField") as TileField;

            //First find all passable tiles then select one at random.
            List<GameObject> tileCandidates = grid.FindAll(obj => obj is Tile && (obj as Tile).Passable);
            Tile ST = tileCandidates[GameEnvironment.Random.Next(tileCandidates.Count)] as Tile;

            float tileX = (ST.TilePosition.ToVector2().X + 1) * ST.Height - ST.Height / 2;
            float tileY = (ST.TilePosition.ToVector2().Y + 1) * ST.Width;
            Position = new Vector2(tileX, tileY);

        }

        protected override void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {
            base.InitAnimation("empty:65:65:10:Purple");
            PlayAnimation("idle");
        }

        public void GoTo(Player player)
        {
            TileField grid = player.GameWorld.Find("TileField") as TileField;
            Tile tempTile = grid[0, 0] as Tile;
            Vector2 selfPos = new Vector2((Position.X + 0.5f * tempTile.Height) / tempTile.Height - 1, Position.Y / tempTile.Width - 1);
            Vector2 playPos = new Vector2((player.Position.X + 0.5f * tempTile.Height) / tempTile.Height - 1, player.Position.Y / tempTile.Width - 1);
            List<Tile> path = Pathfinding.ShortestPath(selfPos, playPos, grid);
            if (path.Count > 1)
            {
                MoveTo(path[0]);
                actionPoints--;
            }
            else if (player.Position.X - Position.X <= Tile.TileWidth && player.Position.X - Position.X >= -Tile.TileWidth * 2)
            {
                if (player.Position.Y - Position.Y <= Tile.TileHeight && player.Position.Y - Position.Y >= -Tile.TileHeight)
                {
                    Attack(player);
                    actionPoints--;
                }
            }
        }

        private void AddHPBar()
        {
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");

            //Healthbar
            hpBar = new Bar<Enemy>(this, e => e.Health, e => e.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar", 1.0f, 1f, false);
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
                    //Not sure if it works with multiplayer
                    string ClientName = Environment.MachineName;
                    Player player = GameWorld.Find("player_" + ClientName) as Player;
                    AttackEvent aE = new AttackEvent(player, this);
                    Server.Send(aE);
                };
            
                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

                base.HandleInput(inputHelper);
            }
        }
    }
}