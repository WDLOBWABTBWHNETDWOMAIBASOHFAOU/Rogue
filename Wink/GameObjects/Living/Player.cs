using System;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections.Generic;

namespace Wink
{
    [Serializable]
    public class Player : Living
    {
        protected int exp;
        private MouseSlot mouseSlot;

        public MouseSlot MouseSlot { get { return mouseSlot; } }

        private GameObjectGrid itemGrid;
        public GameObjectGrid ItemGrid
        {
            get { return itemGrid; }
        }

        public override Point PointInTile
        {
            get { return new Point(Tile.TileWidth / 2, Tile.TileHeight / 2); }
        }

        public Player(string clientName, int layer) : base(layer, "player_" + clientName)
        {
            //Inventory
            mouseSlot = new MouseSlot(layer + 11, "mouseSlot");
            itemGrid = new GameObjectGrid(3, 6, 0, "");

            SetStats(5, 5, 5, 5, 55);

            InitAnimation(); //not sure if overriden version gets played right without restating
        }

        protected override void DoBehaviour(List<GameObject> changedObjects)
        {
            Debug.WriteLine("Called Player.DoBehaviour, but players don't have automated behaviour.");
        }

        #region Serialization
        public Player(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            exp = info.GetInt32("exp");
            if (context.GetVars().GUIDSerialization)
            {
                mouseSlot = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("mouseSlotGUID"))) as MouseSlot;
                itemGrid = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("itemGridGUID"))) as GameObjectGrid; 
            }
            else
            {
                mouseSlot = info.GetValue("mouseSlot", typeof(MouseSlot)) as MouseSlot;
                itemGrid = info.GetValue("itemGrid", typeof(GameObjectGrid)) as GameObjectGrid;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.GetVars().GUIDSerialization)
            {
                info.AddValue("mouseSlotGUID", mouseSlot.GUID.ToString());
                info.AddValue("itemGridGUID", itemGrid.GUID.ToString()); 
            }
            else
            {
                info.AddValue("mouseSlot", mouseSlot);
                info.AddValue("itemGrid", itemGrid);
            }
            info.AddValue("exp", exp);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void Replace(GameObject replacement)
        {
            if (mouseSlot != null && mouseSlot.GUID == replacement.GUID)
                mouseSlot = replacement as MouseSlot;
            if (itemGrid != null && itemGrid.GUID == replacement.GUID)
                itemGrid = replacement as GameObjectGrid;

            base.Replace(replacement);
        }

        protected override void InitAnimation(string idleColor = "player")
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
