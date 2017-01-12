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
            // can get a bit iffy at longer reaches, might need refining depending on max reach used ingame. allot better than nothing
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
            int steps = 1, longSideX,longSideY,shortSideX=0,shortSideY=0;
            float longSide,shortSide;           

            if(Math.Abs(aVec.X) == Math.Abs(aVec.Y))//check diagonal
            {
                for (int i=0;i<= Math.Abs(aVec.Y); i++)
                {
                    int x = currentTile.TilePosition.X + (int)(aVec.X / (Math.Abs(aVec.X))) * i;
                    int y = currentTile.TilePosition.Y + (int)(aVec.Y / (Math.Abs(aVec.Y))) * i;

                    Tile check = grid[x, y] as Tile;
                    if (!check.Passable)
                    {
                        return true;
                    }
                }
            }
            else // check other options
            {
                //this loop checks the tiles along the longer axis, incremented according to its relation to the shorter axis
                int currentLong;
                int checkToLong;

                if (Math.Abs(aVec.X) > Math.Abs(aVec.Y)) //determine which axis is the longest and aply correct mutipliers
                {
                    longSide = aVec.X;
                    shortSide = aVec.Y;
                    longSideX = 0;
                    longSideY = 1;
                }
                else
                {
                    longSide = aVec.Y;
                    shortSide = aVec.X;
                    longSideX = 1;
                    longSideY = 0;
                }
                //correction for negative axis, needed to correct for rounding flaws
                if(aVec.X < 0)
                {
                    shortSideX = 1;
                }
                if (aVec.Y < 0)
                {
                    shortSideY = 1;
                }

                //setting up correct short axis increment
                if (shortSide != 0)
                { nextPointVec = (aVec / Math.Abs(shortSide))/2; }
                else
                { nextPointVec = (aVec / Math.Abs(longSide))/2; }//means the target is on x or y axis relative to attacker, results in every loop checking 1 tile.

                //walk trough dirrect route and check if tiles are passable
                while (currentTile != enemyTile)
                {
                    float nextX = (playerTile.TilePosition.X + nextPointVec.X * steps);
                    float nextY = (playerTile.TilePosition.Y + nextPointVec.Y * steps);

                    //first tile of the next increment
                    nextLineTile = grid[(int)(nextX + (nextPointVec.X *longSideX)), (int)(nextY + (nextPointVec.Y*longSideY))] as Tile;
                    //last tile of the current increment
                    checkToTile = grid[(int)(nextX - (nextPointVec.X * shortSideX*longSideX)), (int)(nextY - (nextPointVec.Y * shortSideY*longSideY))] as Tile;

                    // setting up next input for the forloop, wallking allong the longest axis
                    if (Math.Abs(aVec.X) > Math.Abs(aVec.Y))
                    {
                        currentLong = currentTile.TilePosition.X;
                        checkToLong = checkToTile.TilePosition.X;
                    }
                    else
                    {
                        currentLong = currentTile.TilePosition.Y;
                        checkToLong = checkToTile.TilePosition.Y;
                    }

                    if (!checkToTile.Passable)
                    { return true; }

                    for (int i = (currentLong); i != (checkToLong); i += (int)(longSide / (Math.Abs(longSide))))
                    {
                        Tile check = grid[i*longSideY + currentTile.TilePosition.X*longSideX, i*longSideX + currentTile.TilePosition.Y*longSideY] as Tile;
                        if( check == enemyTile)
                        { return false; }//reached enemy whithout obstruction so path is not blocked

                        if (!check.Passable)
                        { return true; }//
                    }

                    if (checkToTile == enemyTile)//falls out for loop so extra check
                    { return false; }//reached enemy whithout obstruction so path is not blocked

                    currentTile = nextLineTile;
                    steps++;
                }
            }
            return false;               
        }
    }
}
