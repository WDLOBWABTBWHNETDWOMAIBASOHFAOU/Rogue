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
    public abstract class Skill : Item
    {//Skills are considerd items to fasilitate picking up and droping them between the skillbar and skill storage (inventory or skillbook). Think of it as scrolls
        
        protected int skillReach;
        public int SkillReach { get { return skillReach; } }

        protected int ManaCost;

        public Skill(int skillReach = 2, int ManaCost = 20, string assetName = "empty:64:64:10:White") : base(assetName)
        {
            this.skillReach = skillReach;
            this.ManaCost = ManaCost;
            SetId();
        }

        protected virtual void SetId()
        {
            id = "Skill";
        }

        #region Serialization
        public Skill(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            skillReach = info.GetInt32("SkillReach");
            ManaCost = info.GetInt32("ManaCost");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SkillReach", skillReach);
            info.AddValue("ManaCost", ManaCost);
            base.GetObjectData(info, context);
        }
        #endregion

        public abstract void DoSkill(Living caster, Living livingTarget, Tile TileTarget);

        public virtual bool SkillValidation(Living caster, Living livingTarget, Tile TileTarget)
        {
            if (caster.CurrentSkill != null)
            { 
                bool manacost = caster.Mana >= ManaCost;
                bool AtH;

                if (livingTarget != null)
                { AtH = AttackEvent.AbleToHit(caster, livingTarget.Tile, skillReach); }
                else if (TileTarget != null)
                { AtH = AttackEvent.AbleToHit(caster, TileTarget, skillReach); }
                else
                { throw new Exception("invalid target"); }

                return manacost && AtH;
            }
            return false;
        }

        public override void DoBonus(Living living)
        {
            //empty because base skills don't have this by deffinition (aura or buff skills however do)
        }

        public override void RemoveBonus(Living living)
        {
            //see DoBonus
        }

        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject SkillReachInfo = new TextGameObject("Arial12", 0, 0, "SkillReachInfo." + this);
            SkillReachInfo.Text = "Reach: " + skillReach.ToString();
            SkillReachInfo.Color = Color.Red;
            SkillReachInfo.Parent = infoList;
            infoList.Add(SkillReachInfo);

            TextGameObject ManaCostInfo = new TextGameObject("Arial12", 0, 0, "ManaCostInfo." + this);
            ManaCostInfo.Text = "ManaCost: " + ManaCost.ToString();
            ManaCostInfo.Color = Color.Red;
            ManaCostInfo.Parent = infoList;
            infoList.Add(ManaCostInfo);
        }
    }
}
