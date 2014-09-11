// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="DomainObject.Composition.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Wulka.Configuration;
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
        /// The _hydration container
        /// </summary>
        private CompositionContainer _composer;

        /// <summary>
        /// The _source type
        /// </summary>
        private Type _sourceType;

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <returns>Type.</returns>
        protected virtual Type GetSourceType()
        {
            return typeof(T);
        }



        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        public Type SourceType
        {
            get 
            {
                _sourceType = GetSourceType();
                return _sourceType; 
            }
            set 
            { 
                _sourceType = value; 
                ComposeParts();
            }
        }


        /// <summary>
        /// Loads the hydrator.
        /// </summary>
        protected void ComposeParts()
        {
            if(ConfigurationHelper.LogComposition) 
                Logger.Info("Composing Parts...");
            var catalog = new AggregateCatalog();
            var assembly = SourceType.Assembly;
            if (ConfigurationHelper.LogComposition)
                Logger.Info("Checking assembly [{0}]", assembly.FullName);
            catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            _composer = new CompositionContainer(catalog);
            try
            {
                _composer.ComposeParts(this);
            }
            catch (CompositionException exception)
            {
                Logger.Info("No Hydrator or WriteEvents could be injected. Set Composition.Logging=true for more detailed information.");
                if (ConfigurationHelper.LogComposition)
                {
                    Logger.Info("---> Error Composing Parts <----");
                    Logger.Info(exception.GetCombinedMessages());
                }
            }
        }
    }
}
