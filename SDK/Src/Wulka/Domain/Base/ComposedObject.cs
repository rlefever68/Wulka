// ***********************************************************************
// Assembly         : CC4ID.Tema.CalculateCost.Contract
// Author           : Rafael Lefever
// Created          : 07-18-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 09-13-2014
// ***********************************************************************
// <copyright file="ComposedTaxonomyObject.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
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
        /// Gets the display name of the add branch command.
        /// </summary>
        /// <value>The display name of the add branch command.</value>
        public string AddBranchCommandDisplayName 
        {
            get
            {
                var res = CreateBranch();
                return res == null 
                    ? String.Empty 
                    : res.DisplayName;
            }
        }



        


        
        /// <summary>
        /// Gets the display name of the add folder command.
        /// </summary>
        /// <value>The display name of the add folder command.</value>
        public string AddFolderCommandDisplayName 
        {
            get
            {
                var res = CreateFolder();
                return res == null
                    ? String.Empty
                    : res.DisplayName;
            }
        }

        /// <summary>
        /// Gets the display name of the add child command.
        /// </summary>
        /// <value>The display name of the add child command.</value>
        public string AddChildCommandDisplayName 
        {
            get
            {
                var res = CreateChild();
                return res == null
                    ? String.Empty
                    : res.DisplayName;
            }
        }

        public ImageSource AddBranchCommandIcon {
            get {
                var res = CreateBranch();
                return res == null
                    ? null
                    : res.IconSource;
            }
        }

        public ImageSource AddFolderCommandIcon {
            get {
                var res = CreateFolder();
                return res == null
                    ? null
                    : res.IconSource;
            }
        }



        public ImageSource AddChildCommandIcon {
            get {
                var res = CreateChild();
                return res == null
                    ? null
                    : res.IconSource;
            }
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




        public bool AddBranchVisible
        {
            get { return CanAddBranch(); }
        }

        /// <summary>
        /// Determines whether this instance [can add branch].
        /// </summary>
        /// <returns><c>true</c> if this instance [can add branch]; otherwise, <c>false</c>.</returns>
        public bool CanAddBranch()
        {
            var res = CreateBranch();
            return res != null;
        }

        /// <summary>
        /// Determines whether this instance [can add child].
        /// </summary>
        /// <returns><c>true</c> if this instance [can add child]; otherwise, <c>false</c>.</returns>
        public bool CanAddChild()
        {
            var res = CreateChild();
            return res != null;
        }

        /// <summary>
        /// Determines whether this instance [can add folder].
        /// </summary>
        /// <returns><c>true</c> if this instance [can add folder]; otherwise, <c>false</c>.</returns>
        public bool CanAddFolder()
        {
            var res = CreateFolder();
            return res != null;
        }

        public bool AddChildVisible 
        {
            get { return CanAddChild(); }
        }
        public bool AddFolderVisible 
        {
            get { return CanAddFolder(); }
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
            get 
            { 
                return _selectedItem; 
            }
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
