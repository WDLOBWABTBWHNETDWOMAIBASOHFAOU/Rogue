using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Wink
{
    public class RemoteServer : Server
    {
        private LocalClient client;

        public override void AddLocalClient(LocalClient localClient)
        {
            client = localClient;
        }

        public override void Send(Event e)
        {
            //Serialize and send the event over TCP connection.
            StringWriter sw = new StringWriter();

            Assembly assembly = typeof(Event).Assembly;
            IEnumerable<Type> types = assembly.GetTypes().Where(t => t.BaseType == typeof(Event));

            XmlSerializer mySerializer = new XmlSerializer(typeof(Event), null, types.ToArray(), null, "Wink");
            mySerializer.Serialize(sw, e);

            string test = sw.ToString();
            Console.Write(test);
        }
    }
}
