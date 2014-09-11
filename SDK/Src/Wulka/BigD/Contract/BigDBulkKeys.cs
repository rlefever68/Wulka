using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.Interfaces;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// Only used as psuedo doc when doing bulk reads.
    /// </summary>
    public class BigDBulkKeys : ICanJson
    {
        public BigDBulkKeys(IEnumerable<object> keys)
        {
            Keys = keys.ToArray();
        }

        public BigDBulkKeys()
        {
        }

        public BigDBulkKeys(object[] keys)
        {
            Keys = keys;
        }

        public object[] Keys { get; set; }

        #region ICouchBulk Members

        public virtual void WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("keys");
            writer.WriteStartArray();
            foreach (var id in Keys)
            {
                writer.WriteValue(id);
            }
            writer.WriteEndArray();
        }

        public virtual void ReadJson(JObject obj)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return Keys.Count();
        }

        #endregion
    }
}