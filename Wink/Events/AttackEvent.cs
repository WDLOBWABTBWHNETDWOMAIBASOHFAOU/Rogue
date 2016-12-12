﻿using System;

namespace Wink
{
    [Serializable()]
    public class AttackEvent : Event
    {
        public Living Attacker { get; set; }
        public Living Defender { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            Attacker.Attack(Defender);
            server.LevelChanged();
        }
    }
}
