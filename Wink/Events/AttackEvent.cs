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
            get { return Living.BaseActionCost; }
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
            return AbleToHit(Attacker, Defender);
        }

        public static bool AbleToHit(Living att, Living def)
        {
            if (def.Tile.SeenBy.ContainsKey(att))
            {
                //plus a half tile to get the more natural reach area we discussed (melee can also attack 1 tile diagonaly) 

                // GlobalPositon based
                //Vector2 delta = Defender.Tile.GlobalPosition - Attacker.Tile.GlobalPosition;
                //double reach = Tile.TileWidth * Attacker.Reach + Tile.TileWidth/2; 

                //TilePosition based
                Point delta = def.Tile.TilePosition - att.Tile.TilePosition;
                double reach = att.Reach + 0.5f;

                double distance = Math.Sqrt(Math.Pow(delta.X, 2) + Math.Pow(delta.Y, 2));

                bool result = distance <= reach;
                return result;
            }
            return false;
        }
    }
}
