using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class AttackEvent : ActionEvent
    {
        public Living Attacker
        {
            get { return player; }
        }
        public Living Defender { get; set; }

        public AttackEvent(Player attacker, Living defender) : base(attacker)
        {
            Defender = defender;
        }

        public AttackEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Defender = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("DefenderGUID"))) as Living;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("DefenderGUID", Defender.GUID.ToString());
            base.GetObjectData(info, context);
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
