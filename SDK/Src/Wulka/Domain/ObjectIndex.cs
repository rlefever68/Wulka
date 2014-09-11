// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-22-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-22-2014
// ***********************************************************************
// <copyright file="ObjectIndex.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain
{

   

    /// <summary>
    /// Class ObjectIndex. This class cannot be inherited.
    /// </summary>
    public sealed class ObjectIndex :IIndex
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static volatile ObjectIndex instance;
        /// <summary>
        /// The synchronize root
        /// </summary>
        private static readonly object syncRoot = new Object();

        /// <summary>
        /// Prevents a default instance of the <see cref="ObjectIndex"/> class from being created.
        /// </summary>
        private ObjectIndex() 
        { 

        }

        ///// <summary>
        ///// Gets the index.
        ///// </summary>
        ///// <value>The index.</value>
        //public IEnumerable<IDomainObject> Index
        //{
        //    get { return _objects; }
        //}

        /// <summary>
        /// The _objects
        /// </summary>
        private readonly List<IDomainObject> _objects = new List<IDomainObject>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static IIndex Instance
        {
            get
            {
                if (instance != null) return instance;
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ObjectIndex();
                }
                return instance;
            }
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetAll<T>()
        {
            lock(_objects)
            {
                return _objects.OfType<T>();
            }
        }

        public int Count<T>()
        {
            return _objects.OfType<T>().Count();
        }

        public int Count()
        {
            return _objects.Count;
        }

        public IEnumerable<IDomainObject> GetAll()
        {
            return _objects;
        }

        void IIndex.Clear()
        {
            _objects.Clear();
        }

        public T Find<T>(IDomainObject domainObject) where T : IDomainObject
        {
            return Find<T>(domainObject.Id);
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>T.</returns>
        public T Find<T>(string id)
            where T:IDomainObject
        {
            lock (_objects)
            {
                return _objects.OfType<T>().FirstOrDefault(x => x.Id == id);
            }
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDomainObject.</returns>
        public IDomainObject Find(string id)
        {
            lock(_objects)
            {
                return _objects.FirstOrDefault(x => x.Id == id);
            }
        }

        /// <summary>
        /// Finds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IDomainObject.</returns>
        public IDomainObject Find(IDomainObject item)
        {
            return Find(item.Id);
        }




        /// <summary>
        /// Registers the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Register(IDomainObject item)
        {
            AddObject(item);
            if (!(item is IComposedObject)) return;
            var obj = item as IComposedObject;
            foreach (var part in obj.Parts)
            {
                Register(part);
            }
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="item">The item.</param>
        private void AddObject(IDomainObject item)
        {
            lock(_objects)
            {
                if(Find(item)!=null)
                    _objects.Remove(item);
                _objects.Add(item);
            }
        }


        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Unregister(IDomainObject item)
        {
            lock(_objects)
            {
                if(item is IComposedObject)
                {
                    var obj = item as IComposedObject;
                    foreach (var part in obj.Parts)
                    {
                        Unregister(part);
                    }
                }
                _objects.Remove(item);
            }
        }


        public static void Clear()
        {
            Instance.Clear();
        }
    }
}
