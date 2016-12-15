using System;

namespace Wink
{
    [Serializable()]
    public class LevelUpdatedEvent : Event
    {
        public Level updatedLevel { get; set; }

        public LevelUpdatedEvent(Sender sender) : base(sender)
        {

        }

        public override void OnClientReceive(LocalClient client)
        {
            client.Level = updatedLevel;
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
