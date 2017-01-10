using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using static Wink.SerializationHelper;

namespace Wink
{
    public class RemoteClient : Client
    {
        TcpClient tcp;
        BinaryFormatter binaryFormatter;
        List<Event> pendingEvents; //For the server

        Thread receivingThread;

        public override Player Player
        {
            get { return (server as LocalServer).Level.Find("player_" + ClientName) as Player; }
        }

        public RemoteClient(LocalServer server, TcpClient tcp) : base(server)
        {
            this.tcp = tcp;
            binaryFormatter = new BinaryFormatter();
            pendingEvents = new List<Event>();
            receivingThread = new Thread(new ThreadStart(Receive));

            StartReceiving();
        }

        public void ProcessInitialEvent()
        {
            if (pendingEvents.Count > 0 && pendingEvents[0] is JoinServerEvent)
            {
                pendingEvents[0].Sender = this;
                pendingEvents[0].OnServerReceive(server as LocalServer);
            }
        }

        public void ProcessEvents()
        {
            foreach(Event e in pendingEvents)
            {
                if (e.Validate((server as LocalServer).Level))
                {
                    e.Sender = this;
                    e.OnServerReceive((LocalServer)server);
                }
            }
        }

        private void StartReceiving()
        {
            receivingThread.Start();
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
                    //Event e = (Event)binaryFormatter.Deserialize(s);
                    Event e = SerializationHelper.Deserialize(s, server as LocalServer, false) as Event;
                    pendingEvents.Add(e);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public override void Send(Event e)
        {
            if (e.Validate((server as LocalServer).Level))
            {
                //Serialize and send event over TCP connection.
                StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables((server as LocalServer), e.GUIDSerialization));
                binaryFormatter.Context = c;
                binaryFormatter.Serialize(tcp.GetStream(), e);
            }
        }

        public override void SendPreSerialized(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(tcp.GetStream());
        }
    }
}
