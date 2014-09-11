using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Wulka.Utils
{
    public static class InterfaceExtensions
    {
        public static void Raise<T, P>(this PropertyChangedEventHandler value, T source, Expression<Func<T, P>> pe)
        {
            if (value != null)
            {
                value.Invoke(source, new PropertyChangedEventArgs(((MemberExpression)pe.Body).Member.Name));
            }
        }
    }
}
