using System;

namespace Wulka.Core
{
    public class EventArgsT<T> : EventArgs
    {
        private readonly T _eventData;

        public EventArgsT(T data)
        {
            _eventData = data;
        }

        public T Data
        {
            get { return _eventData; }
        }
    }
}
