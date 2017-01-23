
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class TestSkill : Skill
    {

        public TestSkill() : base()
        {
        }

        #region Serialization
        public TestSkill(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        public override void DoSkill(Living caster, Living livingTarget, Tile TileTarget)
        {
            if (livingTarget != null)
            {
                caster.Mana -= ManaCost;
                livingTarget.TakeDamage(100, DamageType.Magic);
            }
            else
            {
                throw new Exception("invalid target");
            }
        }

    }
}
