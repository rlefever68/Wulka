// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-11-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-11-2013
// ***********************************************************************
// <copyright file="TypeFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Configuration;
using Wulka.Logging;

namespace Wulka.Utils
{
    /// <summary>
    /// Helper class to create Type instances
    /// </summary>
    public class TypeFactory
    {

        /// <summary>
        /// Creates a Type instance from information contained in the appSettings of the configuration file.
        /// </summary>
        /// <param name="appSettingsKey">The key in the app settings where the Type information is stored.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static Type CreateTypeFromConfiguration(string appSettingsKey)
        {
            string typeName;
            if (GetTypeInfo(appSettingsKey, out typeName))
            {
                Type type;
                try
                {
                    type = Type.GetType(typeName, throwOnError: true /* throw error if not found */);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "Failed to create Type object from information in configuration file (AppSettings key {0})"
                        + ", typeName = {1}",
                        appSettingsKey, typeName);
                    throw new ApplicationException(errorMessage, ex);
                }
                FxLog<TypeFactory>.DebugFormat("Created Type object from information in configuration file (AppSettings key {0})"
                         + ", typeName = {1}",
                    appSettingsKey, typeName);
                return type;
            }
            return null;
        }

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <param name="appSettingsKey">The application settings key.</param>
        /// <param name="instanceTypeName">Name of the instance type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool GetTypeInfo(string appSettingsKey, out string instanceTypeName)
        {
            instanceTypeName = ConfigurationManager.AppSettings[appSettingsKey];
            if (string.IsNullOrEmpty(instanceTypeName))
            {
                FxLog<TypeFactory>.DebugFormat(
                    "No Type information found in configuration file for AppSettings key {0})",
                    appSettingsKey);
                return false;
            }
            return true;
        }
    }

    //public class FactoryCreator
    //{
    //    private static readonly ILog Logger = LogManager.GetLogger(typeof(FactoryCreator));

    //    public static IFactory CreateFactoryFromConfiguration(string appSettingsKey)
    //    {
    //        IFactory factory;

    //        string factoryTypeName;
    //        string factoryKeyValue;
    //        if (GetFactoryInfo(appSettingsKey, out factoryTypeName, out factoryKeyValue))
    //        {
    //            try
    //            {
    //                Type bindingType = Type.GetType(factoryTypeName, true /* throw error if not found */);
    //                factory = (IFactory)Activator.CreateInstance(bindingType, new object[] { factoryKeyValue });
    //            }
    //            catch (Exception ex)
    //            {
    //                string errorMessage = string.Format(
    //                    "Failed to create factory from information in configuration file (AppSettings key {0})"
    //                    + ", factoryTypeName = {1}, factoryKeyValue = {2}",
    //                    appSettingsKey, factoryTypeName, factoryKeyValue);
    //                throw new ApplicationException(errorMessage, ex);
    //            }
    //            Logger.DebugFormat("Created factory (AppSettings key {0})"
    //                     + ", factoryTypeName = {1}, factoryKeyValue = {2}",
    //                appSettingsKey, factoryTypeName, factoryKeyValue);
    //            return factory;
    //        }
    //        return null;
    //    }

    //    private static bool GetFactoryInfo(string appSettingsKey,
    //        out string factoryTypeName, out string factoryKeyValue)
    //    {
    //        factoryTypeName = null;
    //        factoryKeyValue = null;

    //        string factoryInfo = ConfigurationManager.AppSettings[appSettingsKey];
    //        if (string.IsNullOrEmpty(factoryInfo))
    //        {
    //            Logger.DebugFormat(
    //                "No factory info found in configuration file for AppSettings key {0})",
    //                appSettingsKey);
    //            return false;
    //        }
    //        string[] factoryInfos = factoryInfo.Split(new[] { ';' });
    //        if (factoryInfos.Length != 2)
    //        {
    //            string errorMessage = string.Format(
    //                "Unexpected factory info found in configuration file (AppSettings key {0} = '{1}')"
    //                + ", please provide a value with format '<FactoryTypeName>;<FactoryKeyValue>'"
    //                + " where <FactoryTypeName> is an assembly qualified name formatted as <FullTypeName>[,<AssemblyDisplayName>]"
    //                + " and <FactoryKeyValue> is the key value",
    //                appSettingsKey, factoryInfo);
    //            throw new ApplicationException(errorMessage);
    //        }
    //        factoryTypeName = factoryInfos[0];
    //        factoryKeyValue = factoryInfos[1];
    //        return true;
    //    }
    //}
}
