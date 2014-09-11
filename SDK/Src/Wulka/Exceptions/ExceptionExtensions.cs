// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 06-22-2014
// ***********************************************************************
// <copyright file="ExceptionExtensions.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace Wulka.Exceptions
{
    /// <summary>
    /// Extends the Exception class with reusable code.
    /// </summary>
    public static class ExceptionExtensions
    {

        /// <summary>
        /// Extends Exception to return a combined message. The combined message contains the
        /// the message of the exception, optionally followed by the messages of the inner exceptions
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>System.String.</returns>
        public static string GetCombinedMessages(this Exception ex)
        {
            var result = string.Empty;
            var exRunner = ex;
            do
            {
                if (String.IsNullOrEmpty(result))
                {
                    result += "\n---> ";
                }
                result = String.Format("{0}{1}: {2}\n", result, exRunner.GetType().Name, exRunner.Message);
                result = String.Format("{0}\n\nSTACK TRACE:\n\t{1}", result, exRunner.StackTrace);
                exRunner = exRunner.InnerException;
            } while (exRunner != null);
            return result;
        }


        /// <summary>
        /// Gets the combined message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> GetCombinedMessage(this Exception ex)
        {
            var result = new List<string>();
            var exRunner = ex;
            do
            {
                result.Add(string.Format("{0}: {1}\n", exRunner.GetType().Name, exRunner.Message));
                exRunner = exRunner.InnerException;
            } while (exRunner != null);
            return result;
        }
    }
}
