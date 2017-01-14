using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class Container : SpriteGameObject, IGameObjectContainer, IGUIGameObject, ITileObject
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
        private Tile Tile
        {
            get { return parent as Tile; }
        }

        public Container(string asset, GameObjectGrid itemGrid = null, int layer = 0, string id = "") : base(asset, layer, id)
        {
            itemGrid = itemGrid ?? new GameObjectGrid(2, 4);
            iBox = new InventoryBox(itemGrid, layer + 1, "", cameraSensitivity);
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

        public void InitGUI(Dictionary<string, object> guiState)
        {
            iWindow = new Window(iBox.ItemGrid.Columns * Tile.TileWidth, iBox.ItemGrid.Rows * Tile.TileHeight);
            iWindow.Add(iBox);
            iWindow.Position = guiState.ContainsKey("iWindowPosition") ? (Vector2)guiState["iWindowPosition"] : new Vector2(300, 300);
            iWindow.Visible = guiState.ContainsKey("iWindowVisibility") ? (bool)guiState["iWindowVisibility"] : false;

            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Add(iWindow);
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {
            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Remove(iWindow);

            guiState.Add("iWindowVisibility", iWindow.Visible);
            guiState.Add("iWindowPosition", iWindow.Position);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            iBox.Update(gameTime);

            if (iWindow != null && iWindow.Visible)
            {
                Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                int dx = (int)Math.Abs(player.Tile.Position.X - Tile.Position.X);
                int dy = (int)Math.Abs(player.Tile.Position.Y - Tile.Position.Y);
                bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;

                if (!withinReach)
                    iWindow.Visible = false;
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;

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

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            return iBox.FindAll(del);
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            return iBox.Find(del);
        }
    }
}
