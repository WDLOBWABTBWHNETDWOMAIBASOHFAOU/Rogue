using System;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    public class AttackEvent : ActionEvent
    {
        public Player Attacker { get; set; }
        public Living Defender { get; set; }
        private Vector2 distance;

        public AttackEvent(Player attacker, Living defender) : base(attacker)
        {
            Attacker = attacker;
            Defender = defender;
            distance = Defender.Position - Attacker.Position;
        }

        protected override int Cost
        {
            get { return 1;  }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            Attacker.Attack(Defender);
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(this.distance.X) - Tile.TileWidth/2;
            int dy = (int)Math.Abs(this.distance.Y) - Tile.TileHeight/2;

            double distance = Math.Abs(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double reach = Tile.TileWidth*Attacker.Reach;

            bool withinReach = distance <= reach;
            if (withinReach)
            {
                return !Blocked(Attacker,Defender);
            }

            return false;
        }
        /// <summary>
        /// Checks if the most direct path from attacker to deffender is blocked
        /// </summary>
        /// <returns> returns true if blocked</returns>
        public static bool Blocked(Living att, Living def)
        {
            TileField grid = att.GameWorld.Find("TileField") as TileField;
            Vector2 defPos = (def.Position - def.Origin) / grid.CellHeight;
            Vector2 attPos = (att.Position - att.Origin) / grid.CellHeight;
            Vector2 nextPointVec;
            Tile enemyTile = grid.Get((int)defPos.X,(int)defPos.Y) as Tile;
            Tile playerTile = grid.Get((int)attPos.X, (int)attPos.Y) as Tile;
            Tile currentTile = playerTile;
            Tile nextLineTile;
            Tile checkToTile; 
            
            Vector2 aVec = (enemyTile.TilePosition - playerTile.TilePosition).ToVector2();
            int steps = 1, longSideX,longSideY;
            float longSide,shortSide;           

            if(Math.Abs(aVec.X) == Math.Abs(aVec.Y))//check diagonal
            {
                for(int x = currentTile.TilePosition.X; x != enemyTile.TilePosition.X; x+= (int)(aVec.X / (Math.Abs(aVec.X))))
                {
                    for(int y = currentTile.TilePosition.X;y!=enemyTile.TilePosition.Y; y+= (int)(aVec.Y / (Math.Abs(aVec.Y))))
                    {
                        Tile check = grid[x, y] as Tile;
                        if (check.Passable)
                        {
                            return false;
                        }
                    }
                }
            }
            else 
            {
                if (Math.Abs(aVec.X) > Math.Abs(aVec.Y))
                {
                    longSide = aVec.X;
                    shortSide = aVec.Y;
                    longSideX = 1;
                    longSideY = 0;
                }
                else
                {
                    longSide = aVec.Y;
                    shortSide = aVec.X;
                    longSideX = 0;
                    longSideY = 1;
                }

                if (shortSide != 0)
                { nextPointVec = (aVec / Math.Abs(shortSide))/2; }
                else
                { nextPointVec = (aVec / Math.Abs(longSide))/2; }

                while (currentTile != enemyTile)
                {
                    float nextX = (playerTile.TilePosition.X + nextPointVec.X * steps);
                    float nextY = (playerTile.TilePosition.Y + nextPointVec.Y * steps);

                    nextLineTile = grid[(int)nextX, (int)nextY] as Tile;
                    checkToTile = grid[(int)(nextX - (nextPointVec.X*longSideX)), (int)(nextY - nextPointVec.Y*longSideY)] as Tile;

                    if (!checkToTile.Passable)
                    { return true; }

                    for (int i = (currentTile.TilePosition.X); i != (checkToTile.TilePosition.X); i += (int)(longSide / (Math.Abs(longSide))))
                    {
                        Tile check = grid[i, currentTile.TilePosition.Y] as Tile;
                        if( check == enemyTile)
                        { return false; }

                        if (!check.Passable)
                        { return true; }                    
                    }
                    currentTile = nextLineTile;
                    steps++;
                }
            }
            return false;               
        }
    }
}
