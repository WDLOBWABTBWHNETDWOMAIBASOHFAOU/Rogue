using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wink
{
    [Serializable]
    public class LevelUpdatedEvent : Event
    {
        private Level updatedLevel;

        public LevelUpdatedEvent(Level level) : base()
        {
            updatedLevel = level;
        }

        public LevelUpdatedEvent(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            updatedLevel = info.GetValue("updatedLevel", typeof(Level)) as Level;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("updatedLevel", updatedLevel);
            base.GetObjectData(info, context);
        }

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override void OnClientReceive(LocalClient client)
        {
            Dictionary<Guid, Dictionary<string, object>> guiStates = new Dictionary<Guid, Dictionary<string, object>>();
            if (client.Level != null)
            {
                foreach (GameObject obj in client.Level.FindAll(obj => obj is IGUIGameObject))
                {
                    Dictionary<string, object> guiState = new Dictionary<string, object>();
                    (obj as IGUIGameObject).CleanupGUI(guiState);
                    guiStates.Add(obj.GUID, guiState);
                }
            }   
            
            client.Level = updatedLevel;

            foreach (GameObject obj in updatedLevel.FindAll(obj => obj is IGUIGameObject))
            {
                Dictionary<string, object> guiState;
                guiStates.TryGetValue(obj.GUID, out guiState);
                
                (obj as IGUIGameObject).InitGUI(guiState ?? new Dictionary<string, object>());
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
