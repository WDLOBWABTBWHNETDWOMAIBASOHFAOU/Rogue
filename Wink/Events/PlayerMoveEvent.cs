using System;

namespace Wink
{
    [Serializable()]
    public class PlayerMoveEvent : Event
    {
        public Player Player { get; set; }
        public Tile Tile { get; set; }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            Player.MoveTo(Tile);
            server.LevelChanged();
        }
    }
}
