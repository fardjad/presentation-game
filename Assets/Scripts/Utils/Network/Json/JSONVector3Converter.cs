using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils.Network.Json
{
    public class JSONVector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            var dic = new Dictionary<string, float>
            {
                {"x", value.x},
                {"y", value.y},
                {"z", value.z}
            };
            serializer.Serialize(writer, dic);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, float>>(reader);

            float x, y, z;
            dict.TryGetValue("x", out x);
            dict.TryGetValue("y", out y);
            dict.TryGetValue("z", out z);

            return new Vector3(x, y, z);
        }
    }
}
