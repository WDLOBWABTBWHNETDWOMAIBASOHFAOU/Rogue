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
        }

        #region Serialization
        public MouseSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().DownwardSerialization)
            {
                oldItem = info.GetValue("oldItem", typeof(Item)) as Item;
            }
            else
            {
                oldItem = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("oldItemGUID"))) as Item;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().DownwardSerialization)
            {
                info.AddValue("oldItem", oldItem);
            }
            else
            {
                info.AddValue("oldItemGUID", oldItem.GUID.ToString());
            }
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (oldItem != null && oldItem.GUID == replacement.GUID)
                oldItem = replacement as Item;

            base.Replace(replacement);
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
