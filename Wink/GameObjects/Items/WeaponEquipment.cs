using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class WeaponEquipment : Equipment
    {
        int baseDamage;
        public int BaseDamage { get { return baseDamage; } }
        double scalingFactor;
        public double ScalingFactor { get { return scalingFactor; } }
        int reach;
        public int Reach { get { return reach; } }
        int strRequirement;
        public int StrRequirement { get { return strRequirement; } }

        public WeaponEquipment(int baseDamage, double scalingFactor, int reach, int strRequirement,string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.baseDamage = baseDamage;
            this.scalingFactor = scalingFactor;
            this.reach = reach;
            this.strRequirement = strRequirement;
        }
    }
}
