using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public abstract class Item : SpriteGameObject, ITileObject
    {
        private int stackSize;
        public int stackCount;
        protected GameObjectList infoList;

        public int getStackSize { get { return stackSize; } }
        public GameObjectList InfoList { get { return infoList; } }
        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }
        public bool BlocksTile
        {
            get { return false; }
        }

        public abstract void DoBonus(Living living);
        public abstract void RemoveBonus(Living living);
        /// <summary>
        /// Generated Item
        /// </summary>
        /// <param name="stackSize"></param>
        /// <param name="layer"></param>
        public Item(int floorNumber, int stackSize = 1, int layer = 0) : base("empty:64:64:10:BlanchedAlmond", layer, "", cameraSensitivity: 0)
        { //item id is needed to check if they are the same, for now assetname to test.
            //TODO: if item are proceduraly generated, there should be an algorithm that generates an id that is the same if stats (and sprite) are the same.
            if (id == "")
                this.id = "empty:64:64:10:BlanchedAlmond";

            stackCount = 1;
            this.stackSize = stackSize;
        }

        /// <summary>
        /// Specific Item
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="stackSize"></param>
        /// <param name="layer"></param>
        /// <param name="id"></param>
        public Item(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, layer, id, cameraSensitivity: 0)
        {
            //item id is needed to check if they are the same, for now assetname to test.
            //TODO: if item are proceduraly generated, there should be an algorithm that generates an id that is the same if stats (and sprite) are the same.
            if (id == "")
                this.id = assetName;
            else
                this.id = id;

            stackCount = 1;
            this.stackSize = stackSize;
        }

        #region Serialization
        public Item(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            stackSize = info.GetInt32("stackSize");
            stackCount = info.GetInt32("stackCount");
            infoList = info.GetValue("infoList", typeof(GameObjectList)) as GameObjectList;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("stackSize", stackSize);
            info.AddValue("stackCount", stackCount);
            info.AddValue("infoList", infoList);
        }
        #endregion

        public virtual void ItemInfo(ItemSlot caller)
        {
            infoList = new GameObjectList();
            TextGameObject IDinfo = new TextGameObject("Arial26", cameraSensitivity: 0, layer: 0, id: "IDinfo." + this);
            IDinfo.Text = Id.Split(':')[0];//only show first part of the id
            IDinfo.Color = Color.Red;
            infoList.Add(IDinfo);
        }

        public virtual void ItemAction(Living caller) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);

            if (stackCount > 1)
            {
                // Position and color subject to change
                spriteBatch.DrawString(GameEnvironment.AssetManager.GetFont("Arial26"), stackCount.ToString(), GlobalPosition, Color.WhiteSmoke);
            }
        }
    }   
}
