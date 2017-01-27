using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
        }

        #region Serialization
        public PlayerMoveEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            tile = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("tileGUID"))) as Tile;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("tileGUID", tile.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        protected override int Cost
        {
            get
            {
                int mc = Living.BaseActionCost;
                if ((player.EquipmentSlots.Find("bodySlot") as RestrictedItemSlot).SlotItem != null)
                {
                    mc = (int)(mc * ((player.EquipmentSlots.Find("bodySlot") as RestrictedItemSlot).SlotItem as BodyEquipment).WalkCostMod);
                }

                return mc;
            }
        }

        protected override void DoAction(LocalServer server, HashSet<GameObject> changedObjects)
        {
            Tile oldTile = player.Tile;

            AddVisibleTiles(server.Level, changedObjects);
            player.MoveTo(tile);
            player.ComputeVisibility();
            AddVisibleTiles(server.Level, changedObjects);

            changedObjects.Add(oldTile);
            changedObjects.Add(player);
        }

        protected override bool ValidateAction(Level level)
        {
            if (player.Tile == null)
                return false;

            int dx = (int)Math.Abs(player.Tile.Position.X - tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - tile.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame;
        }
    }
}
