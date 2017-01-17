using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class NextLevelEvent : Event
    {
        public NextLevelEvent()
        {
        }

        public NextLevelEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override bool OnServerReceive(LocalServer server)
        {
            Level level = new Level(server.LevelIndex + 1);
            List<GameObject> playerlist = server.Level.FindAll(obj => obj is Player);
            for (int p = 1; p <= playerlist.Count; p++)
            {
                Player player = playerlist[p - 1] as Player;
                player.MoveTo(level.Find("StartTile" + p) as Tile);
            }
            foreach (Player p in playerlist)
                p.ComputeVisibility();

            server.Level = level;
            return true;
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
