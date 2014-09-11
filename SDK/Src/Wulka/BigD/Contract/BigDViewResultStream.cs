using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.Interfaces;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// This is a view result from a CouchQuery that can return CouchDocuments for
    /// resulting documents (include_docs) and/or ICanJson documents for the
    /// result values. A value returned from a CouchDB view does not need to be
    /// a CouchDocument.
    /// </summary>
    public class CouchViewResultStream<T> : BigDViewResult, IEnumerable<BigDRecord<T>> , IDisposable where T: ICanJson, new()
    {
        class RecordEnumerator : IEnumerator<BigDRecord<T>>
        {
            readonly JsonReader reader;
            BigDRecord<T> current;
            bool hasMore = true;
            readonly CouchViewResultStream<T> owner;

            public RecordEnumerator(JsonReader reader, CouchViewResultStream<T> owner)
            {
                this.reader = reader;
                this.owner = owner;
            }

            public BigDRecord<T> Current
            {
                get { return current; }
            }

            public void Dispose() { }

            object IEnumerator.Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (!hasMore)
                    return false;

                var token = JToken.ReadFrom(reader);
                hasMore = reader.Read() && reader.TokenType == JsonToken.StartObject;
                owner.hasMore = hasMore;

                current = new BigDRecord<T>(token as JObject);

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        readonly JsonReader reader;
        bool hasMore = true;

        public CouchViewResultStream(JsonReader reader)
        {
            this.reader = reader;

            var header = new JObject();
            
            // start object
            reader.Read();

            while (reader.Read() && reader.TokenType != JsonToken.StartArray)
            {
                var name = reader.Value.ToString();
                if (name == "rows")
                    continue;

                reader.Read();
                header[name] = new JValue(reader.Value);
            }

            reader.Read();
        }

        public void Dispose()
        {
            reader.Close();
        }

        public IEnumerator<BigDRecord<T>> GetEnumerator()
        {
            if (!hasMore)
                throw new InvalidOperationException("Result stream cannot be re-enumerated");

            return new RecordEnumerator(reader, this);
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!hasMore)
                throw new InvalidOperationException("Result stream cannot be re-enumerated");

            return new RecordEnumerator(reader, this);
        }
    }
}