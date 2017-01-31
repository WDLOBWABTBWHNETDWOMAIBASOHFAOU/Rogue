using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Wink
{
    static class SerializationExtensions
    {
        public static SerializationHelper.Variables GetVars(this StreamingContext sc)
        {
            return sc.Context as SerializationHelper.Variables;
        }

        public static bool ContainsKey(this SerializationInfo info, string key)
        {
            foreach (SerializationEntry e in info)
            {
                if (e.Name == key)
                    return true;
            }
            return false;
        }

        public static T TryGUIDThenFull<T>(this SerializationInfo info, StreamingContext context, string variableName) where T : GameObject
        {
            if (!context.GetVars().FullySerializeEverything && info.ContainsKey(variableName + "GUID"))
            {
                return context.GetVars().Local.GetGameObjectByGUID(Guid.Parse(info.GetString(variableName + "GUID"))) as T;
            }
            else
            {
                return info.GetValue(variableName, typeof(T)) as T;
            }
        }
    }

    public class SerializationHelper
    {
        public class Variables
        {
            public List<Guid> FullySerialized { private set; get; }
            public bool FullySerializeEverything { set; get; }
            public ILocal Local { private set; get; }

            public Variables(ILocal local, List<Guid> fullySerialized = null, bool fullySerializeEverything = false)
            {
                FullySerialized = fullySerialized;
                Local = local;
                FullySerializeEverything = fullySerializeEverything;
            }
        }

        public static void Serialize(Stream s, object toSerialize, ILocal local, List<Guid> fullySerialized, bool fullySerializeEverything = false)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local, fullySerialized, fullySerializeEverything));
            binaryFormatter.Context = c;

            binaryFormatter.Serialize(s, toSerialize);
        }

        public static object Deserialize(Stream s, ILocal local)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local));
            binaryFormatter.Context = c;
            object result = null;

            result = binaryFormatter.Deserialize(s);

            return result;
        }

        public static T Clone<T>(T toClone, ILocal local) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(ms, toClone, local, null, true);
                ms.Seek(0, SeekOrigin.Begin);
                return Deserialize(ms, local) as T;
            }
        }
    }
}
