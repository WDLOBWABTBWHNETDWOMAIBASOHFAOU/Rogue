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
        protected int ringValue;
        protected RingType ringType;

        public RingEquipment(string assetName, int stackSize = 1, int layer = 0, string id = "", RingType ringType = RingType.none, bool multiplier = false) : base(assetName, stackSize, layer, id)
        {
            this.ringType = ringType;
            this.multiplier = multiplier;
        }

        public int RingValue
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
