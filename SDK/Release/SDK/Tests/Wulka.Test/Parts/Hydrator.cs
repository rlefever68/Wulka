using System;
using System.ComponentModel.Composition;
using Wulka.Data;
using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;

namespace Wulka.Test.Parts
{
    [Export(typeof(IHydrate))]
    internal class Hydrator : IHydrate
    {
        public void Hydrate<T>(DomainObject<T> domainObject) 
            where T : IDomainObject
        {
            Console.WriteLine("OnHydrate");
        }
    }
}
