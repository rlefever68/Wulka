// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="XmlFormatter.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;
using System.Xml;
using NLog;
using Wulka.Exceptions;
using Wulka.Logging;

namespace Wulka.Diagnostics
{
    /// <summary>
    /// Helper class that provides method to convert the content of an Xml text to a readable format.
    /// It also provides methods to log the xml content
    /// </summary>
    public class XmlFormatter
    {
        /// <summary>
        /// Delegate LogHandler
        /// </summary>
        /// <param name="line">The line.</param>
        private delegate void LogHandler(string line);

        /// <summary>
        /// The _current level
        /// </summary>
        private int             _currentLevel;
        /// <summary>
        /// The _current line
        /// </summary>
        private string          _currentLine = string.Empty;
        /// <summary>
        /// The _current node name
        /// </summary>
        private string          _currentNodeName = string.Empty;
        /// <summary>
        /// The _string writer
        /// </summary>
        private StringWriter    _stringWriter;
        /// <summary>
        /// The _is debug
        /// </summary>
        private bool            _isDebug; // If true log debug else log info
        /// <summary>
        /// The _log handler
        /// </summary>
        private LogHandler      _logHandler;

        private Logger _logger;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="XmlFormatter" /> class.
        /// Private constructor to force use of static methods
        /// </summary>
        private XmlFormatter()
        {
        }

        #region Static Methods

