using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class RangedWeapon:WeaponEquipment
    {
        private int strRequirement, dexRequirement;
        private float strScaling, dexScaling;

        #region Serialization
        public RangedWeapon(SerializationInfo info, StreamingContext context) : base(info, context)
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

        public RangedWeapon(string id, int baseValue, int strRequirement = 1, int dexRequirement = 1, float strScaling = 0.1f, float dexScaling = 0.2f, int stackSize = 1, int reach = 1, string assetName = "Sprites/Weapons/longbow", int layer = 0) : base(assetName, id, baseValue, DamageType.Physical, stackSize, reach, layer)
            {
            this.strRequirement = strRequirement;
            this.dexRequirement = dexRequirement;
            this.strScaling = strScaling;
            this.dexScaling = dexScaling;
            hitSound = "Sounds/Arrow";
        }

        protected override bool MeetsRequirements(Living l)
        {
            if (l.Strength >= strRequirement && l.Dexterity >= dexRequirement)
                return true;
            else
                return false;
        }

        protected override double AttackValue(Living user)
        {
            int aVal = 0;
            if (MeetsRequirements(user))
                user.CalculateValue(baseValue, user.Strength - strRequirement, strScaling, 0, user.Dexterity - dexRequirement, dexScaling);
            else
                user.CalculateValue(baseValue, strRequirement - user.Strength, strScaling, 0, dexRequirement - user.Dexterity, dexScaling);
            return aVal;
        }
    }
}
