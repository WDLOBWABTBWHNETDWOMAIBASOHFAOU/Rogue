using System;

namespace Wink
{
    [Serializable]
    public class AttackEvent : ActionEvent
    {
        public Player Attacker { get; set; }
        public Living Defender { get; set; }

        public AttackEvent(Player attacker, Living defender) : base(attacker)
        {
            Attacker = attacker;
            Defender = defender;
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
            int dx = (int)Math.Abs(Attacker.Position.X - Defender.Position.X ) - Tile.TileWidth/2;
            int dy = (int)Math.Abs(Attacker.Position.Y -  Defender.Position.Y) - Tile.TileHeight/2;

            double distance = Math.Abs(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
            double reach = Tile.TileWidth*Attacker.Reach;

            bool withinReach = distance <= reach;
            return withinReach;
        }
    }
}
