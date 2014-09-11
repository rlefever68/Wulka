// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-20-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-22-2013
// ***********************************************************************
// <copyright file="Result.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Wulka.BigD.Contract;
using Wulka.ErrorHandling;
using Wulka.Interfaces;
using Wulka.Utils;
using Wulka.Utils.Json;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// This class holds possible errors returned by the business logic
    /// </summary>
    /// <remarks>Use Result.Errors for situations that are not 'Exception'-al.
    /// For example: business validation errors are more fit to be put into this class than to raise an exception for them.</remarks>
    [DataContract]
    public class Result : BigDDocument, IResult
    {

        /// <summary>
        /// Class Property.
        /// </summary>
        private class Property
        {
            /// <summary>
            /// The identifier
            /// </summary>
            public const string Id = "Id";
            /// <summary>
            /// The priority
            /// </summary>
            public const string Priority = "Priority";
            /// <summary>
            /// The rev
            /// </summary>
            public const string Rev = "Rev";
        }




        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember]
        public new string Id
        {
            get 
            { 
                return base.Id; 
            }
            set
            {
                var newId = value;
                if (!String.IsNullOrEmpty(value))
                {
                    if(value.Contains("//"))
                        newId = value.Replace("//", "");
                    if(newId.Contains('/'))
                        newId = newId.Replace('/', '-');
                }
                else
                {
                    newId = GuidUtils.NullGuid;
                }
                base.Id = newId;                    
                RaisePropertyChanged(Property.Id);
            }
        }

        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>
        /// <value>The type of the document.</value>
        [DataMember]
        public string DocType
        {
            get { return GetDocType(); }
            set { _docType = value; }
        }

        protected virtual string GetDocType()
        {
            return GetType().ToString();
        }


        /// <summary>
        /// Gets or sets the rev.
        /// </summary>
        /// <value>The rev.</value>
        [DataMember]
        public new string Rev
        {
            get
            {
                return base.Rev;
            }
            set
            {
                base.Rev = value;
                RaisePropertyChanged(Property.Rev);
            }

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Result" /> class.
        /// </summary>
        public Result()
        {
            _errors = new List<string>();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Result" /> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public Result(Result seed)
        {
            _errors = new List<string>();
            if (seed != null)
                Merge(seed);
        }


        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        [DataMember]
        public int Priority { get; set; }


        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        /// <value>The correlation id.</value>
        [DataMember]
        public string CorrelationId { get; set; }

        //#region IExtensibleDataObject Members

        ///// <summary>
        ///// Gets or sets the structure that contains extra data.
        ///// </summary>
        ///// <value>The extension data.</value>
        ///// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject" /> that contains data that is not recognized as belonging to the data contract.</returns>
        //[Browsable(false)]
        //public ExtensionDataObject ExtensionData { get; set; }

        //#endregion

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        /// <value>The session id.</value>
        [DataMember]
        [Browsable(false)]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>The errors.</value>
        private List<string> _errors;

        /// <summary>
        /// The _doc type
        /// </summary>
        private string _docType;


        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>The errors.</value>
        [DataMember]
        [Browsable(false)]
        public string[] Errors
        {
            get
            {
                return _errors.ToArray();
            }

            set
            {
                _errors = new List<string>();
                _errors.AddRange(value);
            }

        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value><c>true</c> if this instance has errors; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool HasErrors
        {
            get { return ErrorCount != 0; }
        }

        /// <summary>
        /// returns newline-seperated string containing all error messages
        /// </summary>
        /// <value>All errors.</value>
        [Browsable(false)]
        public string AllErrors
        {
            get
            {
                return Errors.Aggregate(string.Empty, (current, error) => current + (error + Environment.NewLine));
                //short notation for:
                //string retVal = string.Empty;
                //foreach (Error error in Errors)
                //{
                //    retVal += error + Environment.NewLine;
                //}

                //return retVal;
            }
        }

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Result.</returns>
        public Result AddError(string message)
        {
            _errors.Add(message);
            ErrorCount++;
            return this;
        }

        /// <summary>
        /// Adds the errors.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns>Result.</returns>
        public Result AddErrors(IEnumerable<string> messages)
        {
            var errors = messages as string[] ?? messages.ToArray();
            _errors.AddRange(errors);
            ErrorCount += errors.Count();
            return this;
        }

        /// <summary>
        /// Adds the warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Result.</returns>
        public Result AddWarning(string message)
        {
            _errors.Add(message);
            WarningCount++;
            return this;
        }

        /// <summary>
        /// Adds the warnings.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns>Result.</returns>
        public Result AddWarnings(IEnumerable<string> messages)
        {
            var warnings = messages as string[] ?? messages.ToArray();
            _errors.AddRange(warnings);
            WarningCount += warnings.Count();
            return this;
        }

        /// <summary>
        /// Adds the info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Result.</returns>
        public Result AddInfo(string message)
        {
            _errors.Add(message);
            InfoCount++;
            return this;
        }

        /// <summary>
        /// Adds the infos.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns>Result.</returns>
        public Result AddInfos(IEnumerable<string> messages)
        {
            var infos = messages as string[] ?? messages.ToArray();
            _errors.AddRange(infos);
            InfoCount += infos.Count();
            return this;
        }


        /// <summary>
        /// Clears the info/warning/error messages in the Feedback result
        /// </summary>
        public void ClearFeedback()
        {
            _errors.Clear();
            ErrorCount = 0;
            WarningCount = 0;
            InfoCount = 0;
        }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>The error count.</value>
        [DataMember]
        [Browsable(false)]
        public int ErrorCount { get; set; }


        /// <summary>
        /// Gets the warning count.
        /// </summary>
        /// <value>The warning count.</value>
        [DataMember]
        [Browsable(false)]
        public int WarningCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value><c>true</c> if this instance has warnings; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool HasWarnings
        {
            get { return WarningCount != 0; }
        }

        /// <summary>
        /// Gets the info count.
        /// </summary>
        /// <value>The info count.</value>
        [DataMember]
        [Browsable(false)]
        public int InfoCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has infos.
        /// </summary>
        /// <value><c>true</c> if this instance has infos; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool HasInfos
        {
            get { return InfoCount != 0; }
        }

        /// <summary>
        /// Adds the specified error info.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        public void Add(ErrorInfo errorInfo)
        {
            switch (errorInfo.ErrorTypeValue)
            {
                case ErrorType.Error: AddError(errorInfo.ToString()); break;
                case ErrorType.Warning: AddWarning(errorInfo.ToString()); break;
                case ErrorType.Informational: AddInfo(errorInfo.ToString()); break;
                case ErrorType.Success: break;
            }
        }

        /// <summary>
        /// Adds all the result items of given extended result to this instance
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="copyId"></param>
        /// <returns>Result.</returns>
        public Result Merge(Result result, bool copyId=true)
        {
            if (result == null) return this;
            if(copyId) Id = result.Id;
            CorrelationId = result.CorrelationId;
            Priority = result.Priority;
            SessionId = result.SessionId;
            _errors.AddRange(result.Errors);
            ErrorCount += result.ErrorCount;
            WarningCount += result.WarningCount;
            InfoCount += result.InfoCount;
            return this;
        }

        /// <summary>
        /// Adds the feedback.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>original result</returns>
        public Result AddFeedback(Result result)
        {
            _errors.AddRange(result.Errors);
            ErrorCount += result.ErrorCount;
            WarningCount += result.WarningCount;
            InfoCount += result.InfoCount;

            return this;
        }


        /// <summary>
        /// Adds the feedback.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>original result</returns>
        public Result AddFeedback(params Result[] results)
        {
            foreach (var result in results)
            {
                AddFeedback(result);
            }
            return this;
        }


        /// <summary>
        /// Merges the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>Result.</returns>
        public Result Merge(params Result[] results)
        {
            foreach (Result result in results)
            {
                Merge(result);
            }
            return this;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Result target = (Result)obj;
            return
            ((ErrorCount == target.ErrorCount)
            && (WarningCount == target.WarningCount)
            && (InfoCount == target.InfoCount)
            && (this == obj)
            );
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return
            (ErrorCount.GetHashCode()
            ^ WarningCount.GetHashCode()
            ^ InfoCount.GetHashCode()
            );
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteJson(JsonWriter writer)
        {
            JSonHelper<Result>.WriteJSon(this, writer);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var T = GetType();
            var s = new StringBuilder();
            s.AppendFormat("\n");
            s.AppendFormat("\n");
            s.AppendFormat("---------------------------------------------\n");
            s.AppendFormat(" {0}: [{1}]\n", T.Name,  Id);
            s.AppendFormat("---------------------------------------------\n");
            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            return String.Format("{0}\n{1}", s, json);
        }




    }

}