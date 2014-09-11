using System;
using System.Globalization;
using System.Linq;

namespace Wulka.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public class ByteSizeInfoFormat : IFormatProvider, ICustomFormatter
    {
        private enum Multiplier
        {
            Byte = 0,
            KiloByte = 1,
            MegaByte = 2,
            GigaByte = 3,
            TeraByte = 4,
        }

        private long _numberOfBytes;
        private const int ByteConversion = 1024;

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType"/>, if the <see cref="T:System.IFormatProvider"/> implementation can supply that type of object; otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter)
                ? this
                : null;
        }

        /// <summary>
        /// Handles the other formats.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The arg.</param>
        /// <returns></returns>
        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            
            if (arg != null)
                return arg.ToString();

            return String.Empty;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Provide default formatting if arg is not an Int64.
            if (arg.GetType() != typeof(Int64))
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException)
                {
                    throw new FormatException(String.Format("The type of '{0}' is invalid for formatting with {1}.", arg.GetType().Name, GetType().Name));
                }
            }


            _numberOfBytes = (long)arg;

            return ToFileSize(2);
        }

        
        /// <summary>
        /// Gets the multiplier string.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns></returns>
        private static string GetMultiplierString(Multiplier multiplier)
        {
            switch (multiplier)
            {
                case Multiplier.Byte:
                    return "Bytes";
                case Multiplier.KiloByte:
                    return "KB";
                case Multiplier.MegaByte:
                    return "MB";
                case Multiplier.GigaByte:
                    return "GB";
                case Multiplier.TeraByte:
                    return "TB";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Toes the size of the file.
        /// </summary>
        /// <param name="rounding">The rounding.</param>
        /// <returns></returns>
        public string ToFileSize(int rounding)
        {
            double bytes = Convert.ToDouble(_numberOfBytes);

            var multipliers = (Multiplier[])Enum.GetValues(typeof(Multiplier));
            foreach (Multiplier multiplier in multipliers.OrderByDescending(m => (int)m))
            {
                double divider = Math.Pow(ByteConversion, (int)multiplier);
                if (bytes >= divider)
                {
                    return string.Concat(Math.Round(_numberOfBytes / divider), GetMultiplierString(multiplier));
                }
            }
            throw new FormatException("Count not convert ByteSizeInfo.");
        }

    }
}
