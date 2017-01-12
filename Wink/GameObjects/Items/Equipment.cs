using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Wink
{
    public enum DamageType
    {
        physical,
        magic,
        nondamage
    }


    public class Equipment:Item
    {
        protected int strRequirement;
        protected int dexRequirement;
        protected int intRequirement;

        public Equipment(string assetName, string id, int layer = 0, int stackSize = 1, int strRequirement = 0, int dexRequirement = 0, int intRequirement = 0) : base(assetName, stackSize, layer, id)
        {
            this.strRequirement = strRequirement;
            this.dexRequirement = dexRequirement;
            this.intRequirement = intRequirement;
        }

        public Equipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            strRequirement = info.GetInt32("strRequirement");
            dexRequirement = info.GetInt32("dexRequirement");
            intRequirement = info.GetInt32("intRequirement");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("strRequirement", strRequirement);
            info.AddValue("dexRequirement", dexRequirement);
            info.AddValue("intRequirement", intRequirement);
        }

        protected bool MeetsRequirements(Living l)
        {
            if (l.Strength >= strRequirement && l.Dexterity >= dexRequirement && l.Intelligence >= intRequirement)
            {
                return true;
            }
            return false;
        }
    }
}
