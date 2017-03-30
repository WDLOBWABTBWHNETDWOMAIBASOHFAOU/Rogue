using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class MeleeWeapon : WeaponEquipment
    {
        private int strRequirement,dexRequirement;
        private float strScaling, dexScaling;

        #region Serialization
        public MeleeWeapon(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            strRequirement = info.GetInt32("strRequirement");
            dexRequirement = info.GetInt32("dexRequirement");
            strScaling = (float)info.GetDouble("strScaling");
            dexScaling = (float)info.GetDouble("dexScaling");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("strRequirement", strRequirement);
            info.AddValue("dexRequirement", dexRequirement);
            info.AddValue("strScaling", strScaling);
            info.AddValue("dexScaling", dexScaling);
        }
        #endregion

        public MeleeWeapon(string id, int baseValue, int strRequirement=1, int dexRequirement=1, float strScaling=0.2f, float dexScaling=0.1f, int stackSize = 1, int reach = 1, string assetName= "Sprites/Weapons/urand_doom_knight", int layer = 0) : base(assetName, id, baseValue, DamageType.Physical, stackSize, reach, layer)
        {
            this.strRequirement = strRequirement;
            this.dexRequirement = dexRequirement;
            this.strScaling = strScaling;
            this.dexScaling = dexScaling;
            hitSound = "Sounds/SwordHit";
        }

        protected override bool MeetsRequirements(Living l)
        {
            if (l.GetStat(Stat.Strength) >= strRequirement && l.GetStat(Stat.Dexterity) >= dexRequirement)
                return true;
            else
                return false;
        }

        protected override double AttackValue(Living user)
        {
            double aVal=0;
            if (MeetsRequirements(user))
                aVal = user.CalculateValue(baseValue, user.GetStat(Stat.Strength) - strRequirement, strScaling, 0, user.GetStat(Stat.Dexterity) - dexRequirement, dexScaling);
            else
                aVal = user.CalculateValue(baseValue,strRequirement - user.GetStat(Stat.Strength), strScaling, 0,dexRequirement- user.GetStat(Stat.Dexterity), dexScaling);
            return aVal;
        }

        public override void ItemInfo(ItemSlot caller)
        {
            displayedName = "Sword"; 
            base.ItemInfo(caller);
        }
    }
}
