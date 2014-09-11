using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace Wulka.Networking
{

    /// <summary>
    /// Network utilities.
    /// </summary>
    public static class NetUtils
    {

        #region Static Methods

        /// <summary>
        /// Return IP Adresses used by a host. 
        /// </summary>
        /// <remarks>    
        /// On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
        /// </remarks>
        /// <param name="hostName">host</param>
        /// <returns>IP addresses</returns>
        public static IPAddress[] GetIP(string hostName)
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addr = ipEntry.AddressList;

            return (addr);
        }

        /// <summary>
        /// Return IP Adresses used by a localhost.
        /// </summary>
        /// <remarks>    
        /// On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
        /// </remarks>
        /// <returns>IP addresses</returns>
        public static IPAddress[] GetIP()
        {
            string hostName = Dns.GetHostName();

            return (GetIP(hostName));
        }

        //protected static void DNSLookup(string hostNameOrAddress)
        //{
        //    Console.WriteLine("Lookup: {0}\n", hostNameOrAddress);

        //    IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
        //    Console.WriteLine("  Host Name: {0}", hostEntry.HostName);

        //    IPAddress[] ips = hostEntry.AddressList;
        //    foreach (IPAddress ip in ips)
        //    {
        //        Console.WriteLine("  Address: {0}", ip);
        //    }

        //    Console.WriteLine();
        //} 

        /// <summary>
        /// Return V4 IP Adresses used by a localhost.
        /// </summary>
        /// <remarks>    
        /// On system which support IPv6, only IPv4 adresses are returned in the same array for all interfaces.
        /// </remarks>
        /// <returns>IP addresses</returns>
        public static IPAddress[] GetIPv4AssociatedWithLocalHost()
        {
            string hostName = Dns.GetHostName();

            const bool IPv4Only = true;
            return (GetIPs(hostName, IPv4Only));
        }


        /// <summary>
        /// Gets the first ip4.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns></returns>
        public static string GetFirstIp4(string hostName)
        {
            const bool IPv4Only = true;
            IPAddress[] ips = GetIPs(hostName, IPv4Only);
            if (ips.Length > 0)
            {
                return ips[0].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Return IP Adresses used by a host.
        /// </summary>
        /// <param name="hostName">host</param>
        /// <param name="IPv4Only">if set to <c>true</c> [I PV4 only].</param>
        /// <returns>IP addresses</returns>
        /// <remarks>
        /// On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
        /// </remarks>
        public static IPAddress[] GetIPs(string hostName, bool IPv4Only)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addressList = hostEntry.AddressList;

            if (IPv4Only)
            {
                List<IPAddress> IPList = new List<IPAddress>();
                foreach (IPAddress iPAddress in addressList)
                {
                    if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPList.Add(iPAddress);
                    }
                }
                return IPList.ToArray();
            }
            else
            {
                return (addressList);
            }
        }

        /// <summary>
        /// Return the hostname (Dns.GetHostName()).
        /// </summary>
        /// <returns>Hostname</returns>
        public static string GetHostName()
        {
            return (Dns.GetHostName());
        }

        ///// <summary>
        ///// Return local IPv4 Addresses declared by all existing network interfaces.
        ///// </summary>
        ///// <returns>Declared unicast IPv4 Addresses</returns>
        //public static IPAddress[] GetIPv4()
        //{
        //    List<IPAddress> addr;          // Hold existing Ip v4 addresses.
        //    NetworkInterface[] interfaces; // Hold network interfaces.

        //    // Get list of all interfaces.
        //    interfaces = NetworkInterface.GetAllNetworkInterfaces();
        //    addr = new List<IPAddress>();

        //    // Loop through interfaces.
        //    foreach (NetworkInterface iface in interfaces)
        //    {
        //        // Create collection to hold IP information for interfaces.
        //        UnicastIPAddressInformationCollection ips;

        //        // Get list of all unicast IPs from current interface.
        //        ips = iface.GetIPProperties().UnicastAddresses;

        //        // Loop through IP address collection.
        //        foreach (UnicastIPAddressInformation ip in ips)
        //        {
        //            // Check IP address for IPv4.
        //            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
        //            {
        //                addr.Add(ip.Address);
        //            }
        //        }
        //    }

        //    return (addr.ToArray());
        //}



        ///// <summary>
        ///// Declare an URL to be used with HttpApi.<br/>
        ///// Windows Xp SP2 and Windows 2003 introduce an http layer named HttpApi which limit the use of Http only for
        ///// Administrators and for registered URL.
        ///// </summary>
        ///// <param name="networkURL">URL to reserve with HttpApi</param>
        ///// <param name="securityDescriptor">DACL string (see Psbe.Common.Shared.Utils.CreateSddl)</param>
        //public static void ReserveURL(string networkURL, string securityDescriptor)
        //{
        //    uint retVal = (uint)HttpApi.ErrorCodes.NO_ERROR; // NO_ERROR = 0
        //    HttpApi.HTTPAPI_VERSION httpApiVersion = new HttpApi.HTTPAPI_VERSION(1, 0);

        //    retVal = HttpApi.HttpInitialize(httpApiVersion, HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

        //    if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
        //    {
        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY keyDesc = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY(networkURL);
        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM paramDesc = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor);

        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET inputConfigInfoSet = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET();
        //        inputConfigInfoSet.KeyDesc = keyDesc;
        //        inputConfigInfoSet.ParamDesc = paramDesc;

        //        IntPtr pInputConfigInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET)));
        //        Marshal.StructureToPtr(inputConfigInfoSet, pInputConfigInfo, false);

        //        retVal = HttpApi.HttpSetServiceConfiguration(IntPtr.Zero,
        //                                                     HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
        //                                                     pInputConfigInfo,
        //                                                     Marshal.SizeOf(inputConfigInfoSet),
        //                                                     IntPtr.Zero);

        //        if ((uint)HttpApi.ErrorCodes.ERROR_ALREADY_EXISTS == retVal)
        //        {  // ERROR_ALREADY_EXISTS = 183
        //            retVal = HttpApi.HttpDeleteServiceConfiguration(IntPtr.Zero,
        //                                                            HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
        //                                                            pInputConfigInfo,
        //                                                            Marshal.SizeOf(inputConfigInfoSet),
        //                                                            IntPtr.Zero);

        //            if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
        //            {
        //                retVal = HttpApi.HttpSetServiceConfiguration(IntPtr.Zero,
        //                                                             HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
        //                                                             pInputConfigInfo,
        //                                                             Marshal.SizeOf(inputConfigInfoSet),
        //                                                             IntPtr.Zero);
        //            }
        //        }

        //        Marshal.FreeCoTaskMem(pInputConfigInfo);
        //        HttpApi.HttpTerminate(HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
        //    }

        //    if ((uint)HttpApi.ErrorCodes.NO_ERROR != retVal)
        //    {
        //        throw new Win32Exception(Convert.ToInt32(retVal));
        //    }
        //}


        ///// <summary>
        ///// Release an URL used with HttpApi.<br />
        ///// Windows Xp SP2 and Windows 2003 introduce an http layer named HttpApi which limit the use of Http only for
        ///// Administrators and for registered URL.
        ///// </summary>
        ///// <param name="networkURL">URL reserved with HttpApi to release</param>
        ///// <param name="securityDescriptor">DACL string (see Psbe.Common.Shared.Utils.CreateSddl)</param>
        //public static void FreeURL(string networkURL, string securityDescriptor)
        //{
        //    uint retVal = (uint)HttpApi.ErrorCodes.NO_ERROR;
        //    HttpApi.HTTPAPI_VERSION httpApiVersion = new HttpApi.HTTPAPI_VERSION(1, 0);

        //    retVal = HttpApi.HttpInitialize(httpApiVersion, HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

        //    if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
        //    {
        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY urlAclKey = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY(networkURL);
        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM urlAclParam = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor);

        //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET urlAclSet = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET();
        //        urlAclSet.KeyDesc = urlAclKey;
        //        urlAclSet.ParamDesc = urlAclParam;

        //        IntPtr configInformation = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET)));
        //        Marshal.StructureToPtr(urlAclSet, configInformation, false);
        //        int configInformationSize = Marshal.SizeOf(urlAclSet);

        //        retVal = HttpApi.HttpDeleteServiceConfiguration(IntPtr.Zero,
        //                                                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
        //                                                        configInformation,
        //                                                        configInformationSize,
        //                                                        IntPtr.Zero);

        //        Marshal.FreeCoTaskMem(configInformation);

        //        HttpApi.HttpTerminate(HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
        //    }

        //    if ((uint)HttpApi.ErrorCodes.NO_ERROR != retVal)
        //    {
        //        throw new Win32Exception(Convert.ToInt32(retVal));
        //    }
        //}



        ///// <summary>
        ///// Get the first unused port above 1024.
        ///// </summary>
        ///// <param name="localAddr">Ip to bind to</param>
        ///// <returns>Port number, 0 in case of error</returns>
        //public static int GetFirstUnusedPort(IPAddress localAddr)
        //{
        //    for (int p = 1024; p <= IPEndPoint.MaxPort; p++)
        //    {
        //        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //        try
        //        {
        //            // If the port is used, an exception 10048 will be raised when we try to bind to...
        //            s.Bind(new IPEndPoint(localAddr, p));
        //            s.Close();
        //            return (p);
        //        }
        //        catch (SocketException ex)
        //        {
        //            // Address in use ?
        //            if (ex.ErrorCode == 10048)
        //            {
        //                continue;

        //            }
        //            else
        //            {
        //                return (0);
        //            }
        //        }
        //    }

        //    return (0);
        //}

        ///// <summary>
        ///// Get the first unused port above startPort.
        ///// </summary>
        ///// <param name="localAddr">Ip to bind to</param>
        ///// <param name="startPort">Starting port</param>
        ///// <returns>Port number, 0 in case of error</returns>
        //public static int GetFirstUnusedPort(IPAddress localAddr, int startPort)
        //{
        //    for (int p = startPort; p <= IPEndPoint.MaxPort; p++)
        //    {
        //        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //        try
        //        {
        //            // If the port is used, an exception 10048 will be raised when we try to bind to...
        //            s.Bind(new IPEndPoint(localAddr, p));
        //            s.Close();
        //            return (p);
        //        }
        //        catch (SocketException ex)
        //        {
        //            // Address in use ?
        //            if (ex.ErrorCode == 10048)
        //            {
        //                continue;

        //            }
        //            else
        //            {
        //                return (0);
        //            }
        //        }
        //    }

        //    return (0);
        //}


        ///// <summary>
        ///// Get the first unused port between startPort and endPort.
        ///// </summary>
        ///// <param name="localAddr">Ip to bind to</param>
        ///// <param name="startPort">Starting port</param>
        ///// <param name="endPort">Ending port</param>
        ///// <returns>Port number, 0 in case of error</returns>
        //public static int GetFirstUnusedPort(IPAddress localAddr, int startPort, int endPort)
        //{
        //    for (int p = startPort; p <= endPort; p++)
        //    {
        //        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //        try
        //        {
        //            // If the port is used, an exception 10048 will be raised when we try to bind to...
        //            s.Bind(new IPEndPoint(localAddr, p));
        //            s.Close();
        //            return (p);
        //        }
        //        catch (SocketException ex)
        //        {
        //            // Address in use ?
        //            if (ex.ErrorCode == 10048)
        //            {
        //                continue;

        //            }
        //            else
        //            {
        //                return (0);
        //            }
        //        }
        //    }

        //    return (0);
        //}

        ///// <summary>
        ///// Verify is a computer + port is connectable on the network.
        ///// </summary>
        ///// <param name="addr">Ip to bind to</param>
        ///// <param name="port">Starting port</param>
        ///// <returns>Port number, 0 in case of error</returns>
        //public static Boolean isConnectable(string addr, Int32 port)
        //{
        //    Socket connector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    //      IPEndPoint host = new IPEndPoint(IPAddress.Parse(addr), (int)port);

        //    try
        //    {
        //        connector.Blocking = false;
        //        connector.Connect(addr, port);

        //        if (connector.Poll(1000, SelectMode.SelectRead))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //            throw new Exception("Timeout detected");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Compresses the given data using GZip algorithm.
        /// </summary>
        /// <param name="data">The data to be compressed.</param>
        /// <returns>The compressed data</returns>
        public static byte[] CompressGZip(byte[] data)
        {
            Stream memoryStream = new MemoryStream();
            GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
            //Stream gZipStream = new GZipOutputStream(memoryStream);  // Does not work, only returns a header of 10 bytes
            gZipStream.Write(data, 0, data.Length);
            gZipStream.Close();

            // Reposition memoryStream to the beginning
            memoryStream.Position = 0;

            byte[] compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, (int)memoryStream.Length);

            memoryStream.Close();

            return compressedData;
        }

        /// <summary>
        /// Decompresses the given data using GZip algorithm.
        /// </summary>
        /// <param name="data">The data to be decompressed.</param>
        /// <returns>The decompressed data</returns>
        public static byte[] DecompressGZip(byte[] data)
        {
            Stream compressedMemoryStream = new MemoryStream(data);
            GZipStream gZipStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress, true);
            Stream decompressedMemoryStream = new MemoryStream(data.Length);
            int byteRead;
            //while ((byteRead = compressedMemoryStream.ReadByte()) != -1)
            while ((byteRead = gZipStream.ReadByte()) != -1)
            {
                decompressedMemoryStream.WriteByte((byte)byteRead);
            }
            gZipStream.Close();
            compressedMemoryStream.Close();

            // Reposition memoryStream to the beginning
            decompressedMemoryStream.Position = 0;

            byte[] decompressedData = new byte[decompressedMemoryStream.Length];
            decompressedMemoryStream.Read(decompressedData, 0, (int)decompressedMemoryStream.Length);
            decompressedMemoryStream.Close();

            return decompressedData;
        }

        /// <summary>
        /// Encodes the binary data to base64 (UTF8).
        /// </summary>
        /// <param name="data">The data to be encoded.</param>
        /// <returns>The data encoded in base64 (UTF8)</returns>
        public static byte[] EncodeBase64(byte[] data)
        {
            // Convert to base64 (string)
            string base64String = Convert.ToBase64String(data);

            // Convert string to byte array (UTF8)
            Stream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(base64String);
            streamWriter.Flush();

            byte[] encodedData = new byte[memoryStream.Length];
            // Reposition memory stream
            memoryStream.Position = 0;
            memoryStream.Read(encodedData, 0, (int)memoryStream.Length);

            // Close streams
            streamWriter.Close();
            memoryStream.Close();

            // Return the byte array
            return encodedData;
        }

        /// <summary>
        /// Decodes the binary data from base64 (UTF8).
        /// </summary>
        /// <param name="data">The data to be decoded and expected to be encoded in base64 (UTF8).</param>
        /// <returns>The decoded binary data</returns>
        public static byte[] DecodeBase64(byte[] data)
        {
            // Reconstruct a string from input data
            Stream memoryStream = new MemoryStream(data);
            StreamReader streamReader = new StreamReader(memoryStream);
            string base64String = streamReader.ReadToEnd();
            // Close streams
            streamReader.Close();
            memoryStream.Close();

            // Convert from base64
            byte[] decodedData = Convert.FromBase64String(base64String);

            return decodedData;
        }

        #endregion

    } // End NetUtils


}
