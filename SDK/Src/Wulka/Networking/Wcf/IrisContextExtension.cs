using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;

namespace Wulka.Networking.Wcf
{
    internal class WulkaContextExtension : Dictionary<XName, MessageHeader>, IExtension<IContextChannel>
    {
        #region IExtension<IContextChannel> Members

        public void Attach(IContextChannel owner)
        {
        }

        public void Detach(IContextChannel owner)
        {
        }

        #endregion
    }
}