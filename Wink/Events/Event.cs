using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable()]
    public abstract class Event
    {
        public abstract void OnClientReceive(LocalClient client);
        public abstract void OnServerReceive(LocalServer server);
    }
}
