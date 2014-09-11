using System.ComponentModel;
using Wulka.Domain.Base;
using Wulka.ErrorHandling;

namespace Wulka.Interfaces
{
    public interface IResult :  INotifyPropertyChanged
    {
        Result AddError(string message);
        Result AddInfo(string message);
        void Add(ErrorInfo errorInfo);
    }
}
