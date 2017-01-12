using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Wink
{
    public abstract class Item : SpriteGameObject, ITileObject
    {
        int stackSize;

        public Point PointInTile
        {
            get { return new Point(0, 0); }
        }

        public bool BlocksTile
        {
            get { return false; }
        }

        public Item(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, layer, id)
        {
            this.stackSize = stackSize;    
        }

        public Item(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            stackSize = info.GetInt32("stackSize");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("stackSize", stackSize);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Parent is MouseSlot)
            {
                cameraSensitivity = 0;
            }
            else if (Parent is GameObjectGrid)
            {
                cameraSensitivity = 0;
            }
            else
            {
                cameraSensitivity = 1;
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Action onClick = () =>
            {
                PickupEvent PuE = new PickupEvent();
                PuE.player = GameWorld.Find(Player.LocalPlayerName) as Player;
                PuE.item = this;
                PuE.target = PuE.item.Parent as GameObjectGrid;
                Server.Send(PuE);
            };
            inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

            base.HandleInput(inputHelper);
        }
    }   
}
