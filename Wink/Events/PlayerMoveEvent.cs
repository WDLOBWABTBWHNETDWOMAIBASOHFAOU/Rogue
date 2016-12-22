using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
        }

        protected override int Cost
        {
            get { return 1; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            player.MoveTo(tile);
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Position.X - player.Origin.X - tile.Position.X);
            int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - tile.Position.Y);

            // THINGS TO MAKE THINGS WORK
            float TileX = (tile.TilePosition.X + 1) * Tile.TileWidth;
            float TileY = (tile.TilePosition.Y + 1) * Tile.TileHeight;
            int xMovement, yMovement;
            xMovement = Math.Sign((int)(player.Position.X - player.Origin.X - tile.Position.X));
            yMovement = Math.Sign((int)(player.Position.Y - player.Origin.Y - tile.Position.Y));
            bool diagonal = Math.Abs(yMovement) == Math.Abs(xMovement);

            bool canDiagonal = true;
            if (diagonal)
            {
                Tile tile1, tile2;
                TileField tf = level.Find("TileField") as TileField;
                tile1 = tf[tile.TilePosition.X + xMovement, tile.TilePosition.Y] as Tile;
                tile2 = tf[tile.TilePosition.X, tile.TilePosition.Y + yMovement] as Tile;
                if (tile1.TileType == TileType.Wall || tile2.TileType == TileType.Wall)
                {
                    canDiagonal = false;
                }
            }
            // DEAL WITH IT

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame && canDiagonal;
        }
    }
}
