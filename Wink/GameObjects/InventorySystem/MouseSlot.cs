using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        public void AddTo(Item newItem, ItemSlot target)
        {
            // check if 2 actual items are being swapt, else pick up or drop item.
            if(oldItem!=null && newItem != null)
            {
                //check if the 2 items are the same and if stacksize is big enough to stack, else swap items.
                if(oldItem.Id == newItem.Id && oldItem.getStackSize >= oldItem.stackCount + newItem.stackCount)
                {
                    // expand code so 2 partial filled stacks which combined are larger than the allowed stack size
                    // to create 1 fully filled stack and 1 "leftover stack" 
                    oldItem.stackCount += newItem.stackCount;
                    target.ChangeItem(oldItem);
                    oldItem = null;
                    return;
                }
                else
                {
                    target.ChangeItem(oldItem);
                    oldItem = newItem;
                }
            }
            else
            {
                target.ChangeItem(oldItem);
                oldItem = newItem;
            }
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
