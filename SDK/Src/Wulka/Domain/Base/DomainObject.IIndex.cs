// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-20-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-22-2014
// ***********************************************************************
// <copyright file="DomainObject.IIndex.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class DomainObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract partial class DomainObject<T>
    {

        private List<IDomainObject> _ownedObjects = new List<IDomainObject>(); 


        
        /// <summary>
        /// The _owner
        /// </summary>
		private IDomainObject _owner;
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
		public IDomainObject Owner
		{
			get { return _owner; }
			set
			{
				if (value == null)
				{
					
					_owner = value;
                    Unregister(this);
				}
				else
				{
					_owner = value;
					Register(this);
				}
                OnSetOwner();
			}
		}

        protected virtual void OnSetOwner()
        { }

        /// <summary>
        /// The _touched at
        /// </summary>
		private DateTime _touchedAt;




        /// <summary>
        /// Registers the specified domain object.
        /// </summary>
        /// <param name="domainObject">The domain object.</param>
		public void Register(IDomainObject domainObject=null)
		{
            if (domainObject == null)
                Register(this);
            if(domainObject!=this)
                _ownedObjects.Add(domainObject);
            Master.Register(domainObject);
		}
        /// <summary>
        /// Finds the specified domain object.
        /// </summary>
        /// <param name="domainObject">The domain object.</param>
        /// <returns>IDomainObject.</returns>
		public IDomainObject Find(IDomainObject domainObject)
		{
            return Master.Find(domainObject);
		}


        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDomainObject.</returns>
		public IDomainObject Find(string id)
		{
            return Master.Find(id);
		}


        /// <summary>
        /// Finds the specified domain object.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <param name="domainObject">The domain object.</param>
        /// <returns>T1.</returns>
		public T1 Find<T1>(IDomainObject domainObject) where T1 : IDomainObject
		{
            return Master.Find<T1>(domainObject);
		}



        /// <summary>
        /// Finds the specified domain object.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>T1.</returns>
		public T1 Find<T1>(string id) 
			where T1 : IDomainObject
		{
            return Master.Find<T1>(id);
		}

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <returns>IEnumerable&lt;T1&gt;.</returns>
	    public IEnumerable<T1> GetAll<T1>()
	    {
            return Master.GetAll<T1>();
	    }

        public int Count<T1>()
        {
            var o = this as IComposedObject;
            return o != null ? o.Parts.OfType<T1>().Count() : 0;
        }

        public int Count()
        {
            var o = this as IComposedObject;
            return o != null ? o.Parts.Count() : 0;
        }

        public IEnumerable<IDomainObject> GetAll()
        {
            var o = this as IComposedObject;
            return o != null 
                ? o.Parts 
                : new IDomainObject[] {this};
        }

        public void Clear()
        {
            var o = this as IComposedObject;
            if (o == null) return;
            foreach (var part in o.Parts)
            {
                part.Clear();
            }
            o.Parts = new IDomainObject[] { };
        }

        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="domainObject">The domain object.</param>
		public void Unregister(IDomainObject domainObject)
		{
            _ownedObjects.Remove(domainObject);
            Master.Unregister(domainObject);
		}

        public int TreeDepth 
        {
            get
            {
                return GetTreeDepth(this);
            }
        }

        private int GetTreeDepth(IDomainObject item)
        {
            _treeDepth = -1;
            do
            {
                _treeDepth++;
                item = item.Owner;
            }
            while (item != null);
            return _treeDepth;
        }

        int _treeDepth;

        /// <summary>
        /// Gets or sets the touched at.
        /// </summary>
        /// <value>The touched at.</value>
		[DataMember]
		public DateTime TouchedAt
		{
			get 
			{ 
				return _touchedAt; 
			}
			set 
			{ 
				_touchedAt = value;
                if (!(this is IComposedObject)) return;
                var obj = this as IComposedObject;
                if (obj == null) return;
                if (obj.Parts == null) return;
			    if (!obj.Parts.Any()) return;
			    foreach (var part in obj.Parts)
			    {
			        part.TouchedAt = value;
			    }
			}
		}


        public IDomainObject MasterDoc
        {
            get { return GetMasterDoc(); }
        }

        private IDomainObject GetMasterDoc()
        {
            var res = Owner;
            return res != null 
                ? res.MasterDoc 
                : this;
        }


        /// <summary>
        /// Gets the master.
        /// </summary>
        /// <value>The master.</value>
		public IIndex Master
		{
			get
			{
				return ObjectIndex.Instance;
			}
		}




	}
}
