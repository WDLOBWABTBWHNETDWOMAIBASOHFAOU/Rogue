
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
    class MagicBolt : Skill
    {
        int skillPower;
        public MagicBolt(int skillPower = 250, int skillReach = 3, int ManaCost = 20, string assetName = "Sprites/Skills/lehudibs_crystal_spear") : base(skillReach, ManaCost, assetName)
        {
            this.skillPower = skillPower;
        }
        protected override void SetId()
        {
            id = "MagicBolt";
        }

        #region Serialization
        public MagicBolt(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            skillPower = info.GetInt32("skillPower");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("skillPower", skillPower);
        }
        #endregion

        public override void DoSkill(Living caster, Living livingTarget, Tile TileTarget)
        {
            if (livingTarget != null)
            {
                caster.Mana -= ManaCost;
                livingTarget.TakeDamage(skillPower, DamageType.Magic);
            }
            else
            {
                throw new Exception("invalid target");
            }
        }
        public override void ItemInfo(ItemSlot caller)
        {
            base.ItemInfo(caller);

            TextGameObject MagicBoltInfo = new TextGameObject("Arial12", 0, 0, "MagicBoltInfo." + this);
            MagicBoltInfo.Text = "A magic bolt with an attackvalue of " + skillPower;
            MagicBoltInfo.Color = Color.Red;
            MagicBoltInfo.Parent = infoList;
            infoList.Add(MagicBoltInfo);
        }
    }
}
