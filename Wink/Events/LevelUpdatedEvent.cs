using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
