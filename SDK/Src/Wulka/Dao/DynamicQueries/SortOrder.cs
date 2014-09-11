using System.Runtime.Serialization;

namespace Wulka.Dao.DynamicQueries
{
    [DataContract]
    public enum SortOrder
    {
        [EnumMember]
        Ascending,
        [EnumMember]
        Descending
    }
}