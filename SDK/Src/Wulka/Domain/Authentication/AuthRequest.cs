using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain.Authentication
{
    [DataContract]
    public class AuthRequest : DomainObject<AuthRequest>
    {
        private string _userName;
        [DataMember]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }
        private string _password;
        [DataMember]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }
     
        [DataMember]
        public string FunctionId { get; set; }
        [DataMember]
        public string AuthMode { get; set; }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<AuthRequest>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Generic.ICollection<string> Validate()
        {
            return DataErrorInfoValidator<AuthRequest>.Validate(this);
        }
    }

}
