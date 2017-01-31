using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class ClientPlayerType : Event
    {
        PlayerType playerType;
        string ClientName;

        public ClientPlayerType(string ClientName, PlayerType playerType) : base()
        {
            this.ClientName = ClientName;
            this.playerType = playerType;
        }
        
        #region Serialization
        public ClientPlayerType(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            playerType = (PlayerType)info.GetValue("playerType", typeof(PlayerType));
            ClientName = info.GetString("ClientName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("playerType", playerType);
            info.AddValue("ClientName", ClientName);
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
            server.Clients.Find(c => c.ClientName == ClientName).playerType = playerType;
            return true;
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
