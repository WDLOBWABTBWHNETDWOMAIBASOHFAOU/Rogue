using System;
using System.Collections.Generic;
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

        #region Serialization
        public EndTurnEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("playerGUID", player.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //Irrelevant because client->server
        }

        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override bool OnServerReceive(LocalServer server)
        {
            server.EndTurn(player);
            return true;
        }

        public override bool Validate(Level level)
        {
            return player.ActionPoints > 0;
        }
    }
}
