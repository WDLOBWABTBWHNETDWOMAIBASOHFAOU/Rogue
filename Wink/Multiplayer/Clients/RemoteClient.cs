using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework;
using static Wink.SerializationHelper;

namespace Wink
{
    public class RemoteClient : Client
    {
        private LocalServer Server
        {
            get { return server as LocalServer; }
        }

        private TcpClient tcpClient;
        private BinaryFormatter binaryFormatter;

        private Thread receivingThread;
        private bool receiving;

        public override Player Player
        {
            get { return Server.Level.Find("player_" + ClientName) as Player; }
        }

        public RemoteClient(LocalServer server, TcpClient tcp) : base(server)
        {
            tcpClient = tcp;
            binaryFormatter = new BinaryFormatter();

            StartReceiving();
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
                NetworkStream s = tcpClient.GetStream();
                if (s.DataAvailable)
                {
                    Event e = Deserialize(s, Server) as Event;
                    Server.IncomingEvent(this, e);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public override void SendPreSerialized(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(tcpClient.GetStream());
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
