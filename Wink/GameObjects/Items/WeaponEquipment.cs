using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public class WeaponEquipment : Equipment
    {
        int reach;
        public int Reach { get { return reach; } }
        protected int baseValue;
        protected DamageType damageType;
        public DamageType DamageType { get { return damageType; } }
        protected double strScalingFactor;
        protected double dexScalingFactor;
        protected double intScalingFactor;

        public WeaponEquipment(string assetName, string id, int baseValue, DamageType damageType, int stackSize = 1, int reach=1, int strRequirement=0, int dexRequirement = 0, int intRequirement=0, double strScalingFactor=0, double dexScalingFactor=0, double intScalingFactor=0, int layer = 0) : base(assetName,id, layer, stackSize,strRequirement,dexRequirement,intRequirement)
        {
            this.reach = reach;
            this.baseValue = baseValue;
            this.damageType = damageType;
            this.strScalingFactor = strScalingFactor;
            this.dexScalingFactor = dexScalingFactor;
            this.intScalingFactor = intScalingFactor;
        }

        public int Value(Living l)
        {
            int value;
            switch (damageType)
            {
                case DamageType.physical:
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
                case DamageType.magic:
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
