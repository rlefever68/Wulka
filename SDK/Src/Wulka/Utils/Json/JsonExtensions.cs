using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wulka.Utils.Json
{
    public static class JsonExtensions
    {
        public static T DeserializeJson<T>(this string jsonString, IEnumerable<Type> knownTypes = null)
        {
            if (string.IsNullOrEmpty(jsonString))
                return default(T);
            return JsonConvert.DeserializeObject<T>(jsonString,
                                                    new JsonSerializerSettings
                                                    {
                                                        TypeNameHandling = TypeNameHandling.Auto,
                                                        Converters = new List<JsonConverter>(
                                                            new JsonConverter[]
                                                        {
                                                           new JsonKnownTypeConverter(knownTypes)
                                                        })

                                                    });
        }

        public static string SerializeJson(this object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented,
                                               new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

    }
}
