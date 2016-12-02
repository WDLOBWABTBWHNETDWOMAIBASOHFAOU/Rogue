using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class RemoteServer : Server
    {
        private LocalClient client;

        public override void AddLocalClient(LocalClient localClient)
        {
            client = localClient;
        }

        public override void Send(Event e)
        {
            //Serialize and send the event over TCP connection.
        }
    }
}
