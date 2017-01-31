using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Serialization
        public ActionEvent(SerializationInfo info, StreamingContext context)
        {
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore GUID based serialization is used.
            info.AddValue("playerGUID", player.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //Irrelevant because client->server
        }

        protected void AddVisibleTiles(Level level, Living living, HashSet<GameObject> changedObjects)
        {
            List<GameObject> vision = level.FindAll(obj => obj is Tile && (obj as Tile).SeenBy.ContainsKey(living));
            foreach (GameObject go in vision)
                changedObjects.Add(go);
        }
        #endregion

        protected void AddVisibleTiles(Level level, HashSet<GameObject> changedObjects)
        {
            AddVisibleTiles(level, player, changedObjects);
        }

        public sealed override bool OnServerReceive(LocalServer server)
        {
            HashSet<GameObject> changed = new HashSet<GameObject>();
            DoAction(server, changed);
            player.ActionPoints -= Cost;

            changed.Add(player);
            LocalServer.SendToClients(new LevelChangedEvent(changed));
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

        protected abstract bool ValidateAction(Level level);
        protected abstract void DoAction(LocalServer server, HashSet<GameObject> changedObjects);
    }
}
