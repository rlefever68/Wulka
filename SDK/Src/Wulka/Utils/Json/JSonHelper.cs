// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="JSonHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.Interfaces;

namespace Wulka.Utils.Json
{

    /// <summary>
    /// Class JSonHelper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JSonHelper<T> 
        where T:ICanJson
    {
        /// <summary>
        /// Writes the j son.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="writer">The writer.</param>
        public static void WriteJSon(T obj, JsonWriter writer)
        {
            var ser = new JsonSerializer();
            ser.Serialize(writer, obj);
        }

        /// <summary>
        /// Reads the j son.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>`0.</returns>
        public static T ReadJSon(JObject obj)
        {
            var s = obj.ToString();
         //   return JsonConvert.DeserializeObject<T>(s, new JsonKnownTypeConverter(typeof(T).KnownTypes()));
           return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            

        }




    }

}
