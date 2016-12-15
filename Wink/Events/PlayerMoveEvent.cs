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
            Player.ActionPoints = Player.ActionPoints -1;
            server.LevelChanged();
        }

        public override bool Validate()
        {
            //TODO: Implement Validation.
            //Enough action points..
            return true;
        }
    }
}
