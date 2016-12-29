using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class PickupEvent : Event
    {
        public Item item { get; set; }
        public Player player { get; set; }
        public ItemSlot target { get; set; }

        public PickupEvent() : base()
        {
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
