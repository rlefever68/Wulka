using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Wulka.Resources
{
    public abstract class ResourceBase
    {
        /// <summary>
        /// Gets the embedded file.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Byte[] GetEmbeddedFile(string assemblyName, string fileName)
        {
            var a = Assembly.GetAssembly(GetType());
            var str = a.GetManifestResourceNames();
            var imgStream = (from s in str 
                             where s.Contains(fileName) 
                             select a.GetManifestResourceStream(s)).FirstOrDefault();
            var buffer = BufferFromImage(imgStream);
            return buffer;
        }


        /// Converts the stream image file to bytes array
        /// </summary>
        /// <param name="stream">stream of the image file</param>
        /// <returns>byte[]</returns>
        public byte[] BufferFromImage(Stream stream)
        {

            byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (var br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Gets the embedded XML file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns></returns>
        public Stream GetEmbeddedXmlFile(string fileName)
        {
            Stream s = null;
            var assembly = Assembly.GetAssembly(GetType());
            if(assembly != null) s = assembly.GetManifestResourceStream(fileName);
            return s;
        }
    }
}
