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
            get { return 1; }
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
            int dx = (int)Math.Abs(Attacker.Tile.Position.X - Defender.Tile.Position.X);
            int dy = (int)Math.Abs(Attacker.Tile.Position.Y - Defender.Tile.Position.Y);
            
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach;
        }
    }
}
