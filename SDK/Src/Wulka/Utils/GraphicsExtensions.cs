using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Wulka.Utils
{
    public static class GraphicsExtensions
    {
        /// <summary>
        ///     Toes the byte array.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Bitmap img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Froms the byte array.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static Bitmap FromByteArray(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var img = (Bitmap) Image.FromStream(ms);
                return img;
            }
        }
    }
}