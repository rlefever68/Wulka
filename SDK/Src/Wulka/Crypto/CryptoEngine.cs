﻿#region Using directives

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Wulka.Crypto
{
	public partial class CryptoEngine
	{


        /// <summary>
        /// Encrypts the specified ablock.
        /// </summary>
        /// <param name="ablock">The ablock.</param>
        /// <returns></returns>
		public static string Encrypt(string ablock)
		{
			try
			{
                if (string.IsNullOrEmpty(ablock))
                    return ablock;

                    var des = new DESCryptoServiceProvider();
                    var ms = new MemoryStream();
                    var bin = Encoding.UTF8.GetBytes(ablock);

                    var cstream = new CryptoStream(ms, des.CreateEncryptor(_key, _IV), CryptoStreamMode.Write);
                    cstream.Write(bin, 0, bin.Length);
                    cstream.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());

			}
			catch (Exception ex)
			{
				throw new Exception("An exception occurred in Wulka.CryptoEngine.Encrypt(): ", ex);
			}
		}

        /// <summary>
        /// Decrypts the specified ablock.
        /// </summary>
        /// <param name="ablock">The ablock.</param>
        /// <returns></returns>
		public static string Decrypt(string ablock)
		{
			try
			{
                if (string.IsNullOrEmpty(ablock))
                    return ablock;
				var des = new DESCryptoServiceProvider();
				var ms = new MemoryStream();
				byte[] bin = Convert.FromBase64String(ablock);

				var cstream = new CryptoStream(ms, des.CreateDecryptor(_key, _IV), CryptoStreamMode.Write);
				cstream.Write(bin, 0, bin.Length);
				cstream.FlushFinalBlock();

				return Encoding.UTF8.GetString(ms.ToArray());
			}
			catch (Exception ex)
			{
				throw new Exception("An exception occurred in Wulka.CryptoEngine.Decrypt(): ", ex);
			}
		}
	}
}
