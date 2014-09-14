using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Wulka.Domain.Interfaces
{

    public interface IComposedObject : IEcoObject
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
        string AddBranchCommandDisplayName { get; }
        string AddFolderCommandDisplayName { get; }
        string AddChildCommandDisplayName { get; }
        ImageSource AddBranchCommandIcon { get; }
        ImageSource AddFolderCommandIcon { get; }
        ImageSource AddChildCommandIcon { get; }
        IDomainObject AddBranch();
        IDomainObject AddChild();
        IDomainObject AddFolder();
        bool CanAddBranch();
        bool CanAddChild();
        bool CanAddFolder();
        bool AddBranchVisible { get; }
        bool AddChildVisible { get; }
        bool AddFolderVisible { get;  }
    }
}