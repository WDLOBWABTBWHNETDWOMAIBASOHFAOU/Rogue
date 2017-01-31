using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class NextLevelEvent : Event
    {
        private End end;
        private Player player;

        public NextLevelEvent(End end, Player player)
        {
            this.end = end;
            this.player = player;
        }

        #region Serialization
        public NextLevelEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            end = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("endGUID"))) as End;
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("endGUID", end.GUID.ToString());
            info.AddValue("playerGUID", player.GUID.ToString());
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
            NonAnimationSoundEvent nextLevelSound = new NonAnimationSoundEvent("wooden-stairs-1");
            LocalServer.SendToClients(nextLevelSound);
            Level level = new Level(server.LevelIndex + 1);
            List<GameObject> playerlist = server.Level.FindAll(obj => obj is Player);
            for (int p = 1; p <= playerlist.Count; p++)
            {
                Player player = playerlist[p - 1] as Player;
                (level.Find("StartTile" + p) as Tile).PutOnTile(player);
                player.ActionPoints = Player.MaxActionPoints;
            }
            foreach (Player p in playerlist)
                p.ComputeVisibility();

            server.Level = level;

            LocalServer.SendToClients(new LevelUpdatedEvent(level));
            return true;
        }

        public override bool Validate(Level level)
        {
            int dx = (int)Math.Abs(player.Tile.Position.X - end.Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - end.Tile.Position.Y);

            return dx <= Tile.TileWidth && dy <= Tile.TileHeight;
        }
    }
}
