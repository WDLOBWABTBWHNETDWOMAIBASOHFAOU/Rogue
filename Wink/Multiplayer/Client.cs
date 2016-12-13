﻿
namespace Wink
{
    public abstract class Client
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
