using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    public abstract class Item : SpriteGameObject , ClickableGameObject
    {
        int stackSize;
        public Item(string assetName, int stackSize = 1, int layer = 0, string id= "") : base(assetName,layer,id)
        {
            this.stackSize = stackSize;    
        }
        public override void Update(GameTime gameTime)
        {
            if (Parent is MouseSlot)
            {
                cameraSensitivity = 0;
            }
            else
            {
                cameraSensitivity = 1;
            }
        }

        public void OnClick(Server server, LocalClient sender)
        {
            PickupEvent PuE = new PickupEvent(sender);
            PuE.player=(Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            PuE.item = this;
            PuE.target = PuE.item.Parent as GameObjectGrid;
            server.Send(PuE);
        }

    }   
}
