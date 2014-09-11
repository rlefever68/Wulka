using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using Wulka.Domain;

namespace Wulka.Networking.Wcf
{
    public static class WulkaContextHeaderExtension
    {
        private const string Ns = ServiceConst.ContextNamespace;

        /// <summary>
        /// Adds to context.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void AddToContext(this IContextChannel proxy, string name, string value)
        {
            var extension = proxy.Extensions.Find<WulkaContextExtension>();
            if (extension == null)
            {
                extension = new WulkaContextExtension();
                proxy.Extensions.Add(extension);
            }

            extension[XName.Get(name, Ns)] = MessageHeader.CreateHeader(name, Ns, value);
        }


        /// <summary>
        /// Gets the Wulka context headers.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>IEnumerable&lt;MessageHeader&gt;.</returns>
        public static IEnumerable<MessageHeader> GetWulkaContextHeaders(this IContextChannel proxy)
        {
            var extension = proxy.Extensions.Find<WulkaContextExtension>();
            if (extension == null)
            {
                return Enumerable.Empty<MessageHeader>();
            }
            return extension.Values;
        }

        /// <summary>
        /// Removes from context.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="name">The name.</param>
        public static void RemoveFromContext(this IContextChannel proxy, string name)
        {
            var extension = proxy.Extensions.Find<WulkaContextExtension>();
            if (extension != null)
            {
                extension.Remove(XName.Get(name, Ns));
            }
        }
    }
}