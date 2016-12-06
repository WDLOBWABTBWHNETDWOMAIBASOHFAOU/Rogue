using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public abstract class Server
    {
        
        public Server()
        {
        }

        public abstract void Send(Event e);
        public abstract void AddLocalClient(LocalClient localClient);
    }
}
