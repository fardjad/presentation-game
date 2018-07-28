using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utils.Network.Json
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class NetAction
    {
        public string Type { get; set; }
        public NetActionPayload Payload { get; set; }
    }
}