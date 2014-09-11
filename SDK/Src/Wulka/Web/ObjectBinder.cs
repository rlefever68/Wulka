// ***********************************************************************
// Assembly         : Broobu.Utils
// Author           : ON8RL
// Created          : 12-12-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-13-2013
// ***********************************************************************
// <copyright file="ObjectBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Wulka.Web
{
    /// <summary>
    /// Class ObjectBinder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectBinder<T> where T : class, IEquatable<T>, new()
    {

        #region "Properties"

        /// <summary>
        /// Constant Identifier used for Session so that values are retained between Postbacks
        /// </summary>
        /// <value>The identifier.</value>
        private string ID {
            get { return typeof(T).ToString().Replace(".", "_"); }
        }

        /// <summary>
        /// Gets or sets the binder.
        /// </summary>
        /// <value>The binder.</value>
        private List<T> Binder {
            get {
                if (HttpContext.Current.Session[ID] == null) {
                    HttpContext.Current.Session[ID] = new List<T>();
                }

                return (List<T>)HttpContext.Current.Session[ID];

            }
            set { HttpContext.Current.Session[ID] = value; }
        }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBinder{T}"/> class.
        /// </summary>
        public ObjectBinder()
        {
            // Empty Constructor
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBinder{T}"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public ObjectBinder(T instance)
        {
            SetInstance(instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBinder{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public ObjectBinder(List<T> list)
        {
            SetList(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBinder{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ObjectBinder(ICollection<T> collection)
        {
            SetList(collection);
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Get the ObjectDataSource to bind to editable ASP .Net controls like Grid, Formview, etc.
        /// </summary>
        /// <returns>ObjectDataSource.</returns>
        public ObjectDataSource GetDataSource()
        {
            dynamic ods = new ObjectDataSource();
            var with1 = ods;
            with1.ID = "ods" + ID;
            with1.DataObjectTypeName = typeof(T).ToString();
            with1.TypeName = GetType().ToString();
            with1.OldValuesParameterFormatString = "original_{0}";
            with1.SelectMethod = "List";
            with1.InsertMethod = "Insert";
            with1.UpdateMethod = "Update";
            with1.DeleteMethod = "Delete";
            return ods;
        }

        /// <summary>
        /// Gets a single instance if not using a list
        /// </summary>
        /// <returns>`0.</returns>
        /// <remarks>Use this to get object when binding to FormView or other controls that do not work with a list</remarks>
        public T Instance()
        {
            return Binder.Count == 1 ? Binder[0] : null;
        }

        /// <summary>
        /// Manually Dispose the list so that List saved in session is also cleared up
        /// </summary>
        public void Clear()
        {
            Binder.Clear();
            Binder = null;
        }

        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns>List{`0}.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<T> List()
        {
            return Binder;
        }

        /// <summary>
        /// Inserts the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public void Insert(T instance)
        {
            Binder.Add(instance);
        }

        /// <summary>
        /// Updates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public void Update(T instance)
        {
            if (Binder.Count == 1) {
                // Update single instance
                Binder[0] = instance;
            } else {
                // Update item in list
                Binder.Remove(instance);
                // Works b/c T must implement IEquatable
                Binder.Add(instance);
            }
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public void Delete(T instance)
        {
            Binder.Remove(instance);
        }

        /// <summary>
        /// Sets the list.
        /// </summary>
        /// <param name="list">The list.</param>
        public void SetList(List<T> list)
        {
            Binder = list;
        }

        /// <summary>
        /// Sets the list.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void SetList(ICollection<T> collection)
        {
            Binder = (List<T>)collection;
        }

        /// <summary>
        /// Sets the instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void SetInstance(T instance)
        {
            Clear();
            Insert(instance);
        }

        #endregion

    }
}
