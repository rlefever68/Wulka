// ***********************************************************************
// Assembly         : CC4ID.Tema.CalculateCost.Contract
// Author           : Rafael Lefever
// Created          : 07-18-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 09-10-2014
// ***********************************************************************
// <copyright file="ComposedTaxonomyObject.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class ComposedTaxonomyObject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract class ComposedObject<T> : EcoObject<T>, IComposedObject
        where T : IDomainObject
    {



        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns>IParameter.</returns>
        public IParameter AddParameter(string id, object value)
        {
            return AddParameter(new Parameter() { Id = id, Value = value });
        }

        /// <summary>
        /// Adds the branch.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        public virtual IDomainObject AddBranch()
        {
            var brc = CreateBranch();
            AddPart(brc);
            return brc;
        }

        /// <summary>
        /// Creates the branch.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        protected virtual IDomainObject CreateBranch() { return null; }
        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        protected virtual IDomainObject CreateChild() { return null; }
        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        protected virtual IDomainObject CreateFolder() { return null; }



        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        public virtual IDomainObject AddChild()
        {
            var chl = CreateChild();
            AddPart(chl);
            return chl;
        }


        /// <summary>
        /// Adds the folder.
        /// </summary>
        /// <returns>IDomainObject.</returns>
        public virtual IDomainObject AddFolder()
        {
            var fld = CreateFolder();
            AddPart(fld);
            return fld;
        }




        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public IEnumerable SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; RaisePropertyChanged("SelectedItems"); }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        [Bindable(true)]
        public IDomainObject SelectedItem
        {
            get { return _selectedItem; }
            set 
            { 
                _selectedItem = value; 
                RaisePropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>IDomainObject.</returns>
        public IParameter AddParameter(IParameter param)
        {
            AddPart(param);
            return param;
        }




        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IEnumerable<IParameter> Parameters
        {
            get { return Parts.OfType<IParameter>(); }
        }



        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDomainObject.</returns>
        public IDomainObject FindById(string id)
        {
            return ObjectIndex.Instance.Find(id);
            //try
            //{
            //    if (Id == id) return this;
            //    if (!Parts.Any()) return null;
            //    var res = Parts.First(x => x.Id == id);
            //    if (res != null) return res;
            //    foreach (var part in Parts.OfType<IComposedObject>())
            //    {
            //        res = part.FindById(id);
            //        if (res != null) break;
            //    }
            //    return res;
            //}
            //catch (Exception exception)
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// The _parts
        /// </summary>
        private List<IDomainObject> _parts = new List<IDomainObject>();


        /// <summary>
        /// Initializes the fields.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        private void InitFields(StreamingContext context)
        {
            if (_parts == null) _parts = new List<IDomainObject>();
        }



        /// <summary>
        /// The _selected item
        /// </summary>
        private IDomainObject _selectedItem;
        /// <summary>
        /// The _selected items
        /// </summary>
        private IEnumerable _selectedItems;

        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>The parts.</value>
        [DataMember]
        public IDomainObject[] Parts
        {
            get
            {
                return _parts.ToArray();
            }
            set
            {
                ReplaceParts(value);
            }
        }

        /// <summary>
        /// Adds the parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        private void ReplaceParts(IEnumerable<IDomainObject> parts)
        {
            _parts.Clear();
            foreach (var part in parts)
            {
                AddPart(part);
            }
        }

        /// <summary>
        /// Adds the part.
        /// </summary>
        /// <param name="part">The container.</param>
        /// <returns>IDomainObject.</returns>
        public virtual IDomainObject AddPart(IDomainObject part)
        {
            try
            {
                RemovePart(part);
            }
            finally
            {
                _parts.Add(part);
                part.Owner = this;
            }
            return this;
        }

        /// <summary>
        /// Removes the part.
        /// </summary>
        /// <param name="part">The part.</param>
        public void RemovePart(IDomainObject part)
        {
            if (!_parts.Any()) return;
            var res = Find<T>(part);
            if (res == null) return;
            _parts.Remove(res);
            part.Owner = null;
        }




    }
}
