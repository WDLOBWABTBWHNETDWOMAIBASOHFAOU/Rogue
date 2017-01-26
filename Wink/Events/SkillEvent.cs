using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class SkillEvent : ActionEvent
    {
        protected GameObject target; //can be anything form an enemy reciving a powerfull attack, a friendly reciving a heal/buff or a tile as center for an EoA spell, etc.
        public SkillEvent(Player player, GameObject target) : base(player)
        {
            this.target = target;
        }

        #region Serialization
        public SkillEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            target = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("DefenderGUID"))) as GameObject;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //This event can only be sent from client to server, therefore ID based serialization is used.
            info.AddValue("DefenderGUID", target.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return null; //Irrelevant because client->server
        }

        protected override int Cost
        {
            get
            {
                return Living.BaseActionCost;
            }
        }

        protected override void DoAction(LocalServer server)
        {
            player.CurrentSkill.DoSkill(player, target as Living, target as Tile);
        }

        protected override bool ValidateAction(Level level)
        {
            if (player.CurrentSkill != null)
            {
                return player.CurrentSkill.SkillValidation(player, target as Living, target as Tile); //also handles whithin reach
            }
            return false;
        }
    }
}
