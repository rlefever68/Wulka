using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using NLog;

namespace Wulka.Core
{
    public class WulkaContext
    {



        [OnDeserialized]
        private void InitFields(StreamingContext context)
        {
            if (_context == null) _context = new Dictionary<string, string>();
            if (_logger == null) _logger = LogManager.GetCurrentClassLogger();
        }

        private Dictionary<string, string> _context = new Dictionary<string, string>();
        private Logger _logger = LogManager.GetCurrentClassLogger();

   
        public static WulkaContext Current {get;set;}
        
        public void Add(string name, string value)
        {
            lock (_context)
            {
                if (_context.ContainsKey(name))
                    _context.Remove(name);
                _context.Add(name, value);
                //_logger.Debug("\tAdded Context {0}={1}", name, value);
            }
        }

        public void Remove(string name)
        {
            lock (_context)
            {
                var value = _context[name];
                _context.Remove(name);
//                _logger.Debug("\tRemoved Context {0}={1}", name,value);
            }
        }

        public Dictionary<string, string>.KeyCollection Keys
        {
            get
            {
                lock (_context)
                {
                    return _context.Keys;
                }
            }
        }

        public string this[string name]
        {
            get
            {
                lock (_context)
                {
                if (_context.ContainsKey(name))
                    return _context[name];
                }
                if (OperationContext.Current != null)
                    return OperationContext.Current.IncomingMessageProperties[name] as string;

                return string.Empty;
            }
        }

        public void Clear()
        {
            lock (_context)
            {
                _context.Clear();
            }
        }
    }
}

