﻿using System.IO;
using System.IO.Compression;
using Wulka.Core;

namespace Wulka.Utils
{
    public class ZipHelper
    {

        /// <summary>
        /// Compresses file
        /// </summary>
        /// <param name="fi">uncompressed file</param>
        /// <returns>compressed file, same location as original compressed file, but .gz added to extension</returns>
        public static FileInfo Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (var inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and already compressed files.
                if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    var compressedFilePath = string.Format("{0}.gz", fi.FullName);

                    // Create the compressed file.
                    using (var outFile = File.Create(compressedFilePath))
                    {
                        using (var compress = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            // Copy the source file into the compression stream.
                            inFile.CopyTo(compress);
                        }
                    }

                    return new FileInfo(compressedFilePath);
                }
            }

            return null;
        }

        /// <summary>
        /// Uncompresses file
        /// </summary>
        /// <param name="fi">compressed file</param>
        /// <returns>uncompressed file, same location as original compressed file, but minus the extension</returns>
        public static FileInfo Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (var inFile = fi.OpenRead())
            {
                // Get original file name
                var compressedFilePath = fi.FullName;
                var decompressedFilePath = compressedFilePath.Remove(compressedFilePath.Length - fi.Extension.Length);

                // Create the decompressed file.
                using (var outFile = File.Create(decompressedFilePath))
                {
                    using (var decompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        // Copy the decompression stream into the output file.
                        decompress.CopyTo(outFile);
                    }
                }

                return new FileInfo(decompressedFilePath);
            }
        }

        /// <summary>
        /// Uncompresses file
        /// </summary>
        /// <param name="ms">compressed file</param>
        /// <param name="fileName">Name of the compressed file.</param>
        /// <returns>
        /// uncompressed file, stored in the TransactionRouter temporary path with same name, but minus the extension (.gz)
        /// </returns>
        public static FileInfo Decompress(MemoryStream ms, string fileName)
        {
            // Get original file name
            var decompressedFilePath = Path.Combine(PathUtils.InternetCacheDir(), fileName.Remove(fileName.Length - 3));

            // Create the decompressed file.
            using (var outFile = File.Create(decompressedFilePath))
            {
                using (var decompress = new GZipStream(ms, CompressionMode.Decompress))
                {
                    // Copy the decompression stream into the output file.
                    decompress.CopyTo(outFile);
                }
            }
            return new FileInfo(decompressedFilePath);
        }

        /// <summary>
        /// Uncompresses file
        /// </summary>
        /// <param name="ms">compressed file</param>
        /// <param name="fileName">Name of the compressed file.</param>
        /// <param name="temporaryPath">The temporary path.</param>
        /// <returns>
        /// uncompressed file, stored in the TransactionRouter temporary path with same name, but minus the extension (.gz)
        /// </returns>
        public static FileInfo Decompress(MemoryStream ms, string fileName, string temporaryPath)
        {
            // Get original file name
            var decompressedFilePath = Path.Combine(temporaryPath, fileName.Remove(fileName.Length - 3));

            // Create the decompressed file.
            using (var outFile = File.Create(decompressedFilePath))
            {
                using (var decompress = new GZipStream(ms, CompressionMode.Decompress))
                {
                    // Copy the decompression stream into the output file.
                    decompress.CopyTo(outFile);
                }
            }
            return new FileInfo(decompressedFilePath);
        }
    }
}
