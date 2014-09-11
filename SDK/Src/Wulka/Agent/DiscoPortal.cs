// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-13-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-16-2014
// ***********************************************************************
// <copyright file="DiscoPortal.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Wulka.Interfaces;

namespace Wulka.Agent
{
    /// <summary>
    /// Class DiscoPortal.
    /// </summary>
    public static class DiscoPortal 
    {

        public static IDiscoAgent CreateDisco(string ecoSpaceUrl)
        {
            return new DiscoAgent(ecoSpaceUrl);
        }




        /// <summary>
        /// Gets the disco.
        /// </summary>
        /// <value>The disco.</value>
        public static IDiscoAgent Disco 
        {
            get { return new DiscoAgent(null);}
        }

        /// <summary>
        /// Gets the cloud contracts.
        /// </summary>
        /// <value>The cloud contracts.</value>
        public static ICloudContractAgent CloudContracts 
        {
            get { return new CloudContractAgent(null);}
        }


        /// <summary>
        /// Gets the application contracts.
        /// </summary>
        /// <value>The application contracts.</value>
        public static IAppContractAgent AppContracts 
        {
            get { return new AppContractAgent(null);}
        }


    }
}
