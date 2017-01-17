using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class OpenDoorEvent : ActionEvent
    {
        public Door door;
        public OpenDoorEvent(Player player, Door door) : base(player)
        {
            this.door = door;
        }

        public OpenDoorEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            door = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("doorGUID"))) as Door;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("doorGUID", door.GUID.ToString());
            base.GetObjectData(info, context);
        }

        public override bool GUIDSerialization
        {
            get { return false; }
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
            door.Sprite.SheetIndex = (door.Sprite.SheetIndex+1)%2;
            door.open = !door.open;
            player.ComputeVisibility();
            server.ChangedObjects.Add(door);
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Tile.TilePosition.X - door.ParentTile.TilePosition.X);
            int dy = (int)Math.Abs(player.Tile.TilePosition.Y - door.ParentTile.TilePosition.Y);
            if (dx <= 1 && dy <= 1 && player.Tile !=door.ParentTile)
            {
                return true;
            }
            return false;
        }
    }
}
