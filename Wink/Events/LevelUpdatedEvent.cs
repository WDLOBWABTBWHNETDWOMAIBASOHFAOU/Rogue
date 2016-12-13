using System;

namespace Wink
{
    [Serializable()]
    public class LevelUpdatedEvent : Event
    {
        public Level updatedLevel { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            client.Level = updatedLevel;
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }
    }
}
