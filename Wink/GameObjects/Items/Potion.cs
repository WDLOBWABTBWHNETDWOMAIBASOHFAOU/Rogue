using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using System;

namespace Wink
{
    enum PotionType
    {
        Health,
        Mana,

        gen//ALWAYS LAST
    }
    enum PotionPower
    {
        Minor,
        Small,
        Normal,
        Large,
        Greater,
        Huge
    }

    [Serializable]
    class Potion : Item            
    {
        private PotionType potionType;
        private int potionValue;

        public PotionType GetPotionType { get { return potionType; } }
        public int PotionValue { get { return potionValue; } }
        public PotionPower potionPower;

        public Potion(int floorNumber, int stackSize = 10, PotionType genType = PotionType.gen, int layer = 0) : base(floorNumber,stackSize,layer)
        {
            SetRandomPotion(floorNumber,genType);
            setPotionPower();
            SetId();
        }

        public Potion(string assetName, PotionType potionType, PotionPower potionPower,int stackCount=1, int stackSize = 10, int layer = 0):base(assetName, stackSize, layer)
        {
            this.potionType = potionType;
            this.potionPower = potionPower;
            this.stackCount = stackCount;
            setPotionPower();
            SetId();
        }

        void SetRandomPotion(int floorNumber,PotionType genType)
        {
            // set type
            potionType = genType;
            if(potionType == PotionType.gen)
            {
                Array pTypeValues = Enum.GetValues(typeof(PotionType));
                potionType = (PotionType)pTypeValues.GetValue(GameEnvironment.Random.Next(pTypeValues.Length-1));
            }
            switch (potionType)
            {
                case PotionType.Health:
                    spriteAssetName = "Sprites/Potions/ruby";
                    break;
                case PotionType.Mana:
                    spriteAssetName = "Sprites/Potions/brilliant_blue";
                    break;
                default:
                    throw new Exception("invalid potionType");
            }

            // set power
            Array pPowerValues = Enum.GetValues(typeof(PotionPower));
            int pPVNumber = floorNumber;
            if (pPVNumber > pPowerValues.Length)
                pPVNumber = pPowerValues.Length;
            potionPower = (PotionPower)pPowerValues.GetValue(GameEnvironment.Random.Next(pPVNumber));

            //set number of
            stackCount = GameEnvironment.Random.Next(1, (getStackSize*(floorNumber+1)));
            if(stackCount > getStackSize)
            {
                stackCount = getStackSize;
            }
        }

        void SetId()
        {
            id = potionPower.ToString() + " " + potionType.ToString() + "Potion";
        }

        public int setPotionPower()
        {
            switch (potionPower)
            {
                #region PotionPower
                case PotionPower.Minor:
                    potionValue = 10;
                    break;
                case PotionPower.Small:
                    potionValue = 25;
                    break;
                case PotionPower.Normal:
                    potionValue = 50;
                    break;
                case PotionPower.Large:
                    potionValue = 100;
                    break;
                case PotionPower.Greater:
                    potionValue = 250;
                    break;
                case PotionPower.Huge:
                    potionValue = 500;
                    break;
                default:
                    throw new Exception("invalid potionPower");
                    #endregion
            }
            return potionValue;
        }

        #region Serialization
        public Potion(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            potionType = (PotionType)info.GetValue("potionType", typeof(PotionType));
            potionPower = (PotionPower)info.GetValue("potionPower", typeof(PotionPower));
            potionValue = info.GetInt32("potionValue");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("potionType", potionType);
            info.AddValue("potionValue", potionValue);
            info.AddValue("potionPower", potionPower);
        }
        #endregion

        public override void ItemAction(Living caller)
        {
            TakenPotionEvent p = new TakenPotionEvent(caller as Player, this);
            Server.Send(p);
        }

        public override void ItemInfo(ItemSlot caller)
        {
            displayedName = potionPower.ToString() + " " + potionType.ToString() + " Potion";
            base.ItemInfo(caller);

            TextGameObject PotionText = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "PotionText." + this);

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

        public override void DoBonus(Living living)
        {
            throw new NotImplementedException();
        }

        public override void RemoveBonus(Living living)
        {
            throw new NotImplementedException();
        }
    }
}
