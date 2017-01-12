using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class MouseSlot : GameObjectGrid
    {
        public Item oldItem;

        public MouseSlot(int layer = 0, string id = "") : base(1, 1, layer, id)
        {
            oldItem = null;
        }

        public MouseSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            oldItem = info.GetValue("oldItem",typeof(Item))as Item;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("oldItem", oldItem);
        }

        public void AddTo(Item newItem, ItemSlot target)
        {
            // check if 2 actual items are being swapt, else pick up or drop item.
            if(oldItem!=null && newItem != null)
            {
                //check if the 2 items are the same, else swap items. (same if identical Id and type)
                if(oldItem.Id == newItem.Id && oldItem.GetType()==newItem.GetType())
                {
                    // handle item stacking. if total is greater than stacksize creates 1 full stack and 1 "leftover stack" in MouseSlot
                    if (oldItem.getStackSize >= oldItem.stackCount + newItem.stackCount)
                    {
                        oldItem.stackCount += newItem.stackCount;
                        newItem = null;
                    }
                    else if (!(newItem.stackCount == newItem.getStackSize || oldItem.stackCount == oldItem.getStackSize))
                    {
                        int dif = (oldItem.stackCount + newItem.stackCount) - oldItem.getStackSize;
                        oldItem.stackCount = oldItem.getStackSize; // full stack
                        newItem.stackCount = dif; // leftover stack
                    }
                }
            }
            target.ChangeItem(oldItem);
            oldItem = newItem;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (oldItem != null)
            {
                oldItem.Update(gameTime);
                oldItem.Position = this.GlobalPosition;
            }            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            if(oldItem != null)
            {
                oldItem.Draw(gameTime, spriteBatch, camera);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            Position = inputHelper.MousePosition + new Vector2(10, 10);
        }
    }
}
