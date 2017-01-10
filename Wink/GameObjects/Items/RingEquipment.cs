using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
