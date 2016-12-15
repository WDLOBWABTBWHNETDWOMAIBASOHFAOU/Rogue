using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class JoinedServerEvent : LevelUpdatedEvent
    {
        public JoinedServerEvent(Sender sender) : base(sender)
        {
        }

        public override void OnClientReceive(LocalClient client)
        {
            base.OnClientReceive(client);
            client.LoadPlayerGUI();
        }
    }
}
