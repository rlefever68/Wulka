// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-17-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-17-2014
// ***********************************************************************
// <copyright file="DomainSerializer.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Wulka.Utils
{
    /// <summary>
    /// Class DomainSerializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainSerializer<T> 
        where T:class
    {


        /// <summary>
        /// Serializes the json.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>System.String.</returns>
        public static string SerializeJson(T info)
        {
            return JsonConvert.SerializeObject(info, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
        }

        /// <summary>
        /// Serializes the json.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>System.String.</returns>
        public static string SerializeJson(T[] info)
        {
            return JsonConvert.SerializeObject(info,new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects});
        }


        /// <summary>
        /// Deserializes the jsons.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns>IEnumerable{`0}.</returns>
        public static IEnumerable<T> DeserializeJsons(string sIn)
        {
            return JsonConvert.DeserializeObject<List<T>>(sIn, new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects});
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns>`0.</returns>
        public static T DeserializeJson(string sIn)
        {
            return JsonConvert.DeserializeObject<T>(sIn, new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects});
        }


        /// <summary>
        /// Deserializes the jsons.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns>IEnumerable{`0}.</returns>
        public static IEnumerable<T> DeserializeJsons(Stream sIn)
        {
            var s = sIn.AsString();
            return DeserializeJsons(s);
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns>`0.</returns>
        public static T DeserializeJson(Stream sIn)
        {
            var s = sIn.AsString();
            return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
        }



        public static void SaveToJsonFile(string filename, T info)
        {
            var s = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            File.WriteAllText(filename,s);
        }

        public static T LoadFromJsonFile(string filename)
        {
            var s = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
        }













        /// <summary>
        /// Serializes the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="path">The path.</param>
        public static void Serialize(T info, string path)
        {
            if (path == null) return;
            var ser = new XmlSerializer(typeof(T));
            using (var w = XmlWriter.Create(path))
            {
                try
                {
                    ser.Serialize(w, info);
                }
                finally
                {
                    w.Close();
                }
            }
        }

        /// <summary>
        /// Serializes the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="path">The path.</param>
        public static void SerializeDataContract(T info, string path)
        {
            if (path == null) return;
            var ser = new DataContractSerializer(typeof(T));
            using (var w = XmlWriter.Create(path))
            {
                try
                {
                    ser.WriteObject(w,info);
                }
                finally
                {
                    w.Close();
                }
            }
        }



        /// <summary>
        /// Serializes the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="element">The element.</param>
        public static void Serialize(T info, XElement element)
        {
            var sr = new XmlSerializer(typeof(T));
            var wrt = element.CreateWriter();
            try
            {
                sr.Serialize(wrt, info);
            }
            finally
            {
                wrt.Close();
            }
        }


        /// <summary>
        /// Deserializes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>`0.</returns>
        public static T Deserialize(XElement element)
        {
            var ser = new XmlSerializer(typeof(T));
            XmlReader rd = element.CreateReader();
            try
            {
                return (T)ser.Deserialize(rd);
            }
            finally
            {
                rd.Close();
            }
           
        }






        /// <summary>
        /// Deserializes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>`0.</returns>
        public static T Deserialize(string path)
        {
            var ser = new XmlSerializer(typeof (T));
            if (path == null) return null;
            using (var r = XmlReader.Create(path))
            {
                try
                {
                    return (T)ser.Deserialize(r);
                }
                finally
                {
                    r.Close();
                }
            }
        }

    }

}
