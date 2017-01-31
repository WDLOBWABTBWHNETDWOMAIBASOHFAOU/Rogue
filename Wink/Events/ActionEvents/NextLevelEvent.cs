using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class NextLevelEvent : ActionEvent
    {
        private End end;

        public NextLevelEvent(End end, Player player): base(player)
        {
            this.end = end;
        }

        #region Serialization
        public NextLevelEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            end = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("endGUID"))) as End;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("endGUID", end.GUID.ToString());
        }
        #endregion


        public override bool GUIDSerialization
        {
            get { return false; }
        }

        protected override int Cost
        {
            get
            {
                return 0;
            }
        }

        protected override void DoAction(LocalServer server)
        {
            NonAnimationSoundEvent nextLevelSound = new NonAnimationSoundEvent("Sounds/wooden-stairs-1");
            LocalServer.SendToClients(nextLevelSound);
            Level level = new Level(server.LevelIndex + 1);
            List<GameObject> playerlist = server.Level.FindAll(obj => obj is Player);
            for (int p = 1; p <= playerlist.Count; p++)
            {
                Player player = playerlist[p - 1] as Player;
                player.MoveTo(level.Find("StartTile" + p) as Tile);
            }
            foreach (Player p in playerlist)
                p.ComputeVisibility();

            server.Level = level;
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Tile.Position.X - end.Tile.Position.X);
            int dy = (int)Math.Abs(player.Tile.Position.Y - end.Tile.Position.Y);

            return dx <= Tile.TileWidth && dy <= Tile.TileHeight;
        }
    }
}
