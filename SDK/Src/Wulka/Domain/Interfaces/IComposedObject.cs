using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Wulka.Domain.Interfaces
{

    public interface IComposedObject : IDomainObject
    {

        IDomainObject AddPart(IDomainObject part);
        [DataMember]
        IDomainObject[] Parts { get; set; }
        IDomainObject FindById(string id);
        IParameter AddParameter(IParameter param);
        IEnumerable SelectedItems { get; set; }
        IDomainObject SelectedItem { get; set; }
        IEnumerable<IParameter> Parameters { get; }
        IParameter AddParameter(string id, object value);
        IDomainObject AddBranch();
        IDomainObject AddChild();
        IDomainObject AddFolder();
    }
}