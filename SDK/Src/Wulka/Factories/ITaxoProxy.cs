using System;
using Wulka.Domain.Interfaces;

namespace Wulka.Factories
{
    public interface ITaxoProxy
    {
        IHook GetHook(ITaxonomyObject source);
        void GetHookAsync(ITaxonomyObject taxonomyObject, Action<IHook> action=null);
    }
}