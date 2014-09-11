using System;
using Wulka.Domain;

namespace Wulka.Interfaces
{
    public interface IDiscoAgent : IDisco
    {
        event Action<SerializableEndpoint[]> GetEndpointsCompleted;
        void GetEndpointsAsync(string contractType);
        
        event Action<SerializableEndpoint[]> GetAllEndpointsCompleted;
        void GetAllEndpointsAsync(Action<SerializableEndpoint[]> getAllEndpointsCompleted=null);


        event Action<DiscoItem[]> GetAllEndpointAddressesCompleted;
        void GetAllEndpointAddressesAsync(Action<DiscoItem[]> getAllEndpointAddressesCompleted=null);

    }
}
