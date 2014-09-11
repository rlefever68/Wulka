using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// A CouchDocument that holds its contents as a parsed JObject DOM which can be used
    /// as a "light weight" base document instead of CouchDocument.
    /// The _id and _rev are held inside the JObject.
    /// </summary>
    public class BigDJsonDocument : IBigDbDocument
    {
        public BigDJsonDocument(string json, string id, string rev)
        {
            Obj = JObject.Parse(json);
            Id = id;
            Rev = rev;
        }

        public BigDJsonDocument(string json, string id)
        {
            Obj = JObject.Parse(json);
            Id = id;
        }

        public BigDJsonDocument(string json)
        {
            Obj = JObject.Parse(json);
        }

        public BigDJsonDocument(JObject doc)
        {
            Obj = doc;
        }

        public BigDJsonDocument()
        {
            Obj = new JObject();
        }

        public override string ToString()
        {
            return Obj.ToString();
        }

        public JObject Obj { get; set; }

        #region ICouchDocument Members

        public virtual void WriteJson(JsonWriter writer)
        {
            foreach (JToken token in Obj.Children())
            {
                token.WriteTo(writer);
            }
        }

        // Presume that Obj has _id and _rev
        public void ReadJson(JObject obj)
        {
            Obj = obj;
        }

        public string Rev
        {
            get
            {
                if (Obj["_rev"] == null)
                {
                    return null;
                }
                return Obj["_rev"].Value<string>();
            }
            set { Obj["_rev"] = JToken.FromObject(value); }
        }
        public string Id
        {
            get
            {
                if (Obj["_id"] == null)
                {
                    return null;
                }
                return Obj["_id"].Value<string>();
            }
            set { Obj["_id"] = JToken.FromObject(value); }
        }

        #endregion
    }
}