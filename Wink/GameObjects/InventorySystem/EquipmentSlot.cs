﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    public class EquipmentSlot : ItemSlot
    {
        private Type equipmentRestriction;

        /// <summary>
        /// ItemSlots specificly for items that can be equipped, items that do not match the equipment restriction won't be equipped.
        /// </summary>
        /// <param name="equipmentRestriction"> Specify what type of equipment this slot can hold</param>
        public EquipmentSlot(Type equipmentRestriction, string assetName = "empty:65:65:10:Green", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            this.equipmentRestriction = equipmentRestriction;
        }

        #region Serializable
        public EquipmentSlot(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            equipmentRestriction = Type.GetType(info.GetString("equipmentRestriction"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("equipmentRestriction", equipmentRestriction.FullName);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void HandleInput(InputHelper inputHelper)
        {
            Player player = GameWorld.Find(Player.LocalPlayerName) as Player;
            Item mouseItem = player.MouseSlot.oldItem;
            if (mouseItem == null || mouseItem.GetType() == equipmentRestriction)
            {
                base.HandleInput(inputHelper);
                return;
            }         
        }
    }        
}
