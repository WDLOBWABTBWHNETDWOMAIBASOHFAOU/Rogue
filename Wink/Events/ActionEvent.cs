using System;
using System.Runtime.Serialization;

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

        public ActionEvent(SerializationInfo info, StreamingContext context)
        {
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore GUID based serialization is used.
            info.AddValue("playerGUID", player.GUID.ToString());
        }

        public sealed override void OnServerReceive(LocalServer server)
        {
            server.ChangedObjects.Add(player);
            DoAction(server);
            player.ActionPoints -= Cost;
            //server.SendOutLevelChanges();
        }

        public sealed override bool Validate(Level level)
        {
            return ValidateAction(level) && /*player.isTurn &&*/ player.ActionPoints >= Cost;
        }

        protected abstract bool ValidateAction(Level level);
        public abstract void DoAction(LocalServer server);
    }
}
