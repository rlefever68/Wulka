// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="ObjectLogger.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NLog;

namespace Wulka.Diagnostics
{
    /// <summary>
    /// Log helper class that provides a method to log the content of any object
    /// of a serializable Type.
    /// </summary>
    public class ObjectLogger
    {
        /// <summary>
        /// Defines the interface of a log writer
        /// </summary>
        internal interface IWriter
        {
            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="value">The value.</param>
            void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, string typeName, object value);

            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="value">The value.</param>
            void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, int? arrayIndex, object value);

            /// <summary>
            /// Writes the name of the array or list.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="arrayOrListName">Name of the array or list.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="arrayLength">Length of the array.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            void WriteArrayOrListName(int depth, int logOffset, string declaringTypePrefix, string arrayOrListName, string typeName, int arrayLength, string valueSuffix);

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="name">The name.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            void WriteClassOrStructName(int depth, int logOffset, string declaringTypePrefix, string name, string typeName, string valueSuffix);

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="name">The name.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            void WriteClassOrStructName(int depth, int logOffset, string name, int arrayIndex, string typeName, string valueSuffix);

            /// <summary>
            /// Writes the class or struct begin.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            void WriteClassOrStructBegin(int depth, int logOffset);

            /// <summary>
            /// Writes the class or struct item separator.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            void WriteClassOrStructItemSeparator(int depth, int logOffset);

            /// <summary>
            /// Writes the class or struct end.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            void WriteClassOrStructEnd(int depth, int logOffset);

