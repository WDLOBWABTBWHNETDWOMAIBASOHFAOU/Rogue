using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class Player : Living
    {
        protected int exp;
        private MouseSlot mouseSlot;

        public MouseSlot MouseSlot { get { return mouseSlot; } }

        private readonly GameObjectGrid itemGrid;
        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }

        private readonly GameObjectList equipmentSlots;
        public GameObjectList EquipmentSlots
        {
            get { return equipmentSlots; }
        }

        public Player(string clientName, int layer) : base(layer, "player_" + clientName)
        {
            //Inventory
            mouseSlot = new MouseSlot(layer + 11, "mouseSlot");
            itemGrid = new GameObjectGrid(3, 6, 0, "");
            EquipmentSlot weapon = new EquipmentSlot();
            equipmentSlots = new GameObjectList();
            equipmentSlots.Add(weapon);

            SetStats(5, 5, 5, 5, 55);

            //InitAnimation(); not sure if overriden version gets played right without restating
        }

        public Player(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            exp = info.GetInt32("exp");
            mouseSlot = info.GetValue("mouseSlot", typeof(MouseSlot)) as MouseSlot;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("exp", exp);
            info.AddValue("mouseSlot", mouseSlot);
        }

        public void InitPosition()
        {
            //Put player on start tile.
            Tile startTile = GameWorld.Find("startTile") as Tile;
            Position = startTile.Position - startTile.Origin + Origin;
        }

        protected override void InitAnimation(string idleColor = "empty:65:65:10:Magenta")
        {            
            base.InitAnimation(idleColor);
            PlayAnimation("idle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            mouseSlot.Update(gameTime);

            if (exp >= RequiredExperience())
            {
                LevelUp();
            }
        }

        /// <summary>
        /// returns the experience a creature requires for its next level.
        /// </summary>
        /// <returns></returns>
        protected int RequiredExperience()
        {
            double mod = 36.79;
            double pow = Math.Pow(Math.E, Math.Sqrt(creatureLevel));
            int reqExp = (int)(mod * pow);
            return reqExp;
        }

        protected void LevelUp()
        {
            exp -= RequiredExperience();
            creatureLevel++;

            // + some amount of neutral stat points, distriputed by user discresion or increase stats based on picked hero
        } 
    }
}
