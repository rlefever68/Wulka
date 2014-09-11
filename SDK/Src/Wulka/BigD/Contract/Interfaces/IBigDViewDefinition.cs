using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Linq;
using Wulka.Domain;

namespace Wulka.BigD.Contract.Interfaces
{
    public interface IBigDViewDefinition : IBigDViewDefinitionBase
    {
        string Map { get; set; }
        string Reduce { get; set; }
        IEnumerable<T> All<T>() where T : IBigDbDocument, new();
        bool Equals(IBigDViewDefinition other);
        IEnumerable<T> Key<T>(object key) where T : IBigDbDocument, new();
        IEnumerable<T> FromStartId<T>(string id, int limit) where T : IBigDbDocument, new();
        IEnumerable<T> KeyStartEnd<T>(object[] start, object[] end) where T : IBigDbDocument, new();
        IEnumerable<T> KeyStartEnd<T>(object start, object end) where T : IBigDbDocument, new();
        CouchLinqQuery<T> LinqQuery<T>(RequestBase request);
        BigDQuery Query();
        void ReadJson(JObject obj);
        void Touch();
        void WriteJson(JsonWriter writer);
    }
}