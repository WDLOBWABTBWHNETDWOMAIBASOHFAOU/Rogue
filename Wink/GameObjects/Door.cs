using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class Door:SpriteGameObject
    {
        Tile ParentTile;
        public Door(Tile ParentTile,string asset = "empty:65:65:10:DarkGray", int layer = 0, string id = "") : base(asset, layer, id)
        {
            this.ParentTile = ParentTile;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (visible)
            {
                Action onClick = () =>
                {
                    // correct player when in multiplayer?
                    Player player = GameWorld.Find(p => p is Player) as Player;

                    int dx = (int)Math.Abs(player.Position.X - player.Origin.X - Position.X);
                    int dy = (int)Math.Abs(player.Position.Y - player.Origin.Y - Position.Y);

                    if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                    {
                        this.Visible = false;
                        ParentTile.Passable = true;
                    }

                };
                inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
                base.HandleInput(inputHelper);

            }
        }

    }
}
