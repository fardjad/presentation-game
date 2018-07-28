using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Utils.Network.Json
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class NetActionPayload
    {
        // extra fields
        [JsonExtensionData] public IDictionary<string, JToken> ExtraStuff;
    }
}