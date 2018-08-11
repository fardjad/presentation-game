﻿using System.Linq;
using Newtonsoft.Json.Linq;

namespace Utils.StateMachine.Parser
{
    public static class JsonHelper
    {
        public static object Deserialize(string json)
        {
            return ToObject(JToken.Parse(json));
        }

        private static object ToObject(JToken token)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                        .ToDictionary(prop => prop.Name,
                            prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue) token).Value;
            }
        }
    }
}