using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class DisarmTrapEvent : ActionEvent
    {
        private Trap trap;
        public DisarmTrapEvent(Player player, Trap trap) : base(player)
        {
            this.trap = trap;
        }

        #region Serialization
        public DisarmTrapEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            trap = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("trapGUID"))) as Trap;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("trapGUID", trap.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //
        }

        protected override int Cost
        {
            get { return Living.BaseActionCost; }
        }

        protected override void DoAction(LocalServer server, HashSet<GameObject> changedObjects)
        {
            changedObjects.Add(player);
            changedObjects.Add(trap);
            trap.disarmTrap(player);
        }

        protected override bool ValidateAction(Level level)
        {
            if (player.Tile == null)
                return false;
            int dx = (int)Math.Abs(player.Tile.Position.X - trap.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - trap.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame;
        }
    }
}
