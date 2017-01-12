using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Wink
{
    public enum RingType
    {
        intelligence,
        dexterity,
        strength,
        health,
        none
    }

    class RingEquipment : Equipment
    {
        protected bool multiplier;
        protected double ringValue;
        protected RingType ringType;

        public RingEquipment(double ringValue, RingType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, id, stackSize, layer)
        {
            this.ringType = ringType;
            this.ringValue = ringValue;
            this.multiplier = multiplier;
        }

        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ringType = (RingType)info.GetValue("ringType", typeof(RingType));
            ringValue = info.GetDouble("ringValue");
            multiplier = info.GetBoolean("multiplier");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RingType", RingType);
            info.AddValue("ringValue", ringValue);
            info.AddValue("multiplier", multiplier);
        }

        public double RingValue
        {
            get { return ringValue; }
            set { ringValue = value; }
        }

        public RingType RingType
        {
            get { return ringType; }
            set { ringType = value; }
        }

        public bool Multiplier { get { return multiplier; } }
    }
}
