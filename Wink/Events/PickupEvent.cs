using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class PickupEvent : Event
    {
        private Item item;
        private Player player;
        private ItemSlot target;

        public PickupEvent(Item item, Player player, ItemSlot target) : base()
        {
            this.item = item;
            this.player = player;
            this.target = target;
        }

        public PickupEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            item = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("itemGUID"))) as Item;
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
            target = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("targetGUID"))) as ItemSlot;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("itemGUID", item != null ? item.GUID.ToString() : Guid.Empty.ToString());
            info.AddValue("playerGUID", player.GUID.ToString());
            info.AddValue("targetGUID", target.GUID.ToString());
            base.GetObjectData(info, context);
        }

        public override bool GUIDSerialization
        {
            get { return true; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            player.MouseSlot.AddTo(item, target);
            server.ChangedObjects.Add(item);
            server.ChangedObjects.Add(target);
        }

        public override bool Validate(Level level)
        {
            return player.MouseSlot.Item == null || target.TypeRestriction.IsInstanceOfType(player.MouseSlot.Item);
        }
    }
}
