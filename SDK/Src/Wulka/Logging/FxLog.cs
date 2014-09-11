// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 05-26-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 05-26-2014
// ***********************************************************************
// <copyright file="FxLog.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using NLog;
using Wulka.Exceptions;

namespace Wulka.Logging
{
    /// <summary>
    /// Class FxLog.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FxLog<T>  
        where T:class
    {

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger
        {
            get 
            {
                try
                {
                    return LogManager.GetLogger(typeof(T).Name);
                }
                catch (Exception)
                {
                    return LogManager.GetCurrentClassLogger();
                }
                
            }
        }

        /// <summary>
        /// Errors the format.
        /// </summary>
        /// <param name="empty">The empty.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void ErrorFormat(string empty)
        {
            Logger.Error(empty);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.</value>
        public static bool IsDebugEnabled 
        { 
            get
            {
                return Logger.IsDebugEnabled;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is information enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is information enabled; otherwise, <c>false</c>.</value>
        public static bool IsInfoEnabled 
        {
            get { return Logger.IsInfoEnabled; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is error enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is error enabled; otherwise, <c>false</c>.</value>
        public static bool IsErrorEnabled 
        {
            get { return Logger.IsErrorEnabled; }
        }
        /// <summary>
        /// Debugs the format.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="args">The arguments.</param>
        public static void DebugFormat(string s, params object[] args)
        {
            Logger.Debug(s,args);
        }

        /// <summary>
        /// Informations the format.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="args">The arguments.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void InfoFormat(string s, params object[] args)
        {
            Logger.Info(s,args);
        }

        /// <summary>
        /// Errors the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void Error(string format, Exception exception)
        {
            Logger.Error(format,exception);
        }

        /// <summary>
        /// Warns the format.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="args">The arguments.</param>
        public static void WarnFormat(string s, params object[] args)
        {
            Logger.Warn(s,args);
        }

        public static void LogException(Exception exception)
        {
            Logger.Error(exception.GetCombinedMessages());
        }
    }


    public class FxLog
    {

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger GetLogger(Type T)
        {
                try
                {
                    return LogManager.GetLogger(T.Name);
                }
                catch (Exception)
                {
                    return LogManager.GetCurrentClassLogger();
                }
        }


        public static void Debug(Type T, string s, params object[] args)
        {
            var log = GetLogger(T);
            log.Debug(s,args);
        }

        public static void Error(Type T, string message, object[] vals)
        {
            var log = GetLogger(T);
            log.Error(message, vals);
           
        }
    }

}
