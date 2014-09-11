// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-19-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-19-2014
// ***********************************************************************
// <copyright file="TaxonomyObject.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;
using Wulka.Factories;
using Wulka.Utils;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class TaxonomyObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract partial class TaxonomyObject<T> : ComposedObject<T>, ITaxonomyObject
        where T:IDomainObject
    {


        /// <summary>
        /// Gets or sets the hook.
        /// </summary>
        /// <value>The hook.</value>
        public IHook Hook
        {
            get { return _hook; }
            set { _hook=value; RaisePropertyChanged("Hook"); }
        }


        /// <summary>
        /// The TaxoFactory
        /// </summary>
        [Import(typeof(ITaxoFactory))]
        protected ITaxoFactory TaxoFactory;



        /// <summary>
        /// Gets the hook asynchronous.
        /// </summary>
        public void GetHookAsync()
        {
            if (TaxoFactory == null)
                CompositionHelper.ComposeParts(this, GetTaxoFactoryType());
            if (TaxoFactory != null)
                TaxoFactory
                    .TaxoProxy
                    .GetHookAsync(this, (h) => { Hook = h; });
        }

       

        /// <summary>
        /// Gets the type of the taxo factory.
        /// </summary>
        /// <returns>Type.</returns>
        protected virtual Type GetTaxoFactoryType()
        {
            return GetType();
        }


        /// <summary>
        /// The _taxonomy hook
        /// </summary>
        private string _hookId;

        /// <summary>
        /// The _hook
        /// </summary>
        private IHook _hook;

        /// <summary>
        /// Gets or sets the taxonomy hook.
        /// </summary>
        /// <value>The taxonomy hook.</value>
        [DataMember]
        public string HookId
        {
            get
            {
                return _hookId;
            }
            set
            {
                if (_hookId == value) return;
                _hookId = value;
                RaisePropertyChanged("HookId");
                GetHookAsync();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TaxonomyObject{T}"/> class.
        /// </summary>
        protected TaxonomyObject()
        {
            MasterDocId = MasterDoc.Id;
        }



    }
}
