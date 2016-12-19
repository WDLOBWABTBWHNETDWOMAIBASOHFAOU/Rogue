using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class Enemy : Living, ClickableGameObject
    {
        TileField grid;
        Bar<Enemy> hpBar;

        public Enemy(Level level, int layer) : base(level, layer, "Enemy")
        {
            level.Add(this);

            grid = level.Find("TileField") as TileField;
            // This is going to have to be replaced with FindAll but seeing as it's not in this branch and I'm kinda lazy,
            // I'm not doing that now. For now, this works.
            Tile ST = grid[GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)] as Tile;
            while (!ST.Passable)
            {
                ST = grid[GameEnvironment.Random.Next(grid.Columns - 1), GameEnvironment.Random.Next(grid.Rows - 1)] as Tile;
            }
            float tileX = (ST.TilePosition.ToVector2().X + 1) * ST.Height - ST.Height / 2;
            float tileY = (ST.TilePosition.ToVector2().Y + 1) * ST.Width;
            Position = new Vector2(tileX, tileY);

            AddHPBar(this);
        }

        void AddHPBar(Enemy enemy)
        {
            enemy = this;  
            SpriteFont textfieldFont = GameEnvironment.AssetManager.GetFont("Arial26");
            
            //Healthbar
            hpBar = new Bar<Enemy>(enemy, e => e.health, enemy.MaxHealth, textfieldFont, Color.Red, 2, "HealthBar",1.0f,1f,false);


        }

        protected override void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {
            base.InitAnimation("empty:65:65:10:Purple");
            PlayAnimation("idle");
        }

        public void GoTo(Player player)
        {
            Tile tempTile = grid[0, 0] as Tile;
            Vector2 selfPos = new Vector2((Position.X + 0.5f*tempTile.Height) / tempTile.Height - 1, Position.Y / tempTile.Width - 1);
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            hpBar.Position = new Vector2(position.X - hpBar.Origin.X, position.Y - Sprite.Height - 25); ;
            hpBar.Update(gameTime);
            

            if (this.isTurn && this.health > 0)
            {
                GoTo(level.Find(p => p.GetType() == typeof(Player)) as Player);
            }
            else
            {
                ActionPoints = 0;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            if (health < MaxHealth && visible)
            {
                hpBar.Draw(gameTime, spriteBatch, camera);
            }
        }

        public void OnClick(Server server, LocalClient sender)
        {
            AttackEvent aE = new AttackEvent(sender);
            aE.Attacker = sender.Player;
            aE.Defender = this;
            server.Send(aE);
        }
    }
}
