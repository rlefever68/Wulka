using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Media;
using Wulka.BigD.Contract;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Domain.Base;
using Wulka.Interfaces;

namespace Wulka.Domain.Interfaces
{
    public interface IDomainObject : IResult, IDataErrorInfo, IDirty, IWriteable, IReadable, IHaveParent,IBigDbDocument, ISelfContained,IIndex
    {

        ImageSource Glyph { get; }
        ImageSource IconSource { get;}
        int TreeDepth { get; }
        object GetValue(string propName);
        void SetValue(string propName, object value);
        Icon Icon { get; set; }

        IDisplayInfo DisplayInfo { get; }


        [DataMember]
        RequestBase Request { get; set; }

        [DataMember]
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the additional information URI.
        /// </summary>
        /// <value>The additional information URI.</value>
        [DataMember]
        string AdditionalInfoUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [to be flushed].
        /// </summary>
        /// <value><c>true</c> if [to be flushed]; otherwise, <c>false</c>.</value>
        [DataMember]
        bool ToBeFlushed { get; set; }

        /// <summary>
        /// if Null no errors, empty strings means there are errors but no description
        /// </summary>
        /// <value>The error.</value>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        new string Error { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is unknown.
        /// </summary>
        /// <value><c>true</c> if this instance is unknown; otherwise, <c>false</c>.</value>
        bool IsUnknown { get; }
        Type SourceType { get; set; }
        string DocType { get; set; }
        int Priority { get; set; }
        string CorrelationId { get; set; }
        string SessionId { get; set; }
        byte[] Image { get; set;}

        [Browsable(false)]
        string[] Errors { get; set; }
        [Browsable(false)]
        bool HasErrors { get; }
        [Browsable(false)]
        string AllErrors { get; }
        [Browsable(false)]
        int ErrorCount { get; set; }


        [Browsable(false)]
        int WarningCount { get; set; }

        [Browsable(false)]
        bool HasWarnings { get; }


        [Browsable(false)]
        int InfoCount { get; set; }

        [Browsable(false)]
        bool HasInfos { get; }

        /// <summary>
        /// Gets or sets the reconcile by.
        /// </summary>
        /// <value>The reconcile by.</value>
        ReconcileStrategy ReconcileBy { get; set; }

        DateTime TouchedAt{ get; set; }


        /// <summary>
        /// Clears the info/warning/error messages in the Feedback result
        /// </summary>
        void ClearFeedback();


        /// <summary>
        /// Adds all the result items of given extended result to this instance
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="copyId"></param>
        /// <returns>Result.</returns>
        Result Merge(Result result, bool copyId = true);

        /// <summary>
        /// Adds the feedback.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>original result</returns>
        Result AddFeedback(Result result);

        /// <summary>
        /// Adds the feedback.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>original result</returns>
        Result AddFeedback(params Result[] results);

        /// <summary>
        /// Merges the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>Result.</returns>
        Result Merge(params Result[] results);

        /// <summary>
        /// Saves the commited.
        /// </summary>
        void SaveCommited();

        /// <summary>
        /// Called by the runtime when a conflict is detected during save. The supplied parameter
        /// is the database copy of the document being saved.
        /// </summary>
        /// <param name="databaseCopy">The database copy.</param>
        void Reconcile(IBigDbDocument databaseCopy);

        IDomainObject Owner { get; set; }
        IDomainObject MasterDoc { get; }
        [DataMember]
        string MasterDocId { get; set; }

 
    }
}