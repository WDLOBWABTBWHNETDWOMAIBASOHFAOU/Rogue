using System;

namespace Wink
{
    [Serializable]
    public class AttackEvent : ActionEvent
    {
        public Living Attacker { get; set; }
        public Living Defender { get; set; }

        public AttackEvent(Sender sender) : base(sender)
        {

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
            int dx = (int)Math.Abs(Attacker.Position.X - Defender.Position.X);
            int dy = (int)Math.Abs(Attacker.Position.Y -  Defender.Position.Y);
            
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach;
        }
    }
}
