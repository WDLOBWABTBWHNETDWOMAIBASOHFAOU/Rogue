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
        private Window infoWindow;
        ItemSlot caller;

        public MouseSlot(int layer = 0, string id = "") : base(1, 1, layer, id)
        {
        }

        #region Serialization
        public MouseSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                oldItem = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("oldItemGUID"))) as Item; 
            }
            else
            {
                oldItem = info.GetValue("oldItem", typeof(Item)) as Item;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("oldItemGUID", oldItem.GUID.ToString()); 
            }
            else
            {
                info.AddValue("oldItem", oldItem);
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

        public void AddTo(Item newItem, ItemSlot target)
        {
            //check if 2 actual items are being swapped, else pick up or drop item.
            if (oldItem != null && newItem != null)
            {
                //check if the 2 items are the same, else swap items. (same if identical Id and type)
                if (oldItem.Id == newItem.Id && oldItem.GetType() == newItem.GetType())
                {
                    //handle item stacking. if total is greater than stacksize creates 1 full stack and 1 "leftover stack" in MouseSlot
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

        public void InfoScreen(ItemSlot caller)
        {
            this.caller = caller;
            caller.SlotItem.ItemInfo(caller);
            GameObjectList infoList = caller.SlotItem.InfoList;
            int interfall = 2;
            int y = infoTextHeigt(infoList,interfall);            
            int x = interfall*2 + infoTextWidth(infoList);

            infoWindow = new Window(x, y, false, false);
            infoWindow.Add(infoList);
            infoList.Position = infoWindow.Position;
        }

        private int infoTextWidth(GameObjectList list)
        {
            int width = 0;
            foreach (TextGameObject t in list.Children)
            {
                int tWidth = (int)t.Size.X;
                if (tWidth > width)
                {
                    width = tWidth;
                }
            }
            return width;
        }

        private int infoTextHeigt(GameObjectList list,int interfall)
        {
            int height = interfall;
            Vector2 prefPos = new Vector2(interfall, interfall);
            for (int i = 0; i < list.Children.Count; i++)
            {
                TextGameObject t = list.Children[i] as TextGameObject;
                list.Children[i].Position = prefPos ;
                prefPos += new Vector2(0, t.Size.Y + interfall);
                height += (int)t.Size.Y + interfall;
            }
            return height;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            
            if (oldItem != null)
            {
                oldItem.Update(gameTime);
                oldItem.Position = this.GlobalPosition;
            }

            if (infoWindow != null)
            {
                infoWindow.Update(gameTime);
                infoWindow.Position = GlobalPosition + new Vector2(10, -infoWindow.Height);
                if (infoWindow.GlobalPosition.Y + infoWindow.Height < GameWorld.BoundingBox.Top)
                {
                    infoWindow.Position = new Vector2(infoWindow.Position.X , infoWindow.Position.Y- infoWindow.Height - 40);
                }
                if (infoWindow.GlobalPosition.X + infoWindow.Width > GameEnvironment.Screen.X)
                {
                    infoWindow.Position =   new Vector2 (infoWindow.Position.X-infoWindow.Width-40, infoWindow.Position.Y);
                }
            }      
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            if(oldItem != null)
            {
                oldItem.Draw(gameTime, spriteBatch, camera);
            }
            if (infoWindow !=null)
            {
                infoWindow.Draw(gameTime, spriteBatch, camera);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            Position = inputHelper.MousePosition + new Vector2(10, 10);
            if (caller != null)
            {
                // check if mouse is still above the caller, if not dissable infoWindow
                if (!caller.ContainsMouse(inputHelper))
                {
                    caller.containsPrev = false;
                    infoWindow = null;
                    caller = null;
                }
            }
        }
    }
}
