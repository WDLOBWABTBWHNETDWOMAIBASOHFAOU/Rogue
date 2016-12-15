
namespace Wink
{
    public abstract class Client : Sender
    {
        protected Server server;

        public string ClientName { get; set; }

        public Client(Server server)
        {
            this.server = server;
        }

        public abstract void Send(Event e);

    }
}
