using System.Runtime.Serialization;

namespace Wulka.Dao.DynamicQueries
{
    [DataContract]
    public class QueryFilter
    {
        /// <summary>
        /// Field's column name
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }

        /// <summary>
        /// Possible filter value, null means: don't apply filter
        /// </summary>
        [DataMember]
        public string FilterValue { get; set; }

        public QueryFilter(string columnName, string filterValue)
        {
            ColumnName = columnName;
            FilterValue = filterValue;
        }
    }
}