﻿using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    public class PlayerMoveEvent : ActionEvent
    {
        private Tile tile;

        public PlayerMoveEvent(Player player, Tile tile) : base(player)
        {
            this.tile = tile;
        }

        protected override int Cost
        {
            get { return 1; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        protected override void DoAction(LocalServer server)
        {
            player.MoveTo(tile);
            server.LevelChanged();
        }

        protected override bool ValidateAction(Level level)
        {
            int dx = (int)Math.Abs(player.Position.X - player.Origin.X - tile.Position.X);
            int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - tile.Position.Y);

            bool theSame = dx == 0 && dy == 0;
            bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
            return withinReach && !theSame;
        }
    }
}
