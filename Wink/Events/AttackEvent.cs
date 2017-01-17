using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

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

        #region Serialization
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
        #endregion

        protected override int Cost
        {
            get { return 1; }
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        protected override void DoAction(LocalServer server)
        {
            Attacker.Attack(Defender);
            server.ChangedObjects.Add(Defender);
        }

        protected override bool ValidateAction(Level level)
        {
            Vector2 delta = Defender.Tile.GlobalPosition - Attacker.Tile.GlobalPosition;

            double distance = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));
            double reach = Tile.TileWidth * Attacker.Reach;

            bool result = distance <= reach;
            return result;
        }
    }
}
