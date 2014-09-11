using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    [DataContract]
    public abstract class Folder : TaxonomyObject<Folder>,IFolder
    {
        protected Folder()
        {
            Icon = Properties.Resources.Folder32;
            DisplayName = "New Folder";
        }

    }
}
