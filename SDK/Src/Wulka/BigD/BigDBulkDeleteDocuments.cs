using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract;
using Wulka.BigD.Contract.Interfaces;

namespace Wulka.BigD
{
    /// <summary>
    /// Only used as pseudo doc when doing bulk updates/inserts.
    /// </summary>
    public class BigDBulkDeleteDocuments : BigDBulkDocuments
    {
        public BigDBulkDeleteDocuments(IEnumerable<IBigDbDocument> docs) : base(docs)
        {
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("docs");
            writer.WriteStartArray();
            foreach (IBigDbDocument doc in Docs)
            {
                writer.WriteStartObject();
                BigDDocument.WriteIdAndRev(doc, writer);
                writer.WritePropertyName("_deleted");
                writer.WriteValue(true);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override void ReadJson(JObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
