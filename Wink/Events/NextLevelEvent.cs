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

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            Level level = new Level(server.LevelIndex + 1);
            List<GameObject> playerlist = server.Level.FindAll(obj => obj is Player);
            foreach (GameObject obj in playerlist)
            {
                level.Add(obj);
                (obj as Player).MoveTo(level.Find("startTile") as Tile);
            }
            server.Level = level;
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
