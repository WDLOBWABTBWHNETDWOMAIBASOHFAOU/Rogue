using System;

namespace Wink
{
    class PlayerTurnEvent : Event
    {
        public override void OnClientReceive(LocalClient client)
        {
            //TODO: Change variable in localclient that indicates that it's its turn.
            client.IsMyTurn = true;
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
