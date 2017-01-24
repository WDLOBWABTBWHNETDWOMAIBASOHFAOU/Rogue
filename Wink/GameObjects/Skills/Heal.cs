using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class Heal : Skill
    {
        int skillStrenght;
        public Heal(int skillStrenght = 15,int skillReach = 2, int ManaCost = 20, string assetName = "empty:64:64:10:White") : base(skillReach,ManaCost,assetName)
        {
            this.skillStrenght = skillStrenght;
            SetId();
        }

        protected override void SetId()
        {
            id = "Heal Spell";
        }

        #region Serialization
        public Heal(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            skillStrenght = info.GetInt32("skillStrenght");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("skillStrenght", skillStrenght);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void DoSkill(Living caster, Living livingTarget, Tile TileTarget)
        {
            if(livingTarget != null)
            {
                caster.Mana -= ManaCost;
                livingTarget.Health += skillStrenght;
                if(livingTarget.Health > livingTarget.MaxHealth)
                {
                    livingTarget.Health = livingTarget.MaxHealth;
                }
            }
        }

        public override bool SkillValidation(Living caster, Living livingTarget, Tile TileTarget)
        {
            if(livingTarget !=null)
            {
                return base.SkillValidation(caster, livingTarget, TileTarget);
            }
            return false;
        }
    }
}
