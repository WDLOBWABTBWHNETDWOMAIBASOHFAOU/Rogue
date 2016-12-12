
namespace Wink
{
    public abstract class Server
    {
        
        public Server()
        {
        }

        public abstract void Send(Event e);
        public abstract void AddLocalClient(LocalClient localClient);
    }
}
