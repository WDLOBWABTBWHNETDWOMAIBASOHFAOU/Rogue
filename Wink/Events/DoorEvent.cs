using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{ 
    [Serializable]
    public class DoorEvent : ActionEvent
    {
        private Door door;
        
        public DoorEvent(Door door, Player player) : base(player)
        {
            this.door = door;
        }

        #region Serialization
        public DoorEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            door = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("doorGUID"))) as Door;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("doorGUID", door.GUID.ToString());
        }
        #endregion

        public override bool GUIDSerialization
        {
            get
            {
                return true;
            }
        }

        protected override int Cost
        {
            get { return Living.BaseActionCost/2; }
        }

        protected override void DoAction(LocalServer server)
        {
            door.Sprite.SheetIndex = (door.Sprite.SheetIndex + 1) % 2;
            door.Open();
            player.ComputeVisibility();
            server.ChangedObjects.Add(door);
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Tile.Position.X - door.Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - door.Tile.Position.Y);

            return dx <= Tile.TileWidth && dy <= Tile.TileHeight;
        }
    }
}
