using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    public enum WeaponType
    {
        melee,
        bow,
        staff,


        gen//ALLWAYS LAST
    }

    [Serializable]
    public class WeaponEquipment : Equipment
    {
        private int reach;
        protected int baseValue;
        protected WeaponType weaponType;
        protected DamageType damageType;
        protected double strScalingFactor;
        protected double dexScalingFactor;
        protected double intScalingFactor;

        public int Reach { get { return reach; } }
        public DamageType GetDamageType { get { return damageType; } }

        /// <summary>
        /// generate random weapon
        /// </summary>
        /// <param name="floorNumber"></param>
        /// <param name="genWType">use this to select a specific type to generate, else leave on WeaponType.gen</param>
        /// <param name="layer"></param>
        /// <param name="stackSize"></param>
        public WeaponEquipment(int floorNumber, WeaponType genWType = WeaponType.gen, int layer = 0, int stackSize = 1) : base(floorNumber, layer, stackSize)
        {
            SetRandomWeapon(floorNumber, genWType);
            SetId();
        }

        /// <summary>
        /// generate a specific weapon
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="id"></param>
        /// <param name="baseValue"></param>
        /// <param name="damageType"></param>
        /// <param name="weaponType"></param>
        /// <param name="stackSize"></param>
        /// <param name="reach"></param>
        /// <param name="strRequirement"></param>
        /// <param name="dexRequirement"></param>
        /// <param name="intRequirement"></param>
        /// <param name="strScalingFactor"></param>
        /// <param name="dexScalingFactor"></param>
        /// <param name="intScalingFactor"></param>
        /// <param name="layer"></param>
        public WeaponEquipment(string assetName, string id, int baseValue, DamageType damageType, WeaponType weaponType, int stackSize = 1, int reach=1, int strRequirement=0, int dexRequirement = 0, int intRequirement=0, double strScalingFactor=0, double dexScalingFactor=0, double intScalingFactor=0, int layer = 0) : base(assetName,id, layer, stackSize,strRequirement,dexRequirement,intRequirement)
        {
            this.reach = reach;
            this.baseValue = baseValue;
            this.damageType = damageType;
            this.weaponType = weaponType;
            this.strScalingFactor = strScalingFactor;
            this.dexScalingFactor = dexScalingFactor;
            this.intScalingFactor = intScalingFactor;
        }

        void SetId()
        {
            id = weaponType.ToString() + ":" + baseValue + ":" + reach;
        }

        void SetRandomWeapon(int floorNumber, WeaponType genWType)
        {
            weaponType = genWType;
            if(genWType == WeaponType.gen)
            {
                //select random armorType
                Array wTypeValues = Enum.GetValues(typeof(WeaponType));
                weaponType = (WeaponType)wTypeValues.GetValue(GameEnvironment.Random.Next(wTypeValues.Length-1));
            }

            // some values used to calculate final values
            int someBaseValue = 25;
            int baseBonusValue = 100 * floorNumber;
            int highPriority = 10;
            int mediumPriority = 20;
            int lowPriority = 30;
            double strFactor;
            double dexFactor;
            double intFactor;

            // final values based on weaponType
            switch (weaponType)
            {
                case WeaponType.melee:
                    //base values
                    reach = 1;
                    damageType = DamageType.Physical;
                    baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

                    //scaling values
                    strFactor = GameEnvironment.Random.Next(0, lowPriority);
                    strScalingFactor = strFactor / 100;
                    dexFactor = GameEnvironment.Random.Next(0, highPriority);
                    dexScalingFactor =  dexFactor/ 100;
                    intScalingFactor = 0;

                    //requirement values
                    strRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
                    dexRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
                    intRequirement =0;

                    spriteAssetName = "empty:64:64:10:DarkGoldenrod";//TODO: replace by correct spritename
                    break;
                case WeaponType.bow:
                    //base values
                    reach = GameEnvironment.Random.Next(3,5);
                    damageType = DamageType.Physical;
                    baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

                    //scaling values
                    strFactor = GameEnvironment.Random.Next(0, highPriority);
                    strScalingFactor = strFactor / 100;
                    dexFactor = GameEnvironment.Random.Next(0, lowPriority);
                    dexScalingFactor = dexFactor / 100;
                    intScalingFactor = 0;

                    //requirement values
                    strRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
                    dexRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
                    intRequirement = 0;

                    spriteAssetName = "spr_bow";//TODO: replace by correct spritename
                    break;
                case WeaponType.staff:
                    //base values
                    reach = GameEnvironment.Random.Next(2, 4);
                    damageType = DamageType.Magic;
                    baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

                    //scalingvalues
                    strScalingFactor = 0;
                    dexScalingFactor = 0;
                    intFactor = GameEnvironment.Random.Next(0, lowPriority);
                    intScalingFactor = intFactor/ 100;

                    //requirement values
                    strRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
                    dexRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
                    intRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
                    spriteAssetName = "empty:64:64:10:LightSkyBlue";//TODO: replace by correct spritename
                    break;
                default:
                    throw new Exception("invalid weaponType");
            }

        }

        #region Serialization
        public WeaponEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            reach = info.GetInt32("reach");
            baseValue = info.GetInt32("baseValue");
            damageType = (DamageType)info.GetValue("damageType", typeof(DamageType));
            weaponType = (WeaponType)info.GetValue("weaponType", typeof(WeaponType));
            strScalingFactor = info.GetDouble("strScalingFactor");
            dexScalingFactor = info.GetDouble("dexScalingFactor");
            intScalingFactor = info.GetDouble("intScalingFactor");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("reach", reach);
            info.AddValue("baseValue", baseValue);
            info.AddValue("damageType", damageType);
            info.AddValue("weaponType", weaponType);
            info.AddValue("strScalingFactor", strScalingFactor);
            info.AddValue("dexScalingFactor", dexScalingFactor);
            info.AddValue("intScalingFactor", intScalingFactor);
        }
        #endregion

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            Player player = caller.GameWorld.Find(Player.LocalPlayerName) as Player;
            TextGameObject dType = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "DamageTypeInfo." + this);
            dType.Text = GetDamageType + " Damage: " + baseValue + " + " + (Value(player) -baseValue);
            dType.Color = Color.Red;
            dType.Parent = infoList;
            infoList.Children.Insert(1, dType);

            TextGameObject scalingText = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "scalingInfoText." + this);
            scalingText.Text = "DamageBonus:";
            switch (damageType)
            {
                case DamageType.Physical:
                    scalingText.Text += " str " + (int)(strScalingFactor*baseValue) + " dex " + (int)(dexScalingFactor * baseValue);
                    break;
                case DamageType.Magic:
                    scalingText.Text += " int " + (int)(intScalingFactor * baseValue);
                    break;
                default:
                    throw new Exception("invalid damageType");
            }
            scalingText.Color = Color.Red;
            scalingText.Parent = infoList;
            infoList.Children.Insert(2, scalingText);


            TextGameObject scalingStatText = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "scalingStatInfoText." + this);
            scalingStatText.Text = "Statbonus:";
            switch (damageType)
            {
                case DamageType.Physical:
                    scalingStatText.Text += " str " + string.Format("{0:N2}", strScalingFactor) + " dex " + string.Format("{0:N2}", dexScalingFactor);
                    break;
                case DamageType.Magic:
                    scalingStatText.Text += " int " + string.Format("{0:N2}", intScalingFactor);
                    break;
                default:
                    throw new Exception("invalid damageType");
            }
            scalingStatText.Color = Color.Red;
            scalingStatText.Parent = infoList;
            infoList.Children.Insert(3, scalingStatText);

            TextGameObject reachText = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "ReachInfoText." + this);
            reachText.Text = "Reach: " + reach;
            reachText.Color = Color.Red;
            reachText.Parent = infoList;
            infoList.Add(reachText);
        }

        public int Value(Living l)
        {
            int value;
            switch (damageType)
            {
                case DamageType.Physical:
                    if (MeetsRequirements(l))
                    {
                        value = (int)l.CalculateValue(baseValue, l.Strength - strRequirement, strScalingFactor, 0, l.Dexterity - dexRequirement, dexScalingFactor);
                        return value;
                    }
                    else
                    {
                        value = (int)l.CalculateValue(baseValue, strRequirement - l.Strength, -strScalingFactor, 0, dexRequirement - l.Dexterity, -dexScalingFactor);
                        return value;
                    }
                case DamageType.Magic:
                    if (MeetsRequirements(l))
                    {
                        value = (int)l.CalculateValue(baseValue, l.Intelligence - intRequirement, intScalingFactor);
                        return value;
                    }
                    else
                    {
                        value = (int)l.CalculateValue(baseValue, intRequirement - l.Intelligence, -intScalingFactor);
                        return value;
                    }
                default:
                    throw new Exception("invalid damageType");
            }
        }
    }
}
