using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wink
{
    [Serializable]
    class PreLevelChangedEvent : Event
    {
        private List<string> fullySerializedGUIDs;

        public PreLevelChangedEvent(List<GameObject> fullySerialized) : this(fullySerialized.ConvertAll(obj => obj.GUID.ToString()))
        { }

        public PreLevelChangedEvent(List<string> fullySerializedGUIDs)
        {
            this.fullySerializedGUIDs = fullySerializedGUIDs;
        }

        #region Serialization
        public PreLevelChangedEvent(SerializationInfo info, StreamingContext context)
        {
            fullySerializedGUIDs = info.GetValue("fullySerializedGUIDs", typeof(List<string>)) as List<string>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("fullySerializedGUIDs", fullySerializedGUIDs);
        }
        #endregion

        public override bool GUIDSerialization
        {
            get { return false; }
        }

        public override bool OnClientReceive(LocalClient client)
        {
            //TODO: store this list (in the serializationHelper?)

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