        /// <summary>
        /// Formats the content of an xml string into a multiline string.
        /// </summary>
        /// <param name="inputStream">The input stream containing the xml.</param>
        /// <returns>A string containing the formatted xml</returns>
        public static string ToString(Stream inputStream)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(inputStream);
            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Exception in XmlFormatter: {0}", ex.Message);
            } 
            return new XmlFormatter().FormatToString(xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string.
        /// </summary>
        /// <param name="input">The input stream containing the xml.</param>
        /// <returns>A string containing the formatted xml</returns>
        public static string ToString(string input)
        {
            StringReader stringReader = new StringReader(input);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(stringReader);

            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Exception in XmlFormatter: {0}", ex.Message);
            }
            return new XmlFormatter().FormatToString(xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string.
        /// </summary>
        /// <param name="input">The input stream containing the xml.</param>
        /// <returns>A string containing the formatted xml</returns>
        public static string ToString(byte[] input)
        {
            MemoryStream memoryStream = new MemoryStream(input);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(memoryStream);
            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Exception in XmlFormatter: {0}", ex.Message);
            }
            return new XmlFormatter().FormatToString(xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string and
        /// logs it line by line as info.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input.</param>
        public static void LogInfo(Type loggerType, byte[] input)
        {
            XmlDocument xmlDocument = ToXmlDocument(input);
            new XmlFormatter().FormatToLogger(false, loggerType, xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string and
        /// logs it line by line as info.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="inputStream">The input stream containing the xml.</param>
        public static void LogInfo(Type loggerType, Stream inputStream)
        {
            XmlDocument xmlDocument = ToXmlDocument(inputStream);
            new XmlFormatter().FormatToLogger(false, loggerType, xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string and
        /// logs it line by line as info.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input.</param>
        public static void LogDebug(Type loggerType, byte[] input)
        {
            XmlDocument xmlDocument = ToXmlDocument(input);
            new XmlFormatter().FormatToLogger(true, loggerType, xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string into a multiline string and
        /// logs it line by line as info.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="inputStream">The input stream containing the xml.</param>
        public static void LogDebug(Type loggerType, Stream inputStream)
        {
            XmlDocument xmlDocument = ToXmlDocument(inputStream);
            new XmlFormatter().FormatToLogger(true, loggerType, xmlDocument);
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="inputStream">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogInfoSingleWrite(Type loggerType, Stream inputStream, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName,loggerType);
            if (logger.IsInfoEnabled)
            {
                logger.Info("{0}: \r\n{1}", description, ToString(inputStream));
            }
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogInfoSingleWrite(Type loggerType, string input, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            if (logger.IsInfoEnabled)
            {
                logger.Info("{0}: \r\n{1}", description, ToString(input));
            }
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogInfoSingleWrite(Type loggerType, byte[] input, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            if (logger.IsInfoEnabled)
            {
                logger.Info("{0}: \r\n{1}", description, ToString(input));
            }
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="inputStream">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogDebugSingleWrite(Type loggerType, Stream inputStream, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("{0}: \r\n{1}", description, ToString(inputStream));
            }
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogDebugSingleWrite(Type loggerType, string input, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("{0}: \r\n{1}", description, ToString(input));
            }
        }

        /// <summary>
        /// Formats the content of an xml string and logs it as one single string that starts
        /// with the given description followed by a colon and a new line.
        /// </summary>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="input">The input stream containing the xml.</param>
        /// <param name="description">The description to be logged in the first line.</param>
        public static void LogDebugSingleWrite(Type loggerType, byte[] input, string description)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("{0}: \r\n{1}", description, ToString(input));
            }
        }

        #endregion

        #region private functions

        /// <summary>
        /// To the XML document.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>XmlDocument.</returns>
        private static XmlDocument ToXmlDocument(byte[] input)
        {
            MemoryStream memoryStream = new MemoryStream(input);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(memoryStream);
            }
            catch (Exception ex)
            {
                
                FxLog<XmlFormatter>.DebugFormat("Failed to load XmlDocument from byte array --> {0}", ex.GetCombinedMessages());
            }
            return xmlDocument; // Empty if error occurred
        }

        /// <summary>
        /// To the XML document.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>XmlDocument.</returns>
        private static XmlDocument ToXmlDocument(Stream inputStream)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(inputStream);
            }
            catch (Exception ex)
            {
                FxLog<XmlFormatter>.DebugFormat("Failed to load XmlDocument from input stream --> {0}", ex.Message);
            }
            return xmlDocument; // Empty if error occurred
        }

        /// <summary>
        /// Formats the content of an xml document into a string.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>Returns the formatted xml</returns>
        private string FormatToString(XmlDocument xmlDocument)
        {
            _stringWriter = new StringWriter();
            try
            {
                WriteXml(xmlDocument);
                return _stringWriter.ToString();
            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Unhandled exception in XmlLogger: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Formats the content of an xml document and logs it line by line.
        /// </summary>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="loggerType">The type that is logging.</param>
        /// <param name="xmlDocument">The XML document.</param>
        private void FormatToLogger(bool isDebug, Type loggerType, XmlDocument xmlDocument)
        {
            _isDebug = isDebug;
            _logHandler = WriteToLogger;
            _logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            try
            {
                if ((_isDebug && _logger.IsDebugEnabled) || ((_isDebug == false) && _logger.IsInfoEnabled))
                {
                    WriteXml(xmlDocument);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unhandled exception in XmlLogger", ex);
            }
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        private void WriteXml(XmlDocument xmlDocument)
        {
            XmlNodeReader reader = null;

            try
            {
                //Create an XmlNodeReader to read the XmlDocument.
                reader = new XmlNodeReader(xmlDocument);

                //Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    UpdateLevel(reader.Depth);
                    //Write("{0}", reader.Depth.ToString("00"));
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (_currentNodeName != string.Empty)
                            {
                                WriteLine();
                            }
                            _currentNodeName = reader.Name;
                            Write("<{0}", reader.Name);
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    Write(" {0}=\"{1}\"", reader.Name, reader.Value);
                                }
                                Write(" ");
                                // Move the reader back to the element node.
                                reader.MoveToElement();
                            }
                            Write(reader.IsEmptyElement ? "/>" : ">");
                            if (reader.IsEmptyElement)
                            {
                                _currentNodeName = string.Empty;
                                WriteLine();
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (_currentNodeName == reader.Name)
                            {
                                _currentNodeName = string.Empty;
                            }
                            WriteLine("</{0}>", reader.Name);
                            break;
                        case XmlNodeType.Text:
                            Write(reader.Value);
                            break;
                        case XmlNodeType.CDATA:
                            Write(reader.Value);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.XmlDeclaration:
                            WriteLine("<?{0} {1}?>", reader.Name, reader.Value);
                            break;
                        case XmlNodeType.Comment:
                            WriteLine("<!--{0}-->", reader.Value);
                            break;
                        case XmlNodeType.Document:
                            break;
                        default:
                            WriteLine();
                            WriteLine("ERROR: Unexpected node type ({0}) encountered", reader.NodeType);
                            WriteLine();
                            break;
                    }
                }
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Updates the level.
        /// </summary>
        /// <param name="level">The level.</param>
        private void UpdateLevel(int level)
        {
            if (level != _currentLevel)
            {
                _currentLevel = level;
            }
        }


        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        private void WriteLine(string format, params object[] args)
        {
            Write(format, args);
            WriteLine();
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        private void WriteLine()
        {
            if (_logHandler != null)
            {
                _logHandler(_currentLine);
            }
            if (_stringWriter != null)
            {
                _stringWriter.WriteLine(_currentLine);
            }
            _currentLine = string.Empty;
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        private void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Write(string value)
        {
            if (_currentLine == string.Empty)
            {
                _currentLine += new string(' ', _currentLevel * 4);
            }
            _currentLine += value;
        }

        /// <summary>
        /// Writes to logger.
        /// </summary>
        /// <param name="line">The line.</param>
        private void WriteToLogger(string line)
        {
            if (_isDebug)
            {
                _logger.Debug(line);
            }
            else
            {
                _logger.Info(line);
            }
        }

        #endregion

    }
}
