using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Wulka.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public static class ResourceUtil
    {
        /// <summary>
        /// Gets the embedded file.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Stream GetEmbeddedFile(string assemblyName, string fileName)
        {
            try
            {
                var a = Assembly.Load(assemblyName);
                var str = a.GetManifestResourceStream(assemblyName + "." + fileName);

                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        /// <summary>
        /// Gets the embedded file.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Stream GetEmbeddedFile(Assembly assembly, string fileName)
        {
            var assemblyName = assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, fileName);
        }

        /// <summary>
        /// Gets the embedded file.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Stream GetEmbeddedFile(Type type, string fileName)
        {
            var assemblyName = type.Assembly.GetName().Name;
            return GetEmbeddedFile(assemblyName, fileName);
        }


       

        /// <summary>
        /// Gets the embedded XML.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static XmlDocument GetEmbeddedXml(Type type, string fileName)
        {
            var str = GetEmbeddedFile(type, fileName);
            var tr = new XmlTextReader(str);
            var xml = new XmlDocument();
            xml.Load(tr);
            return xml;
        }



        /// <summary>
        /// Converts the byte array to stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Stream ConvertByteArrayToStream(byte[] input)
        {
            var sOut = new MemoryStream();
            sOut.Write(input, 0, input.Length);
            return sOut;
        }





        








        /// <summary>
        /// Toes the byte array.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] ToByteArray(this Stream  sIn)
        {
           

            MemoryStream mem = new MemoryStream();
            CopyStream(sIn, mem);

            // getting the internal buffer (no additional copying)
            var buffer = mem.GetBuffer();
            var length = mem.Length; // the actual length of the data 
            // (the array may be longer)

            // if you need the array to be exactly as long as the data
            return mem.ToArray(); // makes another copy

        }

        /// <summary>
        /// Copies the stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <remarks></remarks>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] b = new byte[32768];
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);
        }
    }
}


