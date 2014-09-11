using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Interfaces;

namespace Wulka.BigD.Contract.Lucene
{
    /// <summary>
    /// A Lucene query hit containing document id and score.
    /// Optionally it also contains field (optional), sort_order (optional) and the actual document embedded
    /// if you used IncludeDocuments() when querying.
    /// </summary>
    public class BigDLuceneHit: ICanJson
    {
        public JObject Obj;

        public BigDLuceneHit(JObject row)
        {
            Obj = row;
        }

        public string Id()
        {
            return Obj["id"].Value<string>();
        }

        public float Score()
        {
            return Obj["score"].Value<float>();
        }

        public Array SortOrder()
        {
            return Obj["sort_order"].Value<Array>();
        }


        public void WriteJson(JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extract any embedded document.
        /// </summary>
        public T Document<T>() where T: IBigDbDocument, new()
        {
            if (!HasDocument())
            {
                throw new BigDException("No embedded document in this Lucene hit. Did you forget IncludeDocuments()?");
            }
            var doc = new T();
            doc.ReadJson(Obj["doc"].Value<JObject>());
            return doc;
        }

        public void ReadJson(JObject obj)
        {
            Obj = obj;
        }

        public bool HasDocument()
        {
            return (Obj["doc"] != null);
        }
    }
}