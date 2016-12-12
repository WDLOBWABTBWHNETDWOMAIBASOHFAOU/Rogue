﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wink
{
    public class MouseSlot : GameObjectGrid
    {
        public Item oldItem;

        public MouseSlot(int layer = 0, string id = "") : base(1, 1, layer, id)
        {
        }

        public void AddTo(Item newItem, GameObjectGrid target)
        {
            if(target != null)
            {
            Vector2 targetPosition = newItem.Position / target.CellHeight;
            target.Add(oldItem, (int)targetPosition.X, (int)targetPosition.Y);
            }

            Add(newItem, 0, 0);
            oldItem = newItem;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = Mouse.GetState().Position.ToVector2() + new Vector2(10,10);
            if (Get(0,0) is EmptyItem)
            {
                grid[0, 0] = null;
            } 
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
        }
    }
}
