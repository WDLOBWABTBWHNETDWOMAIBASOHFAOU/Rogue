using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    public enum RingType
    {
        Intelligence,
        Dexterity,
        Strength,
        Health,
        None
    }

    [Serializable]
    class RingEquipment : Equipment
    {
        protected bool multiplier;
        protected double ringValue;
        protected RingType ringType;

        public RingEquipment(double ringValue, RingType ringType, string assetName, bool multiplier = false, int stackSize = 1, int layer = 0, string id = "") : base(assetName, stackSize, layer, id)
        {
            this.ringType = ringType;
            this.ringValue = ringValue;
            this.multiplier = multiplier;
        }

        #region Serialization
        public RingEquipment(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            multiplier = info.GetBoolean("multiplier");
            ringValue = info.GetDouble("ringValue");
            ringType = (RingType)info.GetValue("ringType", typeof(RingType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("multiplier", multiplier);
            info.AddValue("ringValue", ringValue);
            info.AddValue("ringType", ringType);
        }
        #endregion
        
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
