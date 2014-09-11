using System.Collections.Generic;

namespace Wulka.Domain.Interfaces
{
    public interface IHook 
    {
        string DataCulture { get; set; }
        IEnumerable<IDescription> Descriptions { get;  }
    }
}