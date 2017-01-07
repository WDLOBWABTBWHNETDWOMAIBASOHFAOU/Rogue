using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public class ItemSlot : SpriteGameObject
    {
        Item slotItem;
        public Item SlotItem { get { return slotItem; } }

        public ItemSlot(string assetName = "empty:65:65:10:Gray", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            slotItem = null;
        }

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
