// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 08-05-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-13-2014
// ***********************************************************************
// <copyright file="ResultExtensions.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wulka.Domain.Base;
using Wulka.Interfaces;
using Wulka.Utils;

namespace Wulka.Extensions
{
    /// <summary>
    /// Class ResultExtensions.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Zips the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>System.String.</returns>
        public static IEnumerable<string> Zip(this IEnumerable<IResult> result)
        {
            return result.Select(x => x.Zip());
        }

        /// <summary>
        /// Zips the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>System.String.</returns>
        public static string Zip(this IResult result)
        {
            var s = result.ToFlatJson();
            return s.Compress();
        }


        /// <summary>
        /// Unzips the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>T.</returns>
        public static T Unzip<T>(this string input)
            where T : Result
        {
            var s = input.Decompress();
            return DomainSerializer<T>.DeserializeJson(s);
        }

        /// <summary>
        /// Unzips the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>T.</returns>
        public static IEnumerable<T> Unzip<T>(this string[] input)
            where T : Result
        {
            return input.Select(x => x.Unzip<T>());
        }




        /// <summary>
        /// To the flat json.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>System.String.</returns>
        public static string ToFlatJson(this IResult result)
        {
            return JsonConvert.SerializeObject(result, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
        }



        /// <summary>
        /// Clones the specified seed.
        /// the Clone will be identical to the seed, except for the Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seed">The seed.</param>
        /// <returns>IResult.</returns>
        public static IResult Clone<T>(this IResult seed)
            where T:Result
        {
            var me = seed.Zip();
            var res = me.Unzip<T>();
            res.Id = GuidUtils.NewCleanGuid;
            return res;
        }


    }
}
