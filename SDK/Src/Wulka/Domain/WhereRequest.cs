using System;
using System.Runtime.Serialization;

namespace Wulka.Domain
{
    [DataContract]
    public class WhereRequest : RequestBase
    {

        [DataMember]
        public string Field {get;set; }
        [DataMember]
        public string Value {get;set;}
        protected override string GetFunction()
        {
            return String.Format("if(doc.{0} && doc.{0}=='{1}') emit(doc.{2},doc);", Field, Value, KeyField);
        }
    }
}
