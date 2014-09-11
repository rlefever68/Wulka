using System;
using System.Windows.Media;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class DisplayInfo.
    /// </summary>
    public class DisplayInfo : IDisplayInfo
    {
       
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        public DateTime TouchedAt { get; set; }
        public string AdditionalInfoUri { get; set; }
        public ImageSource Icon { get; set; }
    }
}