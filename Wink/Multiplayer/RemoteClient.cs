using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Wink
{
    public class RemoteClient : Client
    {
        TcpClient tcp;
        BinaryFormatter binaryFormatter;
        List<Event> pendingEvents; //For the server

        Thread receivingThread;

        public RemoteClient(LocalServer server, TcpClient tcp) : base(server)
        {
            this.tcp = tcp;
            binaryFormatter = new BinaryFormatter();
            pendingEvents = new List<Event>();

            StartReceiving();
        }

        public void ProcessEvents()
        {
            foreach(Event e in pendingEvents)
            {
                if (e.Validate())
                {
                    e.OnServerReceive((LocalServer)server);
                }
            }
        }

        private void StartReceiving()
        {
            receivingThread = new Thread(new ThreadStart(Receive));
        }

        public void StopReceiving()
        {
            receivingThread.Abort();
        }

        private void Receive()
        {
            while (true)
            {
                NetworkStream s = tcp.GetStream();
                if (s.DataAvailable)
                {
                    Event e = (Event)binaryFormatter.Deserialize(s);
                    pendingEvents.Add(e);
                }
            }
        }

        public override void Send(Event e)
        {
            if (e.Validate())
            {
                //Serialize and send event over TCP connection.
                binaryFormatter.Serialize(tcp.GetStream(), e);
            }
        }
    }
}
