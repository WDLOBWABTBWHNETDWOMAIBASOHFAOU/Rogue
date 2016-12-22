using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Wink
{
    public class RemoteServer : Server
    {
        private LocalClient client;

        TcpClient tcp;
        BinaryFormatter binaryFormatter;
        List<Event> pendingEvents; //For the server

        Thread receivingThread;

        public RemoteServer(LocalClient client)
        {
            this.client = client;

            string ip = GameEnvironment.GameSettingsManager.GetValue("server_ip_address");
            tcp = new TcpClient(ip, 29793);

            receivingThread = new Thread(new ThreadStart(Receive));
            binaryFormatter = new BinaryFormatter();
            pendingEvents = new List<Event>();
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
        }

        private void StartReceiving()
        {
            
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
                    Event e = (Event)binaryFormatter.Deserialize(s);
                    pendingEvents.Add(e);
                }
            }
        }

        protected override void ReallySend(Event e)
        {
            if (e.Validate(client.Level))
            {
                //Serialize and send the event over TCP connection.
                binaryFormatter.Serialize(tcp.GetStream(), e);
            }
        }
    }
}
