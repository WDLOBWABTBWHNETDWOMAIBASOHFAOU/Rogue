using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    [Serializable]
    public class MouseSlot : GameObjectGrid
    {
        public Item oldItem;

        public MouseSlot(int layer = 0, string id = "") : base(1, 1, layer, id)
        {
        }

        public void AddTo(Item newItem, GameObjectGrid target)
        {
            if (target != null)
            {
                Vector2 targetPosition = newItem.Position / target.CellHeight;
                if (oldItem == null)
                {
                    target.Remove((int)targetPosition.X, (int)targetPosition.Y);
                }
                else
                {
                    target.Add(oldItem, (int)targetPosition.X, (int)targetPosition.Y);
                }
            }

            Add(newItem, 0, 0);
            oldItem = newItem;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // if (Get(0, 0) is EmptyItem)
            if (Get(0, 0) is EmptyItem)
            // it's not shorter but it is more consistent.
            {
                grid[0, 0] = null;
                oldItem = null;
            }
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            Position = inputHelper.MousePosition + new Vector2(10, 10);
        }
    }
}
