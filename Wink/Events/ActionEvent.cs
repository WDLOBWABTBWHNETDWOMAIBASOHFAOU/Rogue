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
        //The player that acted.
        protected Player player;

        protected abstract int Cost { get; }

        public ActionEvent(Player player) : base()
        {
            this.player = player;
        }

        public sealed override void OnServerReceive(LocalServer server)
        {
            player.ActionPoints -= Cost;

            DoAction(server);
        }

        public sealed override bool Validate(Level level)
        {
            return ValidateAction(level) && player.isTurn && player.ActionPoints >= Cost;
        }

        protected abstract bool ValidateAction(Level level);
        protected abstract void DoAction(LocalServer server);
    }
}
