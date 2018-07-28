using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utils.Network.Json
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SpawnNPCPayload : NetActionPayload
    {
        public int SeatNumber { get; set; }
        public string Id { get; set; }
    }
}