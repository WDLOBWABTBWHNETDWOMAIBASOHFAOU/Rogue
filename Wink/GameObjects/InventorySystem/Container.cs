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
        private int clickCount;
        private int floorNumber;
        public InventoryBox IBox { get { return iBox; } }

        public virtual Point PointInTile
        {
            get { return new Point(0, 0); }
        }
        public virtual bool BlocksTile
        {
            get { return true; }
        }
        private Tile Tile
        {
            get { return parent.Parent as Tile; }
        }

        public Container(string asset, int floorNumber, InventoryBox inv = null, int layer = 0, string id = "") : base(asset, layer, id)
        {
            iBox = inv ?? new InventoryBox(new GameObjectGrid(2, 4), layer + 1, "", cameraSensitivity);
            this.floorNumber = floorNumber;
        }

        #region Serialization
        public Container(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            iBox = info.TryGUIDThenFull<InventoryBox>(context, "iBox");
            clickCount = info.GetInt32("clickCount");
            floorNumber = info.GetInt32("floorNumber");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationHelper.Variables v = context.GetVars();
            if (v.FullySerializeEverything || v.FullySerialized.Contains(iBox.GUID))
                info.AddValue("iBox", iBox);
            else
                info.AddValue("iBoxGUID", iBox.GUID.ToString()); 

            info.AddValue("clickCount", clickCount);
            info.AddValue("floorNumber", floorNumber);

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
            //Make a Window to display the contents of this container in.
            iWindow = new Window(iBox.ItemGrid.Columns * Tile.TileWidth, iBox.ItemGrid.Rows * Tile.TileHeight);
            iWindow.Add(iBox);
            iWindow.Position = guiState.ContainsKey("iWindowPosition") ? (Vector2)guiState["iWindowPosition"] : new Vector2(300, 300);
            iWindow.Visible = guiState.ContainsKey("iWindowVisibility") ? (bool)guiState["iWindowVisibility"] : false;

            PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
            gui.Add(iWindow);
        }

        public void CleanupGUI(Dictionary<string, object> guiState)
        {
            if (iWindow != null)
            {
                PlayingGUI gui = GameWorld.Find("PlayingGui") as PlayingGUI;
                gui.Remove(iWindow);

                guiState.Add("iWindowVisibility", iWindow.Visible);
                guiState.Add("iWindowPosition", iWindow.Position);
            }
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

        void InitContents(int floorNumber)
        {
            for (int x = 0; x < IBox.ItemGrid.Columns; x++)
            {
                int i = x % 4;
                int spawnChance;
                Item newItem;
                switch (i)
                {
                    #region cases
                    case 0:
                        spawnChance = 50;
                        newItem = new Potion(floorNumber);
                        break;
                    case 1:
                        spawnChance = 30;
                        newItem = new WeaponEquipment(floorNumber);
                        break;
                    case 2:
                        spawnChance = 30;
                        newItem = new BodyEquipment(floorNumber, 3);
                        break;
                    case 3:
                        spawnChance = 30;
                        newItem = new RingEquipment("empty:64:64:10:Gold");
                        break;
                    default:
                        throw new Exception("wtf");
                        #endregion
                }
                for (int y = 0; y < IBox.ItemGrid.Rows; y++)
                {
                    if (spawnChance > GameEnvironment.Random.Next(100))
                    {
                        ItemSlot cS = IBox.ItemGrid.Get(x, y) as ItemSlot;
                        cS.ChangeItem(newItem);
                    }
                }
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                Player player = GameWorld.Find(p => p.Id == Player.LocalPlayerName) as Player;

                int dx = (int)Math.Abs(player.Tile.Position.X - Tile.Position.X);
                int dy = (int)Math.Abs(player.Tile.Position.Y - Tile.Position.Y);
                bool withinReach = dx <= Tile.TileWidth && dy <= Tile.TileHeight;
                if (withinReach)
                {
                    //if(clickCount == 0)
                    //{
                    //    InitContents(floorNumber);
                    //}
                    //clickCount++;
                    iWindow.Visible = !iWindow.Visible;
                }
                //possibly generate items here on first click instead of at floor generation (possible to take specific players luck in to account)

            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);
            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            if (iBox != null)
                return iBox.FindAll(del);
            else
                return new List<GameObject>();
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            if (iBox != null)
                return iBox.Find(del);
            else
                return null;
        }
    }
}
