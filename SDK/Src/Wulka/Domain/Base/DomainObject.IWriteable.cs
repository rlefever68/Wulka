// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-09-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-11-2014
// ***********************************************************************
// <copyright file="DomainObject.IWriteable.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using Wulka.Data;
using Wulka.Exceptions;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class DomainObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class DomainObject<T> 
    {

        /// <summary>
        /// The dehydrator
        /// </summary>
        [Import(typeof(IWriteEvents))]
        protected IWriteEvents WriteTrigger;

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void OnSaved() 
        {
            ComposeParts();
            if (WriteTrigger == null) return;
            try
            {
                WriteTrigger.OnSaved(this);
            }
            catch (Exception exception)
            {
                AddError(exception.GetCombinedMessages());

                Logger.Error("Error in OnSaved() on [{0}]", Id);
                Logger.Error(exception.GetCombinedMessages());
            }
        }

        /// <summary>
        /// Called when [saving].
        /// </summary>
        public bool OnSaving()
        {
            try
            {
                MasterDocId = MasterDoc.Id;
                ComposeParts();
                return WriteTrigger == null || WriteTrigger.OnSaving(this);
            }
            catch (Exception exception)
            {
                AddError(exception.GetCombinedMessages());
                Logger.Error("Error in OnSaving() on [{0}]", Id);
                Logger.Error(exception.GetCombinedMessages());
                return false;
            }
        }


        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public bool OnDeleting()
        {
            ComposeParts();
            if (WriteTrigger == null) return true;
            try
            {
                return WriteTrigger.OnDeleting(this);
            }
            catch (Exception exception)
            {
                AddError(exception.GetCombinedMessages());
                Logger.Error("Error in OnDeleting() on [{0}]", Id);
                Logger.Error(exception.GetCombinedMessages());
                return false;
            }
        }

        /// <summary>
        /// Called when [deleted].
        /// </summary>
        public void OnDeleted()
        {
            ComposeParts();
            if (WriteTrigger == null) return;
            try
            {
                WriteTrigger.OnDeleted(this);
            }
            catch (Exception exception)
            {
                AddError(exception.GetCombinedMessages());
                Logger.Error("Error in OnDeleted() on [{0}]", Id);
                Logger.Error(exception.GetCombinedMessages());
            }
        }

        public string MasterDocId { get; set; }
    }
}
