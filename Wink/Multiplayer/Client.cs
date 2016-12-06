using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public abstract class Client
    {
        protected Server server;
        public string clientName { get; set; }

        public Client(Server server)
        {
            this.server = server;
        }

        public abstract void Send(Event e);

    }
}
