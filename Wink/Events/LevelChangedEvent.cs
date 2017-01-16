using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public override bool GUIDSerialization
        {
            get { return true; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            foreach (GameObject go in changedObjects)
            {
                client.Replace(go);

                //if (go is IGUIGameObject)
                    //(go as IGUIGameObject).InitGUI();
                    //TODO: enable this again but make replace return the 
            }
                
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
