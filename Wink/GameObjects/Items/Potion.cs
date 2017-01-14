using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Wink
{
    
    enum PotionType
    {
        health,
        mana
    }

    class Potion:Item            
    {
        PotionType potionType;
        public PotionType GetPotionType { get { return potionType; } }
        int potionValue;
        public int PotionValue { get { return potionValue; } }


        public Potion(string assetName, string id, PotionType potionType, int potionValue, int stackSize = 10,int layer=0):base(assetName,stackSize,layer,id)
        {
            this.potionType = potionType;
            this.potionValue = potionValue;
        }

        public Potion(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            potionType = (PotionType)info.GetValue("potionType", typeof(PotionType));
            potionValue = info.GetInt32("potionValue");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("potionType", potionType);
            info.AddValue("potionValue", potionValue);
        }

        public override void ItemAction(Living caller)
        {
            TakenPotionEvent p = new TakenPotionEvent(caller as Player, this);
            Server.Send(p);
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject PotionText = new TextGameObject("Arial12", 0, 0, "PotionText." + this);

            PotionText.Text = "Restores " + potionValue + " points of ";
            switch (potionType)
            {
                case PotionType.health:
                    PotionText.Text += "health";
                    break;

                case PotionType.mana:
                    PotionText.Text += "mana";
                    break;

                default:
                    break;
            }
            PotionText.Color = Color.Red;
            infoList.Add(PotionText);
        }
    }
}
