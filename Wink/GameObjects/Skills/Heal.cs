using Microsoft.Xna.Framework;
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
        int skillPower;
        public Heal(int skillPower = 15,int skillReach = 2, int ManaCost = 20, string assetName = "empty:64:64:10:White") : base(skillReach,ManaCost,assetName)
        {
            this.skillPower = skillPower;
        }

        protected override void SetId()
        {
            id = "Heal Spell";
        }

        #region Serialization
        public Heal(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            skillPower = info.GetInt32("skillPower");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("skillPower", skillPower);
            base.GetObjectData(info, context);
        }
        #endregion

        public override void DoSkill(Living caster, Living livingTarget, Tile TileTarget)
        {
            if(livingTarget != null)
            {
                caster.Mana -= ManaCost;
                livingTarget.Health += skillPower;
                if(livingTarget.Health > livingTarget.MaxHealth)
                {
                    livingTarget.Health = livingTarget.MaxHealth;
                }
            }
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);
            
            TextGameObject HealInfo = new TextGameObject("Arial12", 0, 0, "HealInfo." + this);
            HealInfo.Text = "Heals the target for " + skillPower + " points";
            HealInfo.Color = Color.Red;
            HealInfo.Parent = infoList;
            infoList.Add(HealInfo);

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
