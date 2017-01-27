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

        protected override int Cost
        {
            get { return Living.BaseActionCost / 2; }
        }

        protected override void DoAction(LocalServer server, HashSet<GameObject> changedObjects)
        {
            door.Sprite.SheetIndex = (door.Sprite.SheetIndex + 1) % 2;
            door.Open();

            AddVisibleTiles(server.Level, changedObjects);
            player.ComputeVisibility();
            AddVisibleTiles(server.Level, changedObjects);

            changedObjects.Add(door);
        }

        protected override bool ValidateAction(Level level)
        {
            //Check if player is within reach.
            int dx = (int)Math.Abs(player.Tile.Position.X - door.Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - door.Tile.Position.Y);

            return dx <= Tile.TileWidth && dy <= Tile.TileHeight;
        }
    }
}
