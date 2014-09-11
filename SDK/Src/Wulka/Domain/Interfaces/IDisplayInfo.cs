using System;
using System.Windows.Media;

namespace Wulka.Domain.Interfaces
{
    public interface IDisplayInfo
    {
        string DisplayName { get; set; }
        DateTime TouchedAt { get; set; }
        string AdditionalInfoUri { get; set; }
        ImageSource Icon { get; set; }
    }
}