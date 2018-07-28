using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Utils.Network.Json
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GameObjectStatusPayload : NetActionPayload
    {
        public string Id { get; set; }

        [JsonConverter(typeof(JSONVector3Converter))]
        public Vector3 Position { get; set; }

        public float StoppingDistance { get; set; }
        public float RemainingDistance { get; set; }

        [JsonConverter(typeof(JSONVector3Converter))]
        public Vector3 TargetPosition { get; set; }
    }
}