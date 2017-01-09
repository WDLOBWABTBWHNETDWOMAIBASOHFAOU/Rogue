using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class Container : SpriteGameObject, IGUIGameObject, ITileObject
    {
        private InventoryBox iBox;
        private Window iWindow;

        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }

        public bool BlocksTile
        {
            get { return true; }
        }

        public Container(string asset, GameObjectGrid itemGrid = null, int layer = 0, string id = "") : base(asset, layer, id)
        {
            SetInventory();
        }

        #region Serialization
        public Container(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                iBox = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("iBoxGUID"))) as InventoryBox; 
            }
            else
            {
                iBox = info.GetValue("iBox", typeof(InventoryBox)) as InventoryBox;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("iBoxGUID", iBox.GUID.ToString());
            }
            else
            {
                info.AddValue("iBox", iBox); 
            }
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (iBox != null && iBox.GUID == replacement.GUID)
                iBox = replacement as InventoryBox;

            base.Replace(replacement);
        }

        public void InitGUI()
        {
            iWindow = new Window(iBox.ItemGrid.Columns * Tile.TileWidth, iBox.ItemGrid.Rows * Tile.TileHeight);
            iWindow.Add(iBox);
            iWindow.Position = new Vector2(300, 300);
            iWindow.Visible = false;

            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Add(iWindow);
        }

        public void CleanupGUI()
        {
            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Remove(iWindow);
        }

        public void SetInventory(GameObjectGrid itemGrid = null)
        {
            if (itemGrid == null)
            {
                iBox = new InventoryBox(new GameObjectGrid(2, 4), layer + 1, "", cameraSensitivity);
            }
            else
            {
                iBox = new InventoryBox(itemGrid, layer + 1, "", cameraSensitivity);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            iBox.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                // correct player when in multiplayer?
                Player player = GameWorld.Find(p => p is Player) as Player;

                int dx = (int)Math.Abs(player.Tile.Position.X - GlobalPosition.X);
                int dy = (int)Math.Abs(player.Tile.Position.Y - GlobalPosition.Y);
                if (dx <= Tile.TileWidth && dy <= Tile.TileHeight)
                {
                    iWindow.Visible = !iWindow.Visible;
                }

            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }
    }
}
