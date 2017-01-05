using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class PickupEvent : Event
    {
        public Item item { get; set; }
        public Player player { get; set; }
        public GameObjectGrid target { get; set; }

        public PickupEvent() : base()
        {
        }

        public PickupEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            item = Server.GetGameObjectByGUID(Guid.Parse(info.GetString("itemGUID"))) as Item;
            player = Server.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
            target = Server.GetGameObjectByGUID(Guid.Parse(info.GetString("targetGUID"))) as GameObjectGrid;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("itemGUID", item.GUID.ToString());
            info.AddValue("playerGUID", player.GUID.ToString());
            info.AddValue("targetGUID", target.GUID.ToString());
            base.GetObjectData(info, context);
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            player.MouseSlot.AddTo(item, target);
            server.LevelChanged();
        }

        public override bool Validate(Level level)
        {
            //TODO: Implement Validation.
            //Within Reach
            //
            return true;
        }
    }
}
