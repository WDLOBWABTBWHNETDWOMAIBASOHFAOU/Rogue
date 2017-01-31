using System;
using System.Collections.Generic;
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

        #region Serialization
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
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //Irrelevant because client->server
        }

        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override bool OnServerReceive(LocalServer server)
        {
            List<GameObject> changed = new List<GameObject>();

            if (item != null)
                changed.Add(item);
            if (player.MouseSlot.Item != null)
                changed.Add(player.MouseSlot.Item);

            player.MouseSlot.AddTo(item, target);

            changed.Add(target);
            changed.Add(player.MouseSlot);
            NonAnimationSoundEvent pickupSound = new NonAnimationSoundEvent("Sounds/CLICK10B", true, player.Id);
            LocalServer.SendToClients(new LevelChangedEvent(changed));
            return true;
        }

        public override bool Validate(Level level)
        {
            return player.MouseSlot.Item == null || target.TypeRestriction.IsInstanceOfType(player.MouseSlot.Item);
        }
    }
}
