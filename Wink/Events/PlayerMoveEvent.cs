using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;
        private int cost;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
            cost = 1;
        }

        protected override int Cost
        {
            get { return cost; }
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

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            if (theSame)
            {
                cost = player.ActionPoints;
                // other things that happen on skip turn (?)          
            }
            return withinReach;
        }
    }
}
