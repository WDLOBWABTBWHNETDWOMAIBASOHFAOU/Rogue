using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class ItemSlot : SpriteGameObject, IGameObjectContainer
    {
        private Item slotItem;
        public Item SlotItem
        {
            get { return slotItem; }
        }

        public virtual Type TypeRestriction
        {
            get { return typeof(Item); }
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
                Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                PickupEvent pue = new PickupEvent(slotItem, player, this);
                Server.Send(pue);
            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            if (del.Invoke(slotItem))
                result.Add(slotItem);
            return result;
        }

        public GameObject Find(Func<GameObject, bool> del)
        {
            if (slotItem != null && del.Invoke(slotItem))
                return slotItem;
            else
                return null;
        }
    }
}
