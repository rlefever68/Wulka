// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-19-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-19-2014
// ***********************************************************************
// <copyright file="ImageUtils.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Wulka.Utils
{

    /// <summary>
    /// Provides various image untilities, such as high quality resizing and the ability to save a JPEG.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        private static Dictionary<string, ImageCodecInfo> encoders = null;

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        /// <value>The encoders.</value>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand 
            get
            {
                //if the quick lookup isn't initialised, initialise it 
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                //if there are no codecs, try loading them 
                if (encoders.Count == 0)
                {
                    //get all the codecs 
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        //add each codec to the quick lookup 
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                //return the lookup 
                return encoders;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            //a holder for the result 
            Bitmap result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap 
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality 
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap 
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap 
            return result;
        }


        public static Bitmap ResizeImageWithAspectRatio(Image image, int width, int height)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;
            var newWidth = 0;
            var newHeight = 0;
            newWidth = width >= originalWidth ? originalWidth : width;
            if (height >= originalHeight)
                newHeight = originalHeight;
            else
            {
                if(originalWidth!=0)
                    newHeight = Convert.ToInt32(originalHeight * (newWidth / originalWidth));
            }
            return ResizeImage(image, newWidth, newHeight);
        }

        /// <summary>
        /// Saves an image as a jpeg image, with the given quality
        /// </summary>
        /// <param name="path">Path to which the image would be saved.</param>
        /// <param name="image">The image.</param>
        /// <param name="quality">An integer from 0 to 100, with 100 being the
        /// highest quality</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SaveJpeg(string path, Image image, int quality)
        {
            //ensure the quality is within the correct range 
            if ((quality < 0) || (quality > 100))
            {
                //create the error message 
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception 
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder 
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec 
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters 
            image.Save(path, jpegCodec, encoderParams);
        }

        /// <summary>
        /// Returns the image codec with the given mime type
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>ImageCodecInfo.</returns>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type 
            string lookupKey = mimeType.ToLower();

            //the codec to return, default to null 
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return 
            if (Encoders.ContainsKey(lookupKey))
            {
                //pull the codec from the lookup 
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }


        /// <summary>
        /// Gets the anonoymous avatar.
        /// </summary>
        /// <value>The anonoymous avatar.</value>
        public static Image AnonymousAvatar
        {
            get { return Properties.Resources.anonymousAvatar; }
        }

        /// <summary>
        /// Gets the anonymous avatar bytes.
        /// </summary>
        /// <value>The anonymous avatar bytes.</value>
        public static byte[] AnonymousAvatarBytes 
        {
            get 
            {
                return ImageToByteArray(Properties.Resources.anonymousAvatar);
            }
            
        }

        /// <summary>
        /// Images to byte array.
        /// </summary>
        /// <param name="imageIn">The image in.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }

        /// <summary>
        /// Bytes the array to image.
        /// </summary>
        /// <param name="byteArrayIn">The byte array in.</param>
        /// <returns>Image.</returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }


        /// <summary>
        /// Gets the image from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Image.</returns>
        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        /// <summary>
        /// Gets the image bytes from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] GetImageBytesFromUrl(string url)
        {
            var res = GetImageFromUrl(url);
            return ImageToByteArray(res);
        }


        /// <summary>
        /// Gets the avatar bytes.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="maintainAspectRatio"></param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] GetAvatarBytes(string imageUrl, int width, int height, bool maintainAspectRatio = false)
        {
            var img = AnonymousAvatar;
            try
            {
                img = GetImageFromUrl(imageUrl);
            }
            catch (Exception)
            {
                img = AnonymousAvatar;
            }
            Bitmap bmp;
            bmp = !maintainAspectRatio ? ResizeImage(img, width, height) : ResizeImageWithAspectRatio(img, width, height);
            return ImageToByteArray(bmp);
        }


        /// <summary>
        /// Gets the avatar base64.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="maintainAspectRatio"></param>
        /// <returns>System.String.</returns>
        public static string GetAvatarBase64(string imageUrl, int width, int height, bool maintainAspectRatio = false)
        {
            return Convert.ToBase64String(GetAvatarBytes(imageUrl, width, height, maintainAspectRatio));
        }


        /// <summary>
        /// Gets the avatar.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>System.Byte[][].</returns>
        public static Image GetAvatar(string imageUrl, int width, int height)
        {
            if (String.IsNullOrEmpty(imageUrl)) return AnonymousAvatar;
            try
            {
                var img = GetImageFromUrl(imageUrl);
                return ResizeImage(img, width, height);
            }
            catch (Exception)
            {
                return AnonymousAvatar;
            }
        }



    } 
}
