using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class ChangedSkillEvent : Event
    {
        Player player;
        Skill newSelectedSkill;
        public ChangedSkillEvent(Player player, Skill newSelectedSkill) : base()
        {
            this.player = player;
            this.newSelectedSkill = newSelectedSkill;
        }

        #region Serialization
        public ChangedSkillEvent(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            player = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("playerGUID"))) as Player;
            newSelectedSkill = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("newSelectedSkillGUID"))) as Skill;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("playerGUID", player.GUID.ToString());
            info.AddValue("newSelectedSkillGUID", newSelectedSkill.GUID.ToString());
            base.GetObjectData(info, context);
        }
        #endregion

        public override bool GUIDSerialization
        {
            get
            {
                return true;
            }
        }

        public override bool OnClientReceive(LocalClient client)
        {
            throw new NotImplementedException();
        }

        public override bool OnServerReceive(LocalServer server)
        {
            if(player.CurrentSkill != newSelectedSkill)
            {
                player.CurrentSkill = newSelectedSkill;
            }
            else
            {
                player.CurrentSkill = null;
            }
            NonAnimationSoundEvent selectSkillSound = new NonAnimationSoundEvent("Sounds/CLICK14A",true,player.Id);
            LocalServer.SendToClients(selectSkillSound);
            return true;
        }

        public override bool Validate(Level level)
        {
            if(newSelectedSkill != null)
            {
                return true;
            }
            return false;
        }
    }
}
