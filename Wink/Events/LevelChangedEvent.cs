using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class LevelChangedEvent : Event
    {
        private List<GameObject> changedObjects;

        public LevelChangedEvent(List<GameObject> changedObjects) : base()
        {
            this.changedObjects = changedObjects;
        }

        public LevelChangedEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            changedObjects = info.GetValue("changedObjects", typeof(List<GameObject>)) as List<GameObject>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("changedObjects", changedObjects);
            base.GetObjectData(info, context);
        }

        public override void OnClientReceive(LocalClient client)
        {
            foreach (GameObject go in changedObjects)
                client.Replace(go);
        }

        public override void OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
