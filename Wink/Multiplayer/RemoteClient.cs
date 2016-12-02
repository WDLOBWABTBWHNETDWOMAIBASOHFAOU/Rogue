using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class RemoteClient : Client
    {
        public RemoteClient(Server server) : base(server)
        {
        }

        public override void Send(Event e)
        {
            //Serialize and send event over TCP connection.
        }
    }
}
