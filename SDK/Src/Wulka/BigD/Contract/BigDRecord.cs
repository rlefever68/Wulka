﻿using Newtonsoft.Json.Linq;
using Wulka.Interfaces;
using Wulka.Utils.Json;

namespace Wulka.BigD.Contract
{
    public class BigDRecord<T> where T: ICanJson, new()
    {
        private readonly JObject record;

        public BigDRecord(JObject source)
        {
            record = source;

            Id = record.Value<string>("id");
            Key = record["key"];
            Value = record["value"];
        }

        public string Id { get; private set; }
        public JToken Key { get; private set; }
        public JToken Value { get; private set; }

        public T Document
        {
            get
            {
                JToken val;
                if (!record.TryGetValue("doc", out val))
                {
                    return default(T);
                }

                var doc = val as JObject;
                if (doc == null)
                {
                    return default(T);
                }

                var ret = new T();
                ret = JSonHelper<T>.ReadJSon(doc);
                ret.ReadJson(doc);
                return ret;
            }
        }
    }
}