            /// <summary>
            /// Writes the circular reference detected.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            void WriteCircularReferenceDetected(int depth, int logOffset);
        }

        /// <summary>
        /// Base class for implementing the log interface
        /// </summary>
        internal abstract class WriterBase : IWriter
        {
            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="format">The format.</param>
            /// <param name="arg">The arg.</param>
            public abstract void WriteLine(int depth, int offset, string format, params object[] arg);
            //public abstract void WriteLine(int offset, string format, params object[] arg);

            #region IWriter Members

            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="value">The value.</param>
            public void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, string typeName, object value)
            {
                // <offset><ObjectName> (<TypeName>) = <ObjectValue>
                WriteLine(depth, logOffset, "{0}{1} ({2}) = {3}",
                          declaringTypePrefix,
                          objectName,
                          typeName,
                          value);
            }

            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="value">The value.</param>
            public void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, int? arrayIndex, object value)
            {
                // Object is an array element
                // <offset><ObjectName>[<ArrayIndex>] = <ObjectValue>
                WriteLine(depth, logOffset, "{0}{1}[{2}] = {3}",
                          declaringTypePrefix,
                          objectName,
                          arrayIndex,
                          value);
            }

            /// <summary>
            /// Writes the name of the array or list.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="arrayOrListName">Name of the array or list.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="arrayLength">Length of the array.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteArrayOrListName(int depth, int logOffset, string declaringTypePrefix, string arrayOrListName, string typeName, int arrayLength, string valueSuffix)
            {
                // <offset>[DeclaringType.]<ArrayName> (<ArrayTypeName>, length=<ArrayLength>):
                WriteLine(depth, logOffset, "{0}{1} ({2}, length={3}):{4}",
                          declaringTypePrefix,
                          arrayOrListName,
                          typeName,
                          arrayLength,
                          valueSuffix);
            }

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="name">The name.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteClassOrStructName(int depth, int logOffset, string declaringTypePrefix, string name, string typeName, string valueSuffix)
            {
                // <offset>[DeclaringType.]<Name> (<TypeName>, depth = <depth>) =
                WriteLine(depth, logOffset, "{0}{1} ({2}) ={3}",
                          declaringTypePrefix,
                          name,
                          typeName,
                          valueSuffix);
            }

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="name">The name.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteClassOrStructName(int depth, int logOffset, string name, int arrayIndex, string typeName, string valueSuffix)
            {
                // <offset><Name>[<ArrayIndex>] (<TypeName>) =
                WriteLine(depth, logOffset, "{0}[{1}] ({2}) ={3}",
                          name,
                          arrayIndex,
                          typeName,
                          valueSuffix);
            }

            /// <summary>
            /// Writes the class or struct begin.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructBegin(int depth, int logOffset)
            {
                // No output;
            }

            /// <summary>
            /// Writes the class or struct item separator.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructItemSeparator(int depth, int logOffset)
            {
                // No output;
            }

            /// <summary>
            /// Writes the class or struct end.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructEnd(int depth, int logOffset)
            {
                // No output;
            }

            /// <summary>
            /// Writes the circular reference detected.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteCircularReferenceDetected(int depth, int logOffset)
            {
                WriteLine(depth, logOffset, "Circular reference detected, object already logged");
            }

            #endregion
        }

        /// <summary>
        /// Writes to a log file
        /// </summary>
        internal class LogWriter : WriterBase
        {
            private Logger _logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogWriter" /> class.
            /// </summary>
            /// <param name="logger">The logger.</param>
            public LogWriter(Logger logger)
            {
                _logger = logger;
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="format">The format.</param>
            /// <param name="arg">The arg.</param>
            public override void WriteLine(int depth, int offset, string format, params object[] arg)
            {
                string linePrefix = string.Format("{0} {1}", depth, new string(' ', offset));
                _logger.Debug(linePrefix + format, arg);
            }
        }

        /// <summary>
        /// Writes to a string
        /// </summary>
        internal class StringWriter : WriterBase
        {
            /// <summary>
            /// The _string writer
            /// </summary>
            private readonly System.IO.StringWriter _stringWriter;
            /// <summary>
            /// The _first line
            /// </summary>
            private bool _firstLine = true;

            /// <summary>
            /// Initializes a new instance of the <see cref="StringWriter" /> class.
            /// </summary>
            /// <param name="stringWriter">The string writer.</param>
            public StringWriter(System.IO.StringWriter stringWriter)
            {
                _stringWriter = stringWriter;
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="format">The format.</param>
            /// <param name="arg">The arg.</param>
            public override void WriteLine(int depth, int offset, string format, params object[] arg)
            {
                string linePrefix = string.Format("{0} {1}", depth, new string(' ', offset));
                if (_firstLine)
                {
                    _firstLine = false;
                }
                else
                {
                    _stringWriter.WriteLine();
                }
                _stringWriter.Write(linePrefix + format, arg);
            }
        }

        /// <summary>
        /// Class SimpleWriterBase.
        /// </summary>
        internal abstract class SimpleWriterBase : IWriter
        {
            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="format">The format.</param>
            /// <param name="arg">The arg.</param>
            public abstract void WriteLine(int depth, int offset, string format, params object[] arg);

            #region IWriter Members

            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="value">The value.</param>
            public void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, string typeName, object value)
            {
                // <ObjectName> = <ObjectValue>
                WriteLine(depth, 0, "{0} = {1}",
                          objectName,
                          value);
            }

            /// <summary>
            /// Writes the object name and value.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="value">The value.</param>
            public void WriteObjectNameAndValue(int depth, int logOffset, string declaringTypePrefix, string objectName, int? arrayIndex, object value)
            {
                // <ObjectName>[<ArrayIndex>] = <ObjectValue>
                // <ObjectValue>
                WriteLine(depth, 0, "{0}", value);
            }

            /// <summary>
            /// Writes the name of the array or list.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="arrayOrListName">Name of the array or list.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="arrayLength">Length of the array.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteArrayOrListName(int depth, int logOffset, string declaringTypePrefix, string arrayOrListName, string typeName, int arrayLength, string valueSuffix)
            {
                // <ArrayName>:
                WriteLine(depth, 0, "{0}[{1}] = ",
                    arrayOrListName,
                    arrayLength);
            }

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="declaringTypePrefix">The declaring type prefix.</param>
            /// <param name="name">The name.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteClassOrStructName(int depth, int logOffset, string declaringTypePrefix, string name, string typeName, string valueSuffix)
            {
                // <Name> =
                WriteLine(depth, 0, "{0} = ",
                          name);
            }

            /// <summary>
            /// Writes the name of the class or struct.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="name">The name.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="typeName">Name of the type.</param>
            /// <param name="valueSuffix">The value suffix.</param>
            public void WriteClassOrStructName(int depth, int logOffset, string name, int arrayIndex, string typeName, string valueSuffix)
            {
                // <Name>[<ArrayIndex>] =
                WriteLine(depth, 0, "{0}[{1}] = ",
                          name,
                          arrayIndex);
            }

            /// <summary>
            /// Writes the class or struct begin.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructBegin(int depth, int logOffset)
            {
                WriteLine(depth, logOffset, "{{ ");
            }

            /// <summary>
            /// Writes the class or struct item separator.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructItemSeparator(int depth, int logOffset)
            {
                WriteLine(depth, logOffset, ", ");
            }

            /// <summary>
            /// Writes the class or struct end.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteClassOrStructEnd(int depth, int logOffset)
            {
                WriteLine(depth, logOffset, " }}");
            }

            /// <summary>
            /// Writes the circular reference detected.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            public void WriteCircularReferenceDetected(int depth, int logOffset)
            {
                WriteLine(depth, logOffset, "Circular reference detected, object already logged");
            }

            #endregion
        }

        /// <summary>
        /// Class SingleLineStringWriter.
        /// </summary>
        internal class SingleLineStringWriter : SimpleWriterBase
        {
            /// <summary>
            /// The _string writer
            /// </summary>
            private readonly System.IO.StringWriter _stringWriter;

            /// <summary>
            /// Initializes a new instance of the <see cref="SingleLineStringWriter" /> class.
            /// </summary>
            /// <param name="stringWriter">The string writer.</param>
            public SingleLineStringWriter(System.IO.StringWriter stringWriter)
            {
                _stringWriter = stringWriter;
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="format">The format.</param>
            /// <param name="arg">The arg.</param>
            public override void WriteLine(int depth, int offset, string format, params object[] arg)
            {
                _stringWriter.Write(format, arg);
            }
        }

        /// <summary>
        /// Parses an object for all public properties and writes the property values to the given writer
        /// </summary>
        public class ObjectParser
        {
            /// <summary>
            /// Class DummyValue.
            /// </summary>
            internal class DummyValue
            {
                /// <summary>
                /// The _value
                /// </summary>
                private readonly string _value;

                /// <summary>
                /// Initializes a new instance of the <see cref="DummyValue" /> class.
                /// </summary>
                /// <param name="value">The value.</param>
                public DummyValue(string value)
                {
                    _value = value;
                }

                /// <summary>
                /// Returns a <see cref="System.String" /> that represents this instance.
                /// </summary>
                /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
                public override string ToString()
                {
                    return _value;
                }
            }

            /// <summary>
            /// The _object stack
            /// </summary>
            private readonly Stack<object> _objectStack = new Stack<object>(); // used to detect circular references
            /// <summary>
            /// The _requested depth
            /// </summary>
            private readonly int _requestedDepth;
            /// <summary>
            /// The _writer
            /// </summary>
            private readonly IWriter _writer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectParser" /> class.
            /// </summary>
            /// <param name="depth">The depth, if less than 0 unlimited, if 0, only the properties of the target object are logged,
            /// if greater than 0, properties of contained objects are logged until the requested depth is reached.</param>
            /// <param name="writer">The writer.</param>
            internal ObjectParser(int depth, IWriter writer)
            {
                _requestedDepth = depth;
                _writer = writer;
            }

            /// <summary>
            /// Entry point for parsing an object.
            /// </summary>
            /// <param name="objectValue">The object value.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
            public void ParseObject(object objectValue, string objectName, int indentation)
            {
                // If the target object is null, we cannot obtain its type, therefore we give it the type of object
                Type objectType = (objectValue == null) ? typeof(object) : objectValue.GetType();
                ParseObject(objectValue, objectName, null, objectType, indentation, 0, null, false);
            }

            /// <summary>
            /// Parses an object for logging.  The object may be part of an array
            /// </summary>
            /// <param name="objectValue">The object value.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="declaringType">For class or struct, the type that declares the object.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="depth">The depth.</param>
            /// <param name="arrayIndex">Index of the object in an array.</param>
            /// <param name="objectTypeIsNullable">if set to <c>true</c> [object type is nullable].</param>
            /// <exception cref="System.NotSupportedException">
            /// </exception>
            private void ParseObject(object objectValue, string objectName, Type declaringType, Type objectType, int logOffset, int depth,
                                     int? arrayIndex, bool objectTypeIsNullable)
            {
                if ((objectType == null) && (objectValue != null))
                {
                    objectType = objectValue.GetType();
                }
                if (objectType == null)
                {
                    WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                }
                else if ((objectType.IsGenericType) && (objectType.Namespace == "System") && (objectType.Name == "Nullable`1"))
                {
                    Type[] genericArguments = objectType.GetGenericArguments();
                    Type genericType = genericArguments[0];
                    const bool isNullableType = true;
                    ParseObject(objectValue, objectName, declaringType, genericType, logOffset, depth, arrayIndex, isNullableType);
                }
                else if ((objectType.IsGenericType) && (objectType.Namespace == "System.Collections.Generic") && (objectType.Name == "List`1"))
                {
                    //if (IsRequestedDepthExceeded(depth))
                    //{
                    //    // Log object name, type and length
                    //    IEnumerator enumerator = ((IEnumerable)objectValue).GetEnumerator();
                    //    int length = 0;
                    //    while (enumerator.MoveNext())
                    //    {
                    //        length++;
                    //    }
                    //    WriteArrayOrListName(GetDepthExceededSuffix(), objectName, declaringType, objectType, length, logOffset, depth);
                    //}
                    //else // Log content of children
                    //{
                    WriteClassOrStructName(null, objectName, declaringType, objectType, logOffset, arrayIndex, depth);

                    IEnumerator enumerator = ((IEnumerable)objectValue).GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                    {
                        object elementValue = enumerator.Current;
                        ParseObject(elementValue, objectName, declaringType, null, logOffset + 4, depth, i++, false);
                    }
                    //}
                }
                //else if (objectType.IsGenericType)
                //{
                //    WriteClassOrStructName(null, objectName, declaringType, objectType, logOffset, arrayIndex);
                //    ParseGenericType(objectValue, objectName, declaringType, objectType, logOffset+4, arrayIndex, objectTypeIsNullable);
                //}
                else if (objectValue == null)
                {
                    WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                }
                else if (objectType.IsPrimitive)
                {
                    WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                }
                else if (objectType.IsEnum)
                {
                    WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                }
                else if (objectType.IsArray)
                {
                    // Reject multi-dimentional arrays
                    if (objectType.GetArrayRank() != 1)
                    {
                        throw new NotSupportedException(
                            String.Format("{0}.{1}: Serialization of multi-dimentional arrays not supported",
                                          GetType().Name, MethodBase.GetCurrentMethod().Name));
                    }

                    Array arrayValue = (Array)objectValue; // Here it is never null
                    int arrayLength = arrayValue.Length;

                    WriteArrayOrListName(null, objectName, declaringType, objectType, arrayLength, logOffset, depth);
                    _writer.WriteClassOrStructBegin(depth, logOffset);

                    // Parse array elements
                    if (arrayLength > 0)
                    {
                        for (int i = 0; i < arrayLength; i++)
                        {
                            object elementValue = arrayValue.GetValue(i);

                            if (i > 0)
                            {
                                _writer.WriteClassOrStructItemSeparator(depth, logOffset);
                            }
                            ParseObject(elementValue, objectName, declaringType, null, logOffset + 4, depth, i, objectTypeIsNullable);
                        }
                    }
                    _writer.WriteClassOrStructEnd(depth, logOffset);
                }
                else if (objectType.IsValueType) // must be a struct
                {
                    if (objectType.Equals(typeof(DateTime)))
                    {
                        WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                    }
                    else if (objectType.Equals(typeof(Decimal)))
                    {
                        WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                    }
                    else
                    {
                        if (IsRequestedDepthReached(depth))
                        {
                            DummyValue dummyValue = new DummyValue("<...>");
                            WriteObjectNameAndValue(depth, dummyValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                            //WriteClassOrStructName(GetDepthExceededSuffix(), objectName, declaringType, objectType, logOffset, arrayIndex, depth);
                        }
                        else // Log content one level deeper
                        {
                            WriteClassOrStructName(null, objectName, declaringType, objectType, logOffset, arrayIndex, depth);
                            _writer.WriteClassOrStructBegin(depth, logOffset);
                            ParseClassOrStruct(objectValue, logOffset + 4, depth + 1);
                            _writer.WriteClassOrStructEnd(depth, logOffset);
                        }
                    }
                }
                else if (objectType.IsClass)
                {
                    if (objectType.Equals(typeof(string)))
                    {
                        WriteObjectNameAndValue(depth, objectValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                    }
                    else
                    {
                        if (IsRequestedDepthReached(depth))
                        {
                            DummyValue dummyValue = new DummyValue("<...>");
                            WriteObjectNameAndValue(depth, dummyValue, objectName, declaringType, objectType, logOffset, arrayIndex, objectTypeIsNullable);
                            //WriteClassOrStructName(GetDepthExceededSuffix(), objectName, declaringType, objectType, logOffset, arrayIndex, depth);
                        }
                        else // Log content
                        {
                            WriteClassOrStructName(null, objectName, declaringType, objectType, logOffset, arrayIndex, depth);
                            _writer.WriteClassOrStructBegin(depth, logOffset);
                            ParseClassOrStruct(objectValue, logOffset + 4, depth + 1);
                            _writer.WriteClassOrStructEnd(depth, logOffset);
                        }
                    }
                }
                else if (objectType.IsInterface)
                {
                    WriteClassOrStructName("<Interface logging not yet supported>", objectName, declaringType, objectType, logOffset, arrayIndex, depth);
                    //ParseInterface(objectType, objectValue, logOffset + 4);
                }
                else
                {
                    throw new NotSupportedException(
                        String.Format("{0}.{1}: Serialization of type {2} not supported",
                                      GetType().Name,
                                      MethodBase.GetCurrentMethod().Name,
                                      objectType.FullName));
                }
            }

            /// <summary>
            /// Determines whether [is requested depth reached] [the specified depth].
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <returns><c>true</c> if [is requested depth reached] [the specified depth]; otherwise, <c>false</c>.</returns>
            private bool IsRequestedDepthReached(int depth)
            {
                bool isRequestedDepthReached = ((_requestedDepth >= 0) && (depth >= _requestedDepth));
                return isRequestedDepthReached;
            }

            /// <summary>
            /// Parses an instance of a class or struct.
            /// </summary>
            /// <param name="objectValue">The object value.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="depth">The depth.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            private void ParseClassOrStruct(object objectValue, int logOffset, int depth)
            {
                // Check if on the stack, if so log warning and return
                if (_objectStack.Contains(objectValue))
                {
                    WriteCircularReferenceDetected(depth, logOffset);
                    return;
                }
                _objectStack.Push(objectValue);

                Type objType = objectValue.GetType();

                // Get inheritance tree for this object
                Type[] inheritanceList = GetInheritanceList(objType);

                // Get the list of all public instance members of this object
                //MemberInfo[] memberInfos = FormatterServices.GetSerializableMembers(objType);
                MemberInfo[] memberInfos = objType.FindMembers(
                    MemberTypes.Field | MemberTypes.Property,
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    null);

                bool isFirstField = true;

                // Following loop is needed to change the order of processing of fields, we start with fields of
                // base class and up to the fields of the lowest derived class
                foreach (Type inheritanceType in inheritanceList)
                {
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        // Skip this member, if it does not belong 
                        // to the current type in the inheritance tree
                        if (!memberInfo.DeclaringType.Equals(inheritanceType))
                        {
                            continue;
                        }

                        string memberName;
                        Type memberType;
                        Type declaringType;
                        object memberValue;

                        // If Nonserialized attribute, don't log the content
                        bool nonSerialized = false;
                        if (memberInfo.GetCustomAttributes(typeof(NoObjectLoggingAttribute), false).Length > 0)
                        {
                            nonSerialized = true;
                        }

                        // Process this member
                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo fieldInfo = (FieldInfo)memberInfo;
                                memberName = fieldInfo.Name;
                                memberType = fieldInfo.FieldType;
                                declaringType = fieldInfo.DeclaringType;
                                memberValue = fieldInfo.GetValue(objectValue);
                                break;
                            case MemberTypes.Property:
                                PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                                memberName = propertyInfo.Name;
                                memberType = propertyInfo.PropertyType;
                                declaringType = propertyInfo.DeclaringType;
                                ParameterInfo[] parameterInfos = propertyInfo.GetIndexParameters();
                                // TODO MDG: Add support for indexed properties
                                if (parameterInfos.Length == 0) // According to ReSharper, parameterInfos is never null
                                {
                                    memberValue = propertyInfo.GetValue(objectValue, null);
                                }
                                else
                                {
                                    memberValue = "--- Display of Indexer not yet supported";
                                }
                                break;
                            default:
                                throw new NotSupportedException(
                                    String.Format("{0}.{1}: Parsing of a member of type {2} not supported",
                                                  GetType().Name,
                                                  MethodBase.GetCurrentMethod().Name,
                                                  memberInfo.MemberType));
                        }
                        if (isFirstField == false)
                        {
                            _writer.WriteClassOrStructItemSeparator(depth, logOffset);
                        }
                        if (nonSerialized)
                        {
                            memberValue = "Not logged (has NoObjectLogging attribute)";
                            WriteClassOrStructName(memberValue, memberName, declaringType, memberType, logOffset, null, depth);
                        }
                        else
                        {
                            ParseObject(memberValue, memberName, declaringType, memberType, logOffset, depth, null, false);
                        }
                        isFirstField = false;
                    }
                }
                _objectStack.Pop();
            }

            /// <summary>
            /// Gets the inheritance list for the specified type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns>Type[].</returns>
            private static Type[] GetInheritanceList(Type type)
            {
                ArrayList arrayList = new ArrayList();
                Type parentType = type;
                while (parentType != null)
                {
                    if ((parentType.BaseType == null) || (parentType == typeof(ValueType)))
                        break;
                    arrayList.Add(parentType);  // So, do not add System.Object to the list
                    parentType = parentType.BaseType;
                }
                arrayList.Reverse();
                return (Type[])arrayList.ToArray(typeof(Type));
            }

            ///// <summary>
            ///// Parses an object behind an interface.
            ///// </summary>
            ///// <param name="type">The type of the interface.</param>
            ///// <param name="objectValue">The object value behind this interface.</param>
            ///// <param name="logOffset">The log offset.</param>
            //private void ParseInterface(Type type, object objectValue, int logOffset)
            //{
            //}

            /// <summary>
            /// Logs the object, the object may be part of an array
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="objectValue">The object value.</param>
            /// <param name="objectName">Name of the object.</param>
            /// <param name="declaringType">For class or struct, the type that declares the object.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="nullable">if set to <c>true</c> the objectType is [nullable].</param>
            private void WriteObjectNameAndValue(int depth, object objectValue, string objectName, Type declaringType, Type objectType, int logOffset, int? arrayIndex, bool nullable)
            {
                string typeName = "";
                object value;

                if (objectType != null)
                {
                    typeName = objectType.Name;
                }

                if (nullable)
                {
                    typeName += "?";
                }

                if ((objectValue == null) || (objectType == null))
                {
                    value = "null";
                }
                else if (objectValue.GetType().Equals(typeof(DummyValue)))
                {
                    value = objectValue;
                }
                else if (objectType.IsPrimitive)
                {
                    value = objectValue;
                }
                else if (objectType.IsEnum)
                {
                    if (objectValue.GetType().Equals(typeof(string)))
                    {
                        // If the value type is a string, it is not the actual value of the enumerator that is logged
                        // but a descriptive string (e.g. <SKIPPED...)
                        value = objectValue;
                    }
                    else
                    {
                        // The value type is an enumerator type
                        // Represent the value both as the symbolic name and (between brackets) the numeric value
                        value = String.Format("{0} ({1})", objectValue, Enum.Format(objectType, objectValue, "d"));
                    }
                }
                else if (objectType.Equals(typeof(DateTime)))
                {
                    value = ((DateTime)objectValue).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (objectType.Equals(typeof(Decimal)))
                {
                    value = String.Format("{0}", objectValue);
                }
                else if (objectType.Equals(typeof(string)))
                {
                    value = String.Format("'{0}'", objectValue);
                }
                else
                {
                    value = null;
                }

                if (arrayIndex == null)
                {
                    // <offset><ObjectName> (<TypeName>) = <ObjectValue>
                    _writer.WriteObjectNameAndValue(depth, logOffset,
                              GetDeclaringTypePrefix(declaringType),
                              objectName,
                              typeName,
                              value);
                }
                else
                {
                    // Object is an array element
                    // <offset><ObjectName>[<ArrayIndex>] = <ObjectValue>
                    _writer.WriteObjectNameAndValue(depth, logOffset,
                              GetDeclaringTypePrefix(declaringType),
                              objectName,
                              arrayIndex,
                              value);
                }
            }

            /// <summary>
            /// Writes the name of the array or list.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="arrayOrListName">Name of the array or list.</param>
            /// <param name="declaringType">Type of the declaring.</param>
            /// <param name="arrayOrListType">Type of the array or list.</param>
            /// <param name="arrayLength">Length of the array.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="depth">The depth.</param>
            private void WriteArrayOrListName(object value, string arrayOrListName, Type declaringType, Type arrayOrListType, int arrayLength, int logOffset, int depth)
            {
                string valueSuffix = string.Empty;
                if (value != null)
                {
                    valueSuffix = " " + value;
                }
                string typeName;
                if (arrayOrListType.IsGenericType == false)
                {
                    typeName = arrayOrListType.Name;
                }
                else
                {
                    typeName = string.Empty;
                    foreach (Type genericArgument in arrayOrListType.GetGenericArguments())
                    {
                        if (string.IsNullOrEmpty(typeName))
                        {
                            int quoteIndex = arrayOrListType.Name.IndexOf('`');
                            typeName = arrayOrListType.Name.Substring(0, quoteIndex);
                            typeName += "<";
                        }
                        else
                        {
                            typeName += ",";
                        }
                        typeName += genericArgument.Name;
                    }
                    typeName += ">";
                }
                // <offset>[DeclaringType.]<ArrayName> (<ArrayTypeName>, length=<ArrayLength>):
                _writer.WriteArrayOrListName(depth,
                          logOffset, GetDeclaringTypePrefix(declaringType), arrayOrListName, typeName, arrayLength, valueSuffix);
            }

            /// <summary>
            /// Writes the name of the class or structure.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="name">The name.</param>
            /// <param name="declaringType">Type of the declaring.</param>
            /// <param name="type">The type.</param>
            /// <param name="logOffset">The log offset.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            /// <param name="depth">The depth.</param>
            private void WriteClassOrStructName(object value, string name, Type declaringType, Type type, int logOffset, int? arrayIndex, int depth)
            {
                string valueSuffix = string.Empty;
                if (value != null)
                {
                    valueSuffix = " " + value;
                }
                if (arrayIndex == null) // Not an array element
                {
                    // <offset>[DeclaringType.]<Name> (<TypeName>, depth = <depth>) =
                    _writer.WriteClassOrStructName(depth,
                              logOffset, GetDeclaringTypePrefix(declaringType), name, GetTypeName(type), valueSuffix);
                }
                else
                {
                    // <offset><Name>[<ArrayIndex>] (<TypeName>) =
                    _writer.WriteClassOrStructName(depth,
                              logOffset, name, (int)arrayIndex, GetTypeName(type), valueSuffix);
                }
            }

            /// <summary>
            /// Writes the circular reference detected.
            /// </summary>
            /// <param name="depth">The depth.</param>
            /// <param name="logOffset">The log offset.</param>
            private void WriteCircularReferenceDetected(int depth, int logOffset)
            {
                _writer.WriteCircularReferenceDetected(depth, logOffset);
            }

            /// <summary>
            /// Gets the declaring type prefix.
            /// </summary>
            /// <param name="declaringType">Type of the declaring.</param>
            /// <returns>System.String.</returns>
            private static string GetDeclaringTypePrefix(Type declaringType)
            {
                string declaringTypePrefix = null;

                // Generate a declaringType prefix if declaring type is set
                if ((declaringType != null))
                {
                    string typeName = GetTypeName(declaringType);
                    declaringTypePrefix = typeName + ".";
                }
                return declaringTypePrefix;
            }

            /// <summary>
            /// Gets the name of the type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns>System.String.</returns>
            private static string GetTypeName(Type type)
            {
                string typeName = null;

                if ((type != null))
                {
                    if (type.IsGenericType == false)
                    {
                        typeName = type.Name;
                    }
                    else
                    {
                        bool firstParam = true;
                        string[] typeNameElements = type.Name.Split(new[] { '`' });
                        typeName = typeNameElements[0];
                        typeName += "<";
                        foreach (Type genericArgument in type.GetGenericArguments())
                        {
                            if (firstParam)
                            {
                                firstParam = false;
                            }
                            else
                            {
                                typeName += ",";
                            }
                            typeName += genericArgument.Name;
                        }
                        typeName += ">";
                    }
                }
                return typeName;
            }
        }

        #region Static Methods

        /// <summary>
        /// Logs the content of an object into a string with default offset of 0 and full depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(object objectValue, string objectName)
        {
            return ToString(objectValue, objectName, 0);
        }

        /// <summary>
        /// Logs the content of an object of a serializable type into a string with given offset and full depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(object objectValue, string objectName, int indentation)
        {
            return ToString(objectValue, objectName, indentation, -1);
        }

        /// <summary>
        /// Logs the content of an object of a serializable type into a string with given offset and depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
        /// <param name="depth">The depth, if less than 0 unlimited, if 0, only the properties of the target object are logged,
        /// if greater than 0, properties of contained objects are logged until the requested depth is reached.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(object objectValue, string objectName, int indentation, int depth)
        {
            return LogToString(objectValue, objectName, indentation, depth);
        }

        /// <summary>
        /// Logs the content of an object of a serializable type into a string with given offset and default depth of 1.
        /// The content is loggged on a single line.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>System.String.</returns>
        public static string ToStringSingleLine(object objectValue, string objectName)
        {
            return LogToStringSingleLine(objectValue, objectName);
        }

        /// <summary>
        /// Logs the content of an object to a logger with default indentation of 0 and full depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4net logger to be used.</param>
        public static void ToLogger(object objectValue, string objectName, string log4NetLogName)
        {
            ToLogger(objectValue, objectName, log4NetLogName, 0);
        }

        /// <summary>
        /// Logs the content of an object to a logger with given indentation and full depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4net logger to be used.</param>
        /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
        public static void ToLogger(object objectValue, string objectName, string log4NetLogName, int indentation)
        {
            ToLogger(objectValue, objectName, log4NetLogName, indentation, -1);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="loggerType">Type of the logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The indentation.</param>
        public static void LogDebug(Type loggerType, object objectValue, string objectName, int indentation)
        {
            LogDebug(loggerType, objectValue, objectName, indentation, -1);
        }

        /// <summary>
        /// Logs the content of an object to a logger with given offset and depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4net logger to be used.</param>
        /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
        /// <param name="depth">The depth, if less than 0 unlimited, if 0, only the properties of the target object are logged,
        /// if greater than 0, properties of contained objects are logged until the requested depth is reached.</param>
        public static void ToLogger(object objectValue, string objectName, string log4NetLogName, int indentation, int depth)
        {
            LogDebugToLogger(objectValue, objectName, log4NetLogName, indentation, depth);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="loggerType">Type of the logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="depth">The depth.</param>
        public static void LogDebug(Type loggerType, object objectValue, string objectName, int indentation, int depth)
        {
            LogDebugToLogger(loggerType, objectValue, objectName, indentation, depth);
        }

        /// <summary>
        /// Logs the content of an object to a logger with default indentation of 0 and full depth.
        /// </summary>
        /// <param name="objectValue">The object to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4net logger to be used.</param>
        public static void ToLoggerSingleLine(object objectValue, string objectName, string log4NetLogName)
        {
            LogToLoggerSingleLine(objectValue, objectName, log4NetLogName);
        }

        /// <summary>
        /// Logs the debug to logger.
        /// </summary>
        /// <param name="loggerType">Type of the logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="depth">The depth.</param>
        public static void LogDebugToLogger(Type loggerType, object objectValue, string objectName, int indentation, int depth)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName, loggerType);
            LogDebugToLogger(logger, objectValue, objectName, indentation, depth);
        }

        /// <summary>
        /// Logs the debug to logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="depth">The depth.</param>
        public static void LogDebugToLogger(Logger logger, object objectValue, string objectName, int indentation, int depth)
        {
            try
            {
                if (logger.IsDebugEnabled)
                {
                    IWriter writer = new LogWriter(logger);
                    ObjectParser objectParser = new ObjectParser(depth, writer);
                    objectParser.ParseObject(objectValue, objectName, indentation);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Unhandled exception in ObjectLogger", ex);
            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Logs the debug to logger.
        /// </summary>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4 net log.</param>
        /// <param name="indentation">The indentation.</param>
        /// <param name="depth">The depth.</param>
        private static void LogDebugToLogger(object objectValue, string objectName, string log4NetLogName, int indentation, int depth)
        {
            Logger logger = LogManager.GetLogger(log4NetLogName);
            LogDebugToLogger(logger, objectValue, objectName, indentation, depth);
        }

        /// <summary>
        /// Logs to logger single line.
        /// </summary>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="log4NetLogName">Name of the log4 net log.</param>
        private static void LogToLoggerSingleLine(object objectValue, string objectName, string log4NetLogName)
        {
            Logger logger = LogManager.GetLogger(log4NetLogName);
            LogToLoggerSingleLine(logger, objectValue, objectName);
        }

        /// <summary>
        /// Logs to logger single line.
        /// </summary>
        /// <param name="loggerType">Type of the logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        private static void LogToLoggerSingleLine(Type loggerType, object objectValue, string objectName)
        {
            Logger logger = LogManager.GetLogger(loggerType.FullName,loggerType);
            LogToLoggerSingleLine(logger, objectValue, objectName);
        }

        /// <summary>
        /// Logs to logger single line.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        private static void LogToLoggerSingleLine(Logger logger, object objectValue, string objectName)
        {
            try
            {
                if (logger.IsDebugEnabled)
                {
                    System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                    IWriter writer = new SingleLineStringWriter(stringWriter);
                    ObjectParser objectParser = new ObjectParser(1, writer);
                    objectParser.ParseObject(objectValue, objectName, 0);
                    logger.Debug(stringWriter.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Unhandled exception in ObjectLogger", ex);
            }
        }

        /// <summary>
        /// Logs the content of an object into a string.
        /// </summary>
        /// <param name="objectValue">The object value to be logged.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="indentation">The requested indentation in number of blanks to prefix each line.</param>
        /// <param name="depth">The depth, if 0 unlimited, if 1, only the properties of the target object are logged,
        /// if greater than 1, properties of contained objects are logged until the requested depth is reached.</param>
        /// <returns>A string representing the content of the object value in a readable format</returns>
        private static string LogToString(object objectValue, string objectName, int indentation, int depth)
        {
            try
            {
                System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                IWriter writer = new StringWriter(stringWriter);
                ObjectParser objectParser = new ObjectParser(depth, writer);
                objectParser.ParseObject(objectValue, objectName, indentation);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Unhandled exception in ObjectLogger: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Logs to string single line.
        /// </summary>
        /// <param name="objectValue">The object value.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>System.String.</returns>
        private static string LogToStringSingleLine(object objectValue, string objectName)
        {
            try
            {
                System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                IWriter writer = new SingleLineStringWriter(stringWriter);
                ObjectParser objectParser = new ObjectParser(1, writer);
                objectParser.ParseObject(objectValue, objectName, 0);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                return String.Format("ERROR: Unhandled exception in ObjectLogger: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        #endregion

    }
}
