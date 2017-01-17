using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    class TestWeapon : WeaponEquipment
    {
        public TestWeapon(int Level = 1, string assetName = "TestStaff", string id = "TestStaff", int baseValue = 60, DamageType damageType = DamageType.Magic, int stackSize = 1, int reach = 4, int strRequirement = 1, int dexRequirement = 1, int intRequirement = 5, double strScalingFactor = 0, double dexScalingFactor = 0, double intScalingFactor = 0, int layer = 0) : base(assetName, id, baseValue, damageType, stackSize, reach, strRequirement, dexRequirement, intRequirement, strScalingFactor, dexScalingFactor, intScalingFactor, layer)
        {
            
        }
    }
}
