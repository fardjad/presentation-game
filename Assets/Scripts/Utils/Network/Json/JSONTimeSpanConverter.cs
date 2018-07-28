using System;
using Newtonsoft.Json;

namespace Utils.Network.Json
{
    public class JSONTimeSpanConverter : JsonConverter<TimeSpan>
    {
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1);

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var ts = new TimeSpan(value.Ticks - EPOCH.Ticks);
            writer.WriteValue((long) Math.Floor(ts.TotalMilliseconds));
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer) return TimeSpan.Zero;

            var milliseconds = (long) reader.Value;
            var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            return new TimeSpan(timeSpan.Ticks + EPOCH.Ticks);
        }
    }
}