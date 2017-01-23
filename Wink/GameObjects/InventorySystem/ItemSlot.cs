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
        public bool containsPrev;

        public virtual Type TypeRestriction
        {
            get { return typeof(Item); }
        }
        public Item SlotItem
        {
            get { return slotItem; }
        }

        public ItemSlot(string assetName = "empty:65:65:10:Gray", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            slotItem = null;            
            containsPrev = false;
        }

        #region Serialization
        public ItemSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            slotItem = info.TryGUIDThenFull<Item>(context, "slotItem");

            containsPrev = info.GetBoolean("containsPrev");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationHelper.Variables v = context.GetVars();
            if (v.FullySerializeEverything || v.FullySerialized.Contains(slotItem.GUID))
            {
                info.AddValue("slotItem", slotItem);
            }
            else
            {
                info.AddValue("slotItemGUID", slotItem.GUID.ToString());
            }

            info.AddValue("containsPrev", containsPrev);
            base.GetObjectData(info, context);
        }
        #endregion

        public void ChangeItem(Item newItem)
        {
            slotItem = newItem;
            if (slotItem != null)
                slotItem.Parent = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (slotItem != null)
            {
                if (slotItem.stackCount <= 0)
                    slotItem = null;
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
            Action onLeftClick = () =>
            {
                Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                PickupEvent pue = new PickupEvent(slotItem, player, this);
                Server.Send(pue);
            };
            
            Action onRightClick = () =>
            {
                Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                slotItem.ItemAction(player);
            };

            if (slotItem != null)
            {
                inputHelper.IfMouseRightButtonPressedOn(this, onRightClick);
                if (ContainsMouse(inputHelper) && !containsPrev)
                {
                    Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
                    player.MouseSlot.InfoScreen(this);
                    containsPrev = true;
                }
            }
            inputHelper.IfMouseLeftButtonPressedOn(this, onLeftClick);

            base.HandleInput(inputHelper);
        }

        public List<GameObject> FindAll(Func<GameObject, bool> del)
        {
            List<GameObject> result = new List<GameObject>();
            if (slotItem != null && del.Invoke(slotItem))
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

        public bool ContainsMouse(InputHelper inputHelper)
        {
            return BoundingBox.Contains(inputHelper.MousePosition);
        }
    }
}
