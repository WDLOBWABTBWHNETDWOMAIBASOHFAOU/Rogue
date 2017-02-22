using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class MageWeapon : WeaponEquipment
    {
        private int intRequirement;
        private float intScaling;

        #region Serialization
        public MageWeapon(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            intRequirement = info.GetInt32("intRequirement");
            intScaling = (float)info.GetDouble("intScaling");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("intRequirement", intRequirement);
            info.AddValue("intScaling", intScaling);
        }
        #endregion

        public MageWeapon( string id, int baseValue, int intRequirement = 1, float intScaling=0.3f, int stackSize = 1, int reach = 1, string assetName = "Sprites/Weapons/staff04", int layer = 0) : base(assetName, id, baseValue, DamageType.Magic, stackSize, reach, layer)
        {
            this.intRequirement = intRequirement;
            this.intScaling = intScaling;
            hitSound = "Sounds/FireIgnite";
        }

        protected override bool MeetsRequirements(Living l)
        {
            if (l.Intelligence >= intRequirement)
                return true;
            else
                return false;
        }
        protected override double AttackValue(Living user)
        {
            int aVal = 0;
            if (MeetsRequirements(user))
                user.CalculateValue(baseValue, user.Intelligence - intRequirement, intScaling);
            else
                user.CalculateValue(baseValue, intRequirement - user.Intelligence, intScaling);
            return aVal;
        }
    }
}
