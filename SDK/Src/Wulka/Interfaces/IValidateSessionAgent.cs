using System;

namespace Wulka.Interfaces
{
    public interface IValidateSessionAgent : IValidateSession
    {
        void ValidateAsync(string userName, string sessionId);
        event Action<bool> ValidateCompleted;
    }
}
