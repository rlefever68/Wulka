// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-16-2014
// ***********************************************************************
// <copyright file="AppContractAgent.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;
using Wulka.Domain;
using Wulka.Domain.Interfaces;
using Wulka.Exceptions;
using Wulka.Extensions;
using Wulka.Interfaces;
using Wulka.Networking.Wcf;

namespace Wulka.Agent
{
    /// <summary>
    /// Class AppContractAgent.
    /// </summary>
    class AppContractAgent : DiscoProxy<IAppContractSentry>,IAppContractAgent
    {
        public AppContractAgent(string discoUrl) 
            : base(discoUrl)
        {
        }

        /// <summary>
        /// Registers the application usage.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IAppContract.</returns>
        public IAppContract RegisterAppUsage(IAppContract item)
        {
            var clt = CreateClient();
            try
            {
                return clt.RegisterAppUsage(item.Zip())
                    .Unzip<AppContract>();
            }
            finally
            {
                CloseClient(clt);
            }
        }




        /// <summary>
        /// Registers the application usage asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="act">The act.</param>
        public void RegisterAppUsageAsync(IAppContract item, Action<IAppContract> act = null)
        {
            IAppContract res = null;
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (sender, args) =>
                {
                    try
                    {
                        res = RegisterAppUsage(item);
                    }
                    catch (Exception ex)
                    {
                        wrk.Dispose();
                        Logger.Error(ex.GetCombinedMessages());

                    }
                };
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    wrk.Dispose();
                    if (act != null)
                        act(res);
                };
                wrk.RunWorkerAsync();
            }

        }
    }
}
