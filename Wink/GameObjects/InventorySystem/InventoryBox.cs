using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class InventoryBox : GameObjectList
    {
        /// <summary>
        /// The grid that contains the actual items
        /// </summary>
        private GameObjectGrid itemGrid;

        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }
        private float cameraSensitivity;
        public float CameraSensitivity { get { return cameraSensitivity; } }
        
        public InventoryBox(GameObjectGrid itemGrid, int layer = 0, string id = "", float cameraSensitivity = 0) : base(layer, id)
        {
            this.cameraSensitivity = cameraSensitivity;
            itemGrid.CameraSensitivity = cameraSensitivity;
            itemGrid.CellHeight = Tile.TileHeight;
            itemGrid.CellWidth = Tile.TileWidth;
            CheckGrid(itemGrid);
            itemGrid.Add(new TestItem(), 2, 2);
            Add(itemGrid);
            this.itemGrid = itemGrid;
        }

        //public InventoryBox(int rows=3, int columns=6, int layer = 0, string id = "") : this(new GameObjectGrid(rows, columns, layer + 1, "itemGrid"), layer, id)
        //{
        //}

        public InventoryBox(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            itemGrid = info.GetValue("itemGrid", typeof(GameObjectGrid)) as GameObjectGrid;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("itemGrid", itemGrid);
        }   

        private void CheckGrid(GameObjectGrid itemGrid)
        {
            for (int x = 0; x < itemGrid.Columns; x++)
            {
                for (int y = 0; y < itemGrid.Rows; y++)
                {
                    if ( itemGrid.Get(x,y) == null)
                    {
                        itemGrid.Add(new EmptyItem("empty:65:65:10:Gray"),x,y);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            itemGrid.Update(gameTime);
            CheckGrid(ItemGrid);
            for (int x = 0; x < itemGrid.Columns; x++)
            {
                for (int y = 0; y < itemGrid.Rows; y++)
                {
                    itemGrid.Get(x, y).Visible = visible;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            itemGrid.Draw(gameTime, spriteBatch, camera);
        }
    } 
}
