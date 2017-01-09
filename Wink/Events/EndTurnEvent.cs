using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class EndTurnEvent : Event
    {
        private Player player;

        public EndTurnEvent(Player player)
        {
            this.player = player;
        }

        public EndTurnEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("playerGUID", player.GUID.ToString());
            base.GetObjectData(info, context);
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnServerReceive(LocalServer server)
        {
            server.EndTurn(player);
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
