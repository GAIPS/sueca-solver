using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SuecaInterface
{
    public abstract class JsonSerializable
    {
        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static T DeserializeFromJson<T>(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (T)serializer.Deserialize(textReader, typeof(T));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize from '" + typeof(T) + "': " + e.Message);
            }

            return default(T);
        }
    }
}
