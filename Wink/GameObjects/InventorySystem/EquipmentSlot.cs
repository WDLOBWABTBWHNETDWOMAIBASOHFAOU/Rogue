using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class EquipmentSlot : ItemSlot
    {
        Type equipmentRestriction;

        /// <summary>
        /// ItemSlots specificly for items that can be equipped, items that do not match the equipment restriction won't be equipped.
        /// </summary>
        /// <param name="equipmentRestriction"> Specify what type of equipment this slot can hold</param>
        public EquipmentSlot(Type equipmentRestriction, string assetName = "empty:65:65:10:Green", int layer = 0, string id = "", int sheetIndex = 0, float cameraSensitivity = 0, float scale = 1) : base(assetName, layer, id, sheetIndex, cameraSensitivity, scale)
        {
            this.equipmentRestriction = equipmentRestriction;
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Player player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            Item mouseItem = player.MouseSlot.oldItem;
            if(mouseItem == null || mouseItem.GetType() == equipmentRestriction)
            {
                base.HandleInput(inputHelper);
                return;
            }         
        }
    }        
}
