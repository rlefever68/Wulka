// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-20-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-20-2013
// ***********************************************************************
// <copyright file="ProviderWorkerContainer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Wulka.BigD;
using Wulka.Domain.Interfaces;
using Wulka.Interfaces;

namespace Wulka.Data
{
    /// <summary>
    /// Class ProviderWorkerContainer.
    /// </summary>
    public class ProviderWorkerContainer<T>
        where T:IDomainObject, new()
    {

        private static ProviderWorkerContainer<T> _instance;

        public static ProviderWorkerContainer<T> Instance 
        {
            get { return _instance ?? (_instance = new ProviderWorkerContainer<T>()); }
        }

        
        private IProvider<T> _worker;

        /// <summary>
        /// Gets the worker.
        /// </summary>
        /// <value>The worker.</value>
        public IProvider<T> Worker
        {
            get { return _worker ?? (_worker = new BigDWorker<T>()); }
        }




    }
}