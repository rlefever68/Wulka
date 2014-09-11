// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-31-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-24-2014
// ***********************************************************************
// <copyright file="DomainObject.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using NLog;
using Wulka.Domain.Interfaces;
using Wulka.Utils;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class DomainObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract partial class DomainObject<T> : Result, IDomainObject where T : IDomainObject
    {

        protected Logger Logger = LogManager.GetLogger(typeof(T).FullName);




        [OnDeserializing]
        private void InitFields(StreamingContext context)
        {
            if (Logger == null) Logger = LogManager.GetLogger(typeof(T).Name);
            if(_image==null) _image = new byte[] { };
            if(_ownedObjects==null) _ownedObjects = new List<IDomainObject>(); 
        }

             

        private string _viewName;

        [DataMember]
        public string ViewName
        {
            get { return _viewName; }
            set
            {
                _viewName = value;
                RaisePropertyChanged("ViewName");
            }
        }


        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [DataMember]
        public string DisplayName
        {
            get 
            { 
                return GetDisplayName(); 
            }
            set 
            { 
                _displayName = value; 
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetDisplayName()
        {
            return _displayName; 
        }


        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public Icon Icon
        {
            get { return GetIcon(); }
            set { _icon = value; }
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <returns>Icon.</returns>
        protected virtual Icon GetIcon()
        {
            return _icon ?? Properties.Resources.Unknown;
        }


        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <value>The display information.</value>
        public IDisplayInfo DisplayInfo
        {
            get
            {
                return GetDisplayInfo();
            }
        }


       


        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [DataMember]
        public byte[] Image
        {
            get { return GetImage(); }
            set { _image = value; }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        protected virtual byte[] GetImage()
        {
            return _image; 
        }

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>IDisplayInfo.</returns>
        protected virtual IDisplayInfo GetDisplayInfo()
        {
            return new DisplayInfo()
            {
                DisplayName = DisplayName,
                AdditionalInfoUri = AdditionalInfoUri,
                TouchedAt = TouchedAt,
                Icon = Icon.ToImageSource()
            };
        }



        /// <summary>
        /// Gets or sets the additional information URI.
        /// </summary>
        /// <value>The additional information URI.</value>
        [DataMember]
        public string AdditionalInfoUri
        {
            get 
            {
                return GetAdditionalInfoUri();
            }
            set 
            { 
                _additionalInfoUri = value; 
            }
        }

        protected virtual string GetAdditionalInfoUri()
        {
            return _additionalInfoUri;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [to be flushed].
        /// </summary>
        /// <value><c>true</c> if [to be flushed]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool ToBeFlushed { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DomainObject&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="copyId">if set to <c>true</c> [copy identifier].</param>
        protected DomainObject(DomainObject<T> seed, bool copyId=true)
        {
            Icon = null;
            if (seed == null) return;
            Merge(seed,copyId);
            ParentId = seed.ParentId;
            AdditionalInfoUri = seed.AdditionalInfoUri;
            DisplayName = seed.DisplayName;
            ToBeFlushed = seed.ToBeFlushed;
            Errors = new string[] { };
            Request = null;
        }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        [DataMember]
        public RequestBase Request { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="Result" /> class.
        /// </summary>
        protected DomainObject()
        {
            Icon = Properties.Resources.Unknown;
            Id = GuidUtils.NewCleanGuid;
        }

        #region Implementation of IDataErrorInfo

        /// <summary>
        /// Gets the error for property called [columnName]
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }


        /// <summary>
        /// if Null no errors, empty strings means there are errors but no description
        /// </summary>
        /// <value>The error.</value>

        public string Error
        {
            get
            {
                var errorMessages = Validate();
                return errorMessages.Count == 0 ? null : string.Join(Environment.NewLine, errorMessages.ToArray());
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is unknown.
        /// </summary>
        /// <value><c>true</c> if this instance is unknown; otherwise, <c>false</c>.</value>
        public bool IsUnknown
        {
            get 
            { 
                return (Id == GuidUtils.NullGuid);
            }
        }



        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected abstract string Validate(string columnName);

        /// <summary>
        /// Implement this method using
        /// DataErrorInfoValidator;
        /// </summary>
        /// <returns>ICollection{System.String}.</returns>
        protected abstract ICollection<string> Validate();

        #endregion

        #region IDirty Implementation

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool IsDirty { get; set; }

        /// <summary>
        /// Clears the is dirty.
        /// </summary>
        public void ClearIsDirty()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Sets the is dirty.
        /// </summary>
        public void SetIsDirty()
        {
            IsDirty = true;
        }

        #endregion


        /// <summary>
        /// The _parent identifier
        /// </summary>
        private string _parentId;

        /// <summary>
        /// The _image
        /// </summary>
        private byte[] _image = {};




        /// <summary>
        /// The _display name
        /// </summary>
        private string _displayName;
        /// <summary>
        /// The _icon
        /// </summary>
        private Icon _icon;

        private string _additionalInfoUri;


        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        [DataMember]
        public string ParentId
        {
            get { return _parentId; }
            set
            {
                if (_parentId == value) return;
                _parentId = String.IsNullOrEmpty(value) ? SysConst.NullGuid : value;
                RaisePropertyChanged("ParentId");
            }
        }





        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <returns>System.Object.</returns>
        public object GetValue(string propName)
        {
            var prop = GetType().GetProperty(propName);
            return prop != null ? prop.GetValue(this) : null;
        }


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string propName, object value)
        {
            var prop = GetType().GetProperty(propName);
            if (prop == null) return;
            prop.SetValue(this, value);
            if (value == this) return;
            var obj = value as IDomainObject;
            if (obj != null)
                obj.Owner = this;
        }


        public ImageSource Glyph
        { get { return GetGlyph(); } }

        public ImageSource IconSource 
        {
            get { return Icon.ToImageSource(); }
        }

        protected virtual ImageSource GetGlyph()
        {
            return Image.ToImageSource();
        }

    }
}
