using System;

namespace Wink
{
    [Serializable()]
    public class PlayerMoveEvent : ActionEvent
    {
        public Player Player { get; set; }
        public Tile Tile { get; set; }

        protected override int Cost
        {
            get { return 1; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            Player.MoveTo(Tile);
            Player.ActionPoints = Player.ActionPoints - 1;
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}
