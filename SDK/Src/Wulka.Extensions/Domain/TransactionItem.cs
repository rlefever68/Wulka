using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [DataContract]
    public class TransactionItem  : DomainObject<TransactionItem>
    {


        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks></remarks>
        [DataMember]
        public string Message { get; set; }




        /// <summary>
        /// Gets or sets the type id.
        /// </summary>
        /// <value>The type id.</value>
        /// <remarks></remarks>
        [DataMember]
        public string TypeId { get; set; }


        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        /// <value>The raw data.</value>
        /// <remarks></remarks>
        [DataMember]
        public string RawData { get; set; }


        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        /// <remarks></remarks>
        [DataMember]
        public byte[] Body { get; set; }



        /// <summary>
        /// Gets or sets the XML.
        /// </summary>
        /// <value>The XML.</value>
        /// <remarks></remarks>
        [DataMember]
        public XElement Xml { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<TransactionItem>.Validate(this);
        }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<TransactionItem>.Validate(this, columnName);
        }
    }
}
