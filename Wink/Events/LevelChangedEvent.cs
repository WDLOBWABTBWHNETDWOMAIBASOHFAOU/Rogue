using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    class LevelChangedEvent : Event
    {
        private List<GameObject> changedObjects;

        public LevelChangedEvent(List<GameObject> changedObjects)
        { 
            this.changedObjects = changedObjects;
        }

        public LevelChangedEvent(HashSet<GameObject> changedObjects) : this(changedObjects.ToList())
        { }

        #region Serialization
        public LevelChangedEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            changedObjects = info.GetValue("changedObjects", typeof(List<GameObject>)) as List<GameObject>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("changedObjects", changedObjects);
            base.GetObjectData(info, context);
        }
        #endregion

        public override List<Guid> GetFullySerialized(Level level)
        {
            return changedObjects.ConvertAll(obj => obj.GUID);
        }

        public override bool OnClientReceive(LocalClient client)
        {
            foreach (GameObject go in changedObjects)
            {
                Dictionary<string, object> guiState = new Dictionary<string, object>();
                if (go is IGUIGameObject)
                {
                    IGUIGameObject gui = client.Level.Find(obj => obj.GUID == go.GUID) as IGUIGameObject;
                    if (gui != null)
                        gui.CleanupGUI(guiState);
                }

                client.Replace(go);

                if (go is IGUIGameObject)
                    (go as IGUIGameObject).InitGUI(guiState);
            }
            return true;
        }

        public override bool OnServerReceive(LocalServer server)
        {
            throw new NotImplementedException();
        }

        public override bool Validate(Level level)
        {
            return true;
        }
    }
}
