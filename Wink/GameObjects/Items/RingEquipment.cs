using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class RingEquipment : Equipment
    {
        public enum Rings
        {
            intelligence,
            dexterity,
            strength,
            health,
            none
        }

        protected bool multiplier;
        protected int ringValue;
        protected Rings ringType;

        public RingEquipment(string assetName, int stackSize = 1, int layer = 0, string id = "", Rings ringType = Rings.none, bool multiplier = false) : base(assetName, stackSize, layer, id)
        {
            this.ringType = ringType;
            this.multiplier = multiplier;
        }

        public int RingValue
        {
            get { return ringValue; }
            set { ringValue = value; }
        }

        public Rings RingType
        {
            get { return ringType; }
            set { ringType = value; }
        }
    }
}
