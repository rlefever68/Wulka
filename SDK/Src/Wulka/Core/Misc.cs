using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Wulka.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Misc
    {
        /// <summary>
        /// Gets a value indicating whether [design mode].
        /// </summary>
        /// <value><c>true</c> if [design mode]; otherwise, <c>false</c>.</value>
        public static bool DesignMode
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName == "devenv";
            }
        }

        ///// <summary>
        ///// Gets the device.
        ///// </summary>
        ///// <value>The device.</value>
        //public static string Device
        //{
        //    get
        //    {
        //        return SystemInformation.ComputerName;
        //    }
        //}

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public static string UserName
        {
            get
            {
                return WindowsIdentity.GetCurrent().Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public const string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Parses the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static DateTime? ParseDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Runs the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public static void RunFile(string fileName ,string arguments)
        {
            var startInfo = new ProcessStartInfo {FileName = fileName, Arguments = arguments} ;
//            startInfo.Arguments = f;
            Process.Start(startInfo);
        }


        /// <summary>
        /// Calculates the MD5 hash.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>MD5 hash over the input</returns>
        public static string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


    }
}
