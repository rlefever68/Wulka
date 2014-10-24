﻿using System.ServiceModel.Description;

namespace Wulka.Networking.Wcf.Utils
{
    static class Extensions
    {
        public static string GetHeaderType(this MessageHeaderDescription header)
        {
            return (string)ReflectionUtils.GetValue(header, "BaseType");
        }
    }
}
