using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Wink
{
    public class ItemSlot : SpriteGameObject
    {
        Item slotItem;
        public Item SlotItem { get { return slotItem; } }
        
        public bool containsPrev;

        public ItemSlot(string assetName = "empty:65:65:10:Gray", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            slotItem = null;            
            containsPrev = false;
        }

        public ItemSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            slotItem = info.GetValue("slotItem", typeof(Item)) as Item;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("slotItem", slotItem);
        }

        public void ChangeItem(Item newItem)
        {
            slotItem = newItem;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (slotItem != null)
            {
                slotItem.Position = GlobalPosition;
                if (slotItem.stackCount <= 0)
                {
                    slotItem = null;
                }
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
                PickupEvent pue = new PickupEvent();
                pue.item = slotItem;
                pue.target = this;
                pue.player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                Server.Send(pue);
            };
            
            Action onRightClick = () =>
            {
                // rightclick action
                Player player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                slotItem.ItemAction(player);
               
            };

            if (slotItem != null)
            {
                inputHelper.IfMouseRightButtonPressedOn(this, onRightClick);
                if (ContainsMouse(inputHelper) && !containsPrev)
                {
                    Player player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
                    player.MouseSlot.InfoScreen(this);
                    containsPrev = true;
                }
            }
            inputHelper.IfMouseLeftButtonPressedOn(this, onLeftClick);

            base.HandleInput(inputHelper);
        }

        public bool ContainsMouse(InputHelper inputHelper)
        {
            return BoundingBox.Contains(inputHelper.MousePosition);
        }
    }
}
