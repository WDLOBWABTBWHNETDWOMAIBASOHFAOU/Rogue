using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    public abstract class ActionEvent : Event
    {
        protected abstract int Cost { get; }

        public ActionEvent(Sender sender) : base(sender)
        {

        }

        public sealed override void OnServerReceive(LocalServer server)
        {
            Player p = server.Level.Find("player_" + (sender as Client).ClientName) as Player;
            p.ActionPoints -= Cost;

            DoAction(server);
        }

        public sealed override bool Validate(Level level)
        {
            Player p = level.Find("player_" + (sender as Client).ClientName) as Player;
            return ValidateAction(level) && p.isTurn && p.ActionPoints >= Cost;
        }

        protected abstract bool ValidateAction(Level level);
        protected abstract void DoAction(LocalServer server);
    }
}
