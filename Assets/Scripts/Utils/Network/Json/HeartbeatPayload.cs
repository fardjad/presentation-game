using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utils.Network.Json
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HeartbeatPayload : NetActionPayload
    {
        [JsonConverter(typeof(JSONTimeSpanConverter))]
        public TimeSpan Ts { get; set; }
    }
}