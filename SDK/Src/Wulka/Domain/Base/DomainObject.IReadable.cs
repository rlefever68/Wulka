// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-09-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-09-2014
// ***********************************************************************
// <copyright file="DomainObject.IReadable.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
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
        /// The hydrator
        /// </summary>
        [Import(typeof(IHydrate))]
        protected IHydrate Hydrator;

        /// <summary>
        /// Hydrates this instance.
        /// </summary>
        public void Hydrate()
        {
            ComposeParts();
            if (Hydrator == null) return;
            if (!IsHydrated) return;
            try
            {
                Hydrator.Hydrate(this);
            }
            catch (Exception ex)
            {
                IsHydrated = false;
                AddErrors(ex.GetCombinedMessage());
            }
        }

        [DataMember]
        public bool IsHydrated { get; set; }
    }
}
