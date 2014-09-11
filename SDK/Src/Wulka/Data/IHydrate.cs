using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;

namespace Wulka.Data
{
    /// <summary>
    /// Interface IHydrate
    /// </summary>
    public interface IHydrate
    {
        /// <summary>
        /// Hydrates the specified domain object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainObject">The domain object.</param>
        void Hydrate<T>(DomainObject<T> domainObject) 
            where T : IDomainObject;
    }
}