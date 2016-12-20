using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        public Player Player { get; set; }
        public Tile Tile { get; set; }

        public PlayerMoveEvent(Sender sender) : base(sender)
        {

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
            Player.MoveTo(Tile);
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(Player.Position.X - Player.Origin.X - Tile.Position.X);
            int dy = (int)Math.Abs(Player.Position.Y - Player.Origin.Y - Tile.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame;
        }
    }
}
