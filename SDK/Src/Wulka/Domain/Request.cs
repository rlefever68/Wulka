using System.Runtime.Serialization;

namespace Wulka.Domain
{
    [DataContract]
    public class RequestBase
    {
        [DataMember]
        public string StartId { get; set; }
        [DataMember]
        public int Limit { get; set; }
        [DataMember]
        public int Skip { get; set; }
        [DataMember]
        public string EndId { get; set; }
        [DataMember]
        public string StartKey { get; set; }
        [DataMember]
        public string EndKey { get; set; }
        [DataMember]
        public bool Reduce { get; set; }
        [DataMember]
        public bool Stale { get; set; }
        [DataMember]
        public bool IncludeDocs { get; set; }
        [DataMember]
        public bool KeepView { get; set; }
        [DataMember]
        public string ViewName { get; set; }
        [DataMember]
        public string DocName { get; set; }
        [DataMember]
        public string Function 
        {
            get { return GetFunction(); }
            set { _function = value; } 
        }

        protected  virtual string GetFunction()
        {
            return _function;
        }

        public RequestBase SetFunction(string function)
        {
            Function = function;
            return this;
        }

        [DataMember]
        public bool Descending { get; set; }
        private string _keyField = "Id";
        private string _function;

        [DataMember]
        public string KeyField
        {
            get { return _keyField; }
            set { _keyField = value; }
        }
    }
}