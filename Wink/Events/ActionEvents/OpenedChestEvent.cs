using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class OpenedChestEvent : ActionEvent
    {
        private Container container;
        public OpenedChestEvent(Player player, Container container) : base(player)
        {
            this.container = container;
        }

        #region Serialization
        public OpenedChestEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            container = context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString("containerGUID"))) as Container;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("containerGUID", container.GUID.ToString());
        }
        #endregion

        protected override int Cost
        {
            get
            {
                //cant get it to work correctly regarding showing contents
                //if (container.Closed)
                //{
                //    return Living.BaseActionCost / 2;
                //}
                return 0;
            }
        }

        protected override void DoAction(LocalServer server, HashSet<GameObject> changedObjects)
        {
            container.openingChest();
            changedObjects.Add(container);
            container.Closed = false;
            //TODO: sugetions: 
            //1 init contents here to take specific players luck in to account.
            //2 if keys (for doors and chests) are implemented, add if(locked) with key requirement in validation

        }

        protected override bool ValidateAction(Level level)
        {
            return true;
        }
    }
}
