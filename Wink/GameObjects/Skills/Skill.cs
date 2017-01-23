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
        
        protected int SkillReach;
        protected int ManaCost;

        public Skill(int SkillReach = 3, int ManaCost = 10, string assetName = "empty:64:64:10:White") : base(assetName)
        {
            this.SkillReach = SkillReach;
            this.ManaCost = ManaCost;
            SetId();
        }

        protected virtual void SetId()
        {
            id = "TestSkill";
        }

        #region Serialization
        public Skill(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SkillReach = info.GetInt32("SkillReach");
            ManaCost = info.GetInt32("ManaCost");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SkillReach", SkillReach);
            info.AddValue("ManaCost", ManaCost);
            base.GetObjectData(info, context);
        }
        #endregion

        public abstract void DoSkill(Living caster, Living livingTarget, Tile TileTarget);

        public bool SkillValidation(Living caster, Living livingTarget, Tile TileTarget)
        {
            if(caster.CurrentSkill!=null)
            { 
                bool manacost = caster.Mana >= ManaCost;
                bool AtH;

                if (livingTarget != null)
                { AtH = AttackEvent.AbleToHit(caster, livingTarget.Tile, SkillReach); }
                else if (TileTarget != null)
                { AtH = AttackEvent.AbleToHit(caster, TileTarget, SkillReach); }
                else
                { throw new Exception("invalid target"); }

                return manacost && AtH;
            }
            return false;
        }
        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject SkillReachInfo = new TextGameObject("Arial12", 0, 0, "SkillReachInfo." + this);
            SkillReachInfo.Text = "Reach: " + SkillReach.ToString();
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
