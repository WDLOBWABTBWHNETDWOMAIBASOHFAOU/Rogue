using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class WeaponEquipment : Equipment
    {
        private int baseDamage;
        private double scalingFactor;
        private int reach;
        private int strRequirement;

        public int BaseDamage { get { return baseDamage; } }
        public double ScalingFactor { get { return scalingFactor; } }
        public int Reach { get { return reach; } }
        public int StrRequirement { get { return strRequirement; } }

        public WeaponEquipment(int baseDamage, double scalingFactor, int reach, int strRequirement, string assetName, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.baseDamage = baseDamage;
            this.scalingFactor = scalingFactor;
            this.reach = reach;
            this.strRequirement = strRequirement;
        }

        #region Serialization
        public WeaponEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            baseDamage = info.GetInt32("baseDamage");
            scalingFactor = info.GetDouble("scalingFactor");
            reach = info.GetInt32("reach");
            strRequirement = info.GetInt32("strRequirement");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("baseDamage", baseDamage);
            info.AddValue("scalingFactor", scalingFactor);
            info.AddValue("reach", reach);
            info.AddValue("strRequirement", strRequirement);
        }
        #endregion
    }
}
