using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class LevelUpdatedEvent : Event
    {
        public Level updatedLevel { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            client.level = updatedLevel;
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }
    }
}
