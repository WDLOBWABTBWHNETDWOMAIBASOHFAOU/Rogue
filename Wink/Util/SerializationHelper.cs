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

    class SerializationHelper
    {
        public class Variables
        {
            public bool DownwardSerialization { set; get; }
            public bool UpwardSerialization { set; get; }
            public ILocal Local { private set; get; }

            public Variables(ILocal local, bool upward = false, bool downward = false)
            {
                DownwardSerialization = downward;
                UpwardSerialization = upward;
                Local = local;
            }
        }

        public static void Serialize(Stream s, object toSerialize, ILocal local, bool upwardSerialization = false, bool downwardSerialization = false)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local, upwardSerialization, downwardSerialization));
            binaryFormatter.Context = c;

            binaryFormatter.Serialize(s, toSerialize);
        }

        public static object Deserialize(Stream s, ILocal local, bool upwardSerialization = false, bool downwardSerialization = false)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            StreamingContext c = new StreamingContext(StreamingContextStates.All, new Variables(local, upwardSerialization, downwardSerialization));
            binaryFormatter.Context = c;

            return binaryFormatter.Deserialize(s);
        }

        public static T Clone<T>(T toClone, ILocal local, bool upwardSerialization = true, bool downwardSerialization = true) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(ms, toClone, local, upwardSerialization, downwardSerialization);
                ms.Seek(0, SeekOrigin.Begin);
                return Deserialize(ms, local, upwardSerialization, downwardSerialization) as T;
            }
        }
    }
}
