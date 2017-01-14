using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    enum PotionType
    {
        Health,
        Mana
    }

    [Serializable]
    class Potion : Item            
    {
        private PotionType potionType;
        private int potionValue;

        public PotionType PotionType { get { return potionType; } }
        public int PotionValue { get { return potionValue; } }

        public Potion(string assetName, string id, PotionType potionType, int potionValue, int stackSize = 10, int layer = 0):base(assetName, stackSize, layer, id)
        {
            this.potionType = potionType;
            this.potionValue = potionValue;
        }

        #region Serialization
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
        #endregion

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
                case PotionType.Health:
                    PotionText.Text += "health";
                    break;

                case PotionType.Mana:
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
