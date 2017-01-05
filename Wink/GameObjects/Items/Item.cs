using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace Wink
{
    public abstract class Item : SpriteGameObject
    {
        int stackSize;
        public int stackCount;
        public int getStackSize { get { return stackSize; } }

        public Item(string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, layer, id)
        {
            // item id is needed to chech if they are the same, for now assetname to test.
            // if item are proceduraly generated, there should be an algoritim that generates a id that is the same if stats (and sprite) are the same.
            this.id = assetName;

            this.stackSize = stackSize;
            cameraSensitivity = 0;    
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
           //position = parent.Position;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            //Action onClick = () =>
            //{
            //    PickupEvent PuE = new PickupEvent();
            //    PuE.player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            //    PuE.item = this;
            //    PuE.target = PuE.item.Parent as GameObjectGrid;
            //    Server.Send(PuE);
            //};
            //inputHelper.IfMouseLeftButtonPressedOn(this, onClick);

            base.HandleInput(inputHelper);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, camera);

            if (stackCount > 1)
            {
                // Position and color subject to change
                spriteBatch.DrawString(GameEnvironment.AssetManager.GetFont("Arial26"), stackCount.ToString(), GlobalPosition, Color.WhiteSmoke);
            }
        }
    }   
}
