using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Wink
{
    [Serializable]
    public abstract class WeaponEquipment : Equipment
    {
        private int reach;
        protected int baseValue;
        protected DamageType damageType;
        protected string hitSound;
        public string HitSound { get { return hitSound; } }
        
        public int Reach { get { return reach; } }
        public DamageType GetDamageType { get { return damageType; } }

        /// <summary>
        /// generate random weapon
        /// </summary>
        /// <param name="floorNumber"></param>
        /// <param name="genWType">use this to select a specific type to generate, else leave on WeaponType.gen</param>
        /// <param name="layer"></param>
        /// <param name="stackSize"></param>
        //public WeaponEquipment(int floorNumber, WeaponType genWType = WeaponType.gen, int layer = 0, int stackSize = 1) : base(floorNumber, layer, stackSize)
        //{
        //    SetRandomWeapon(floorNumber, genWType);
        //    SetId();
        //}

        /// <summary>
        /// generate a specific weapon
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="id"></param>
        /// <param name="baseValue"></param>
        /// <param name="damageType"></param>
        /// <param name="stackSize"></param>
        /// <param name="reach"></param>
        /// <param name="layer"></param>
        public WeaponEquipment(string assetName, string id, int baseValue, DamageType damageType, int stackSize = 1, int reach=1, int layer = 0) : base(assetName,id, layer, stackSize)
        {
            this.reach = reach;
            this.baseValue = baseValue;
            this.damageType = damageType;
        }

        void SetId()
        {
            id = GetType().ToString() + ":" + baseValue + ":" + reach;
        }

        //void SetRandomWeapon(int floorNumber, WeaponType genWType)
        //{
        //    weaponType = genWType;
        //    if(genWType == WeaponType.gen)
        //    {
        //        //select random armorType
        //        Array wTypeValues = Enum.GetValues(typeof(WeaponType));
        //        weaponType = (WeaponType)wTypeValues.GetValue(GameEnvironment.Random.Next(wTypeValues.Length-1));
        //    }

        //    // some values used to calculate final values
        //    int someBaseValue = 25;
        //    int baseBonusValue = 100 * floorNumber;
        //    int highPriority = 10;
        //    int mediumPriority = 20;
        //    int lowPriority = 30;
        //    double strFactor;
        //    double dexFactor;
        //    double intFactor;

        //    // final values based on weaponType
        //    switch (weaponType)
        //    {
        //        case WeaponType.melee:
        //            //base values
        //            reach = 1;
        //            damageType = DamageType.Physical;
        //            baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

        //            //scaling values
        //            strFactor = GameEnvironment.Random.Next(0, lowPriority);
        //            strScalingFactor = strFactor / 100;
        //            dexFactor = GameEnvironment.Random.Next(0, highPriority);
        //            dexScalingFactor =  dexFactor/ 100;
        //            intScalingFactor = 0;

        //            //requirement values
        //            strRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
        //            dexRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
        //            intRequirement =0;
                    
        //            hitSound = "Sounds/SwordHit";
        //            spriteAssetName = "Sprites/Weapons/urand_doom_knight"; //TODO: replace by correct spritename
        //            break;
        //        case WeaponType.bow:
        //            //base values
        //            reach = GameEnvironment.Random.Next(3,5);
        //            damageType = DamageType.Physical;
        //            baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

        //            //scaling values
        //            strFactor = GameEnvironment.Random.Next(0, highPriority);
        //            strScalingFactor = strFactor / 100;
        //            dexFactor = GameEnvironment.Random.Next(0, lowPriority);
        //            dexScalingFactor = dexFactor / 100;
        //            intScalingFactor = 0;

        //            //requirement values
        //            strRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
        //            dexRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
        //            intRequirement = 0;

        //            spriteAssetName = "Sprites/Weapons/longbow";//TODO: replace by correct spritename
        //            hitSound = "Sounds/Arrow";
        //            break;
        //        case WeaponType.staff:
        //            //base values
        //            reach = GameEnvironment.Random.Next(2, 4);
        //            damageType = DamageType.Magic;
        //            baseValue = someBaseValue + MediumBonusValue(baseBonusValue);

        //            //scalingvalues
        //            strScalingFactor = 0;
        //            dexScalingFactor = 0;
        //            intFactor = GameEnvironment.Random.Next(0, lowPriority);
        //            intScalingFactor = intFactor/ 100;

        //            //requirement values
        //            strRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
        //            dexRequirement = GameEnvironment.Random.Next(baseValue) / lowPriority;
        //            intRequirement = GameEnvironment.Random.Next(baseValue) / highPriority;
        //            spriteAssetName = "Sprites/Weapons/staff04";//TODO: replace by correct spritename
        //            hitSound = "Sounds/FireIgnite";

        //            break;
        //        default:
        //            throw new Exception("invalid weaponType");
        //    }

        //}

        #region Serialization
        public WeaponEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            reach = info.GetInt32("reach");
            baseValue = info.GetInt32("baseValue");
            damageType = (DamageType)info.GetValue("damageType", typeof(DamageType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("reach", reach);
            info.AddValue("baseValue", baseValue);
            info.AddValue("damageType", damageType);
        }
        #endregion

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            Player player = caller.GameWorld.Find(Player.LocalPlayerName) as Player;

            TextGameObject reachText = new TextGameObject("Arial12", cameraSensitivity: 0, layer: 0, id: "ReachInfoText." + this);
            reachText.Text = "Reach: " + reach;
            reachText.Color = Color.Red;
            reachText.Parent = infoList;
            infoList.Add(reachText);
        }
        
        public override void DoBonus(Living living)
        {
            living.Reach = reach;
        }
        public override void RemoveBonus(Living living)
        {
            living.Reach = living.BaseReach;
        }

        //virtual so special weapons (2+ damageTypes for example) can alter the Attack Method
        public virtual void Attack(Living user, Living target)
        {
            target.TakeDamage(AttackValue(user), damageType);
        }
        protected abstract double AttackValue(Living user);
    }
}
