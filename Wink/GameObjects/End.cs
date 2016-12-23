using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class End : SpriteGameObject
    {
        Tile ParentTile;
        int levelIndex;
        Level level;
        public End(Tile ParentTile, int levelIndex, Level level2, string asset = "empty:65:65:10:Yellow", int layer = 0, string id = "") : base(asset, layer, id)
        {
            this.ParentTile = ParentTile;
            this.levelIndex = levelIndex;
            level = level2;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
                {
                    // correct player when in multiplayer?
                    Player player = GameWorld.Find(p => p is Player) as Player;

                    int dx = (int)Math.Abs(player.Position.X - player.Origin.X - Position.X);
                    int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - Position.Y);

                    if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                    {
                        Event e = new NextLevelEvent();
                        Server.Send(e);
                    }
                };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
