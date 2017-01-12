using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public abstract class Item : SpriteGameObject
    {
        int stackSize;
        public int stackCount;
        public int getStackSize { get { return stackSize; } }
        protected GameObjectList infoList;
        public GameObjectList InfoList { get { return infoList; } }

        public Item(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, layer, id)
        {
            // item id is needed to chech if they are the same, for now assetname to test.
            // if item are proceduraly generated, there should be an algoritim that generates a id that is the same if stats (and sprite) are the same.
            if (id == "")
            {
                this.id = assetName;
            }
            else
            {
                this.id = id;
            }
            stackCount = 1;
            this.stackSize = stackSize;
            cameraSensitivity = 0;
        }

        public Item(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            stackSize = info.GetInt32("stackSize");
            stackCount = info.GetInt32("stackCount");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("stackSize", stackSize);
            info.AddValue("stackCount", stackCount);
        }

        public virtual void ItemInfo(ItemSlot caller)
        {
            infoList = new GameObjectList();
            TextGameObject IDinfo = new TextGameObject("Arial26",0,0,"IDinfo."+this);
            IDinfo.Text = Id;
            IDinfo.Color = Color.Red;
            infoList.Add(IDinfo);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
        }
        

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
