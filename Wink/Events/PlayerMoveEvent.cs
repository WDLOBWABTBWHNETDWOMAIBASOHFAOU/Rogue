using System;

namespace Wink
{
    [Serializable()]
    public class PlayerMoveEvent : ActionEvent
    {
        public Player Player { get; set; }
        public Tile Tile { get; set; }

        public PlayerMoveEvent(Sender sender) : base(sender)
        {

        }

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
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            //TODO: Implement Validation.
            return true;
        }
    }
}
