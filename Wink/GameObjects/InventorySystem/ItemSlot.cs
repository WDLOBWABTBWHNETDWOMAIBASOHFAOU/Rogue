using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class ItemSlot : SpriteGameObject
    {
        private Item slotItem;
        public Item SlotItem
        {
            get { return slotItem; }
        }

        public ItemSlot(string assetName = "empty:65:65:10:Gray", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            slotItem = null;
        }

        #region Serialization
        public ItemSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                slotItem = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("slotItemGUID"))) as Item;
            }
            else
            {
                slotItem = info.GetValue("slotItem", typeof(Item)) as Item;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("slotItemGUID", slotItem.GUID.ToString());
            }
            else
            {
                info.AddValue("slotItem", slotItem);
            }
            base.GetObjectData(info, context);
        }
        #endregion

        public void ChangeItem(Item newItem)
        {
            slotItem = newItem;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(slotItem != null)
            {
                slotItem.Position = GlobalPosition;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);
            if (slotItem != null)
            {
                slotItem.Draw(gameTime, spriteBatch, camera);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                PickupEvent pue = new PickupEvent();
                pue.item = slotItem;
                pue.target = this;
                pue.player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                Server.Send(pue);
            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

            base.HandleInput(inputHelper);
        }
    }
}
