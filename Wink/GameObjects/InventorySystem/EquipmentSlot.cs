using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class EquipmentSlot : ItemSlot
    {
        public EquipmentSlot(string assetName = "empty:65:65:10:Green") : base(assetName)
        {
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            Player player = (Root as GameObjectList).Find("player_" + Environment.MachineName) as Player;
            if(player.MouseSlot.oldItem == null)
            {
                base.HandleInput(inputHelper);
                return;
            }
            else if (player.MouseSlot.oldItem.GetType() == typeof(Equipment))
            {
                base.HandleInput(inputHelper);
                return;
            }            
        }
    }        
}
