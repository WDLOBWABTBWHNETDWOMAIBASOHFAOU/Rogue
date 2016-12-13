using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Wink
{
    public class RemoteClient : Client
    {
        public RemoteClient(Server server) : base(server)
        {
        }

        public override void Send(Event e)
        {
            //Serialize and send event over TCP connection.
            IFormatter myFormatter = new BinaryFormatter();
            Stream stream = new FileStream("MyTestFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            myFormatter.Serialize(stream, e);
            stream.Close();
        }
    }
}
