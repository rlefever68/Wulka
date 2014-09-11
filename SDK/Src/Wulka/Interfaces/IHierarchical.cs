using System.Collections.Generic;

namespace Wulka.Interfaces
{
    public interface IHierarchical :IHaveParent
    {
        IEnumerable<IHierarchical> ChildItems { get; set; }
    }
}
