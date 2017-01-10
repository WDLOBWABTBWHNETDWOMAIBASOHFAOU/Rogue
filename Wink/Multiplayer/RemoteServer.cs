using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    public class RemoteServer : Server
    {
        private LocalClient client;

        private TcpClient tcp;
        private BinaryFormatter binaryFormatter;
        private List<Event> pendingEvents; //From the server

        private Thread receivingThread;

        public Level Level
        {
            get { return client.Level; }
        }

        public RemoteServer(LocalClient client)
        {
            this.client = client;

            string ip = GameEnvironment.GameSettingsManager.GetValue("server_ip_address");
            tcp = new TcpClient(ip, 29793);

            receivingThread = new Thread(new ThreadStart(Receive));
            binaryFormatter = new BinaryFormatter();
            pendingEvents = new List<Event>();

            StartReceiving();
            ReallySend(new JoinServerEvent(client.ClientName));
        }

        public override void Update(GameTime gameTime)
        {
            ProcessEvents();
        }

        public void ProcessEvents()
        {
            foreach (Event e in pendingEvents)
            {
                if (e.Validate(client.Level))
                {
                    e.OnClientReceive(client);
                }
            }
            pendingEvents.Clear();
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
                    System.Diagnostics.Debug.WriteLine("data is available");
                    Event e = SerializationHelper.Deserialize(s, client, false) as Event; 
                    //Event e = (Event)binaryFormatter.Deserialize(s);
                    pendingEvents.Add(e);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        protected override void ReallySend(Event e)
        {
            if (e.Validate(client.Level))
            {
                //Serialize and send the event over TCP connection.
                StreamingContext c = new StreamingContext(StreamingContextStates.All, new SerializationHelper.Variables(client, e.GUIDSerialization));
                binaryFormatter.Context = c;
                binaryFormatter.Serialize(tcp.GetStream(), e);
            }
        }
    }
}
