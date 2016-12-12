using System;

namespace Wink
{
    class PickupEvent : Event
    {
        public Item item { get; set; }
        public Player player { get; set; }
        public GameObjectGrid target { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            player.mouseSlot.AddTo(item,target);
            server.LevelChanged();
        }
    }
}
