using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Utils.Network.Json
{
    public static class JsonValidate
    {
        public static bool Validate(string strInput)
        {
            strInput = strInput.Trim();
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) &&
                (!strInput.StartsWith("[") || !strInput.EndsWith("]"))) return false;

            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
