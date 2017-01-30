using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class MouseSlot : GameObject, IGameObjectContainer
    {
        private Item item;
        private Window infoWindow;
        private ItemSlot caller;

        public Item Item
        {
            get { return item; }
        }

        public MouseSlot(int layer = 0, string id = "") : base(layer, id)
        {
        }

        #region Serialization
        public MouseSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                item = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("oldItemGUID"))) as Item; 
            }
            else
            {
                item = info.GetValue("oldItem", typeof(Item)) as Item;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("oldItemGUID", item.GUID.ToString()); 
            }
            else
            {
                info.AddValue("oldItem", item);
            }
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (item != null && item.GUID == replacement.GUID)
                item = replacement as Item;

            base.Replace(replacement);
        }

        public void AddTo(Item newItem, ItemSlot target)
        {
            Item oldItem = item;
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
            GameEnvironment.AssetManager.PlaySound("Sounds/CLICK10B");
            item = newItem;
            if (item != null)
                item.Parent = this;
        }

        public void InfoScreen(ItemSlot caller)
        {
            this.caller = caller;
            caller.SlotItem.ItemInfo(caller);
            GameObjectList infoList = caller.SlotItem.InfoList;
            int interfall = 2;
            int height = CalculateInfoTextHeight(infoList, interfall);
            int width = interfall * 2 + CalculateInfoTextWidth(infoList);

            infoWindow = new Window(width, height, false, false);
            infoWindow.Add(infoList);
        }

        private int CalculateInfoTextWidth(GameObjectList list)
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

        private int CalculateInfoTextHeight(GameObjectList list, int interfall)
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
            
            if (item != null)
                item.Update(gameTime);

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
            if (item != null)
                item.Draw(gameTime, spriteBatch, camera);

            if (infoWindow != null)
                infoWindow.Draw(gameTime, spriteBatch, camera);
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

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            GameObject go = Find(del);
            if (go != null)
                result.Add(go);

            return result;
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            if (item != null && del.Invoke(item))
                return item;
            else
                return null;
        }
    }
}
