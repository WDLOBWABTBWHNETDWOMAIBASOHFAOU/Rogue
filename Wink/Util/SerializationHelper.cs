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
    }

    public class SerializationHelper
    {
        public class Variables
        {
            public bool GUIDSerialization { set; get; }
            public ILocal Local { private set; get; }

            public Variables(ILocal local, bool guidSerialization = false)
            {
                GUIDSerialization = guidSerialization;
                Local = local;
            }
        }

        public static void Serialize(Stream s, object toSerialize, ILocal local, bool guidSerialization)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local, guidSerialization));
            binaryFormatter.Context = c;

            binaryFormatter.Serialize(s, toSerialize);
        }

        public static object Deserialize(Stream s, ILocal local, bool guidSerialization)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local, guidSerialization));
            binaryFormatter.Context = c;
            object result = null;
            try
            {
                result = binaryFormatter.Deserialize(s);
            }
            catch (SerializationException se)
            {
                c.GetVars().GUIDSerialization = !c.GetVars().GUIDSerialization;
                s.Seek(0, SeekOrigin.Begin);
                result = binaryFormatter.Deserialize(s);
            }

            return result;
        }

        public static T Clone<T>(T toClone, ILocal local, bool guidSerialization) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(ms, toClone, local, guidSerialization);
                ms.Seek(0, SeekOrigin.Begin);
                return Deserialize(ms, local, guidSerialization) as T;
            }
        }
    }
}
