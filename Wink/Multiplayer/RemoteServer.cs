using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using System;

namespace Wink
{
    public class RemoteServer : Server
    {
        private LocalClient client;

        private TcpClient tcp;
        private BinaryFormatter binaryFormatter;
        private List<Event> pendingEvents; //From the server

        private Thread receivingThread;
        private bool receiving;

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

        private void StartReceiving()
        {
            receivingThread = new Thread(new ThreadStart(Receive));
            receivingThread.Start();
            receiving = true;
        }

        public void StopReceiving()
        {
            receiving = false;
        }

        private void Receive()
        {
            while (receiving)
            {
                NetworkStream s = tcp.GetStream();
                if (s.DataAvailable)
                {
                    Event e = SerializationHelper.Deserialize(s, client) as Event;
                    client.IncomingEvent(e);
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
                StreamingContext c = new StreamingContext(StreamingContextStates.All, new SerializationHelper.Variables(client, e.GetFullySerialized(client.Level)));
                binaryFormatter.Context = c;
                binaryFormatter.Serialize(tcp.GetStream(), e);
            }
        }

        public override void Reset()
        {
            StopReceiving();
            receivingThread.Join();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
