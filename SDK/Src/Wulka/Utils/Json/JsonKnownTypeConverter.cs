// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-20-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-20-2014
// ***********************************************************************
// <copyright file="JsonKnownTypeConverter.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wulka.Utils.Json
{
    /// <summary>
    /// Use KnownType Attribute to match a divierd class based on the class given to the serilaizer
    /// Selected class will be the first class to match all properties in the json object.
    /// </summary>
    public class JsonKnownTypeConverter : JsonConverter
    {
        /// <summary>
        /// Gets or sets the known types.
        /// </summary>
        /// <value>The known types.</value>
        public IEnumerable<Type> KnownTypes { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonKnownTypeConverter"/> class.
        /// </summary>
        /// <param name="knownTypes">The known types.</param>
        public JsonKnownTypeConverter(IEnumerable<Type> knownTypes)
        {
            KnownTypes = knownTypes;
        }

        /// <summary>
        /// Creates the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="jObject">The j object.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.InvalidOperationException">No supported type</exception>
        protected object Create(Type objectType, JObject jObject)
        {
            if (jObject["$type"] != null)
            {
                string typeName = jObject["$type"].ToString();
                return Activator.CreateInstance(KnownTypes.First(x => typeName.Contains("." + x.Name + ",")));
            }
            throw new InvalidOperationException("No supported type");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            KnownTypes = objectType.KnownTypes();

            if (KnownTypes == null)
                return false;
            var res = (objectType.IsInterface || objectType.IsAbstract) && KnownTypes.Any(objectType.IsAssignableFrom);
            return res;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);
            // Create target object based on JObject
            var target = Create(objectType, jObject);
            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
