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

        public sealed override bool OnServerReceive(LocalServer server)
        {
            server.ChangedObjects.Add(player);
            DoAction(server);
            player.ActionPoints -= Cost;
            //server.SendOutLevelChanges();
            return true;
        }
        
        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public sealed override bool Validate(Level level)
        {
            return ValidateAction(level) && player.ActionPoints >= Cost;
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected abstract bool ValidateAction(Level level);
        protected abstract void DoAction(LocalServer server);
    }
}
