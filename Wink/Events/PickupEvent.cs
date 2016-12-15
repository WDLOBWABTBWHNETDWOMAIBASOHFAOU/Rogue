﻿using System;

namespace Wink
{
    class PickupEvent : Event
    {
        public Item item { get; set; }
        public Player player { get; set; }
        public GameObjectGrid target { get; set; }

        public PickupEvent(Sender sender) : base(sender)
        {

        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            player.mouseSlot.AddTo(item,target);
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
