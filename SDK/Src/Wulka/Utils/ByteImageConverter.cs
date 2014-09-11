// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-19-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-05-2014
// ***********************************************************************
// <copyright file="ByteImageConverter.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NLog;
using Wulka.Exceptions;

namespace Wulka.Utils
{
    /// <summary>
    /// Class ByteImageConverter.
    /// </summary>
    public static class ByteImageConverter
    {
        /// <summary>
        /// To the image source.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <returns>ImageSource.</returns>
        public static ImageSource ToImageSource(this byte[] imageData)
        {
            var biImg = new BitmapImage();
            try
            {
                var ms = new MemoryStream(imageData);
                biImg.BeginInit();
                biImg.StreamSource = ms;
                biImg.EndInit();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
            }
            return biImg;
        }


        private static Logger _logger = LogManager.GetCurrentClassLogger();

    }
}