using System.Runtime.Serialization;

namespace Wulka.Domain.Base
{
    [DataContract]
    public class BooleanResult : Result
    {

        public BooleanResult(bool value = false)
        {
            Value = value;
        }

        [DataMember]
        public bool Value { get; set; }
    }
}
