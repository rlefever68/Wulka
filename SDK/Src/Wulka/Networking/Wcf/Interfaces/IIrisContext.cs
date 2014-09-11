namespace Wulka.Networking.Wcf.Interfaces
{
    public interface IWulkaContext
    {
        string DataCulture { get;  }
        string UserCulture { get;  }
        string SessionId { get;  }
        string UserId { get;  }
        string UserTimeZone { get;  }
        string AuthenticationMode { get;  }
        string DataMode { get; }
    }
}
