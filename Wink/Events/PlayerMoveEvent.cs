using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;
        private int cost;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
            cost = 1;
        }

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

        public override bool GUIDSerialization
        {
            get { return true; }
        }

        protected override int Cost
        {
            get { return cost; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void DoAction(LocalServer server)
        {
            server.ChangedObjects.Add(player.Tile);
            server.ChangedObjects.Add(tile);
            player.MoveTo(tile);
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Tile.Position.X - tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - tile.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            if (theSame)
            {
                cost = player.ActionPoints;
                // other things that happen on skip turn (?)          
            }
            return withinReach;
        }
    }
}
