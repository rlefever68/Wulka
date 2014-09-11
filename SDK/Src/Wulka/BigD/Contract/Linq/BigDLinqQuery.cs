// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="CouchLinqQuery.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Wulka.BigD.Contract.Linq
{
    /// <summary>
    /// IQueryable implementation for CouchDB Query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CouchLinqQuery<T>: IQueryable, IQueryable<T>
    {
        /// <summary>
        /// The expression
        /// </summary>
        Expression expression;
        /// <summary>
        /// The provider
        /// </summary>
        BigDQueryProvider provider;


        /// <summary>
        /// Initializes a new instance of the <see cref="CouchLinqQuery&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="provider">The provider.</param>
        public CouchLinqQuery(Expression expression, BigDQueryProvider provider)
        {
            this.expression = expression;
            this.provider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchLinqQuery&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public CouchLinqQuery(BigDQueryProvider provider)
        {
            this.expression = Expression.Constant(this);
            this.provider = provider;
        }


        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.
        /// </summary>
        /// <value>The expression.</value>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance of <see cref="T:System.Linq.IQueryable" />.
        ///   </returns>
        Expression IQueryable.Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.
        /// </summary>
        /// <value>The type of the element.</value>
        /// <returns>
        /// A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        ///   </returns>
        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value>The provider.</value>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.
        ///   </returns>
        IQueryProvider IQueryable.Provider
        {
            get { return this.provider; }
        }

        /// <summary>
        /// Enumerator facade that applies the select expression to all internal members
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        class TransformingEnumerator<TReturn> : IEnumerator<TReturn>
        {
            /// <summary>
            /// The e
            /// </summary>
            private IEnumerator e;
            /// <summary>
            /// The transformer
            /// </summary>
            private Delegate transformer;

            /// <summary>
            /// Initializes a new instance of the <see cref="TransformingEnumerator`1"/> class.
            /// </summary>
            /// <param name="e">The e.</param>
            /// <param name="transformer">The transformer.</param>
            public TransformingEnumerator(IEnumerator e, MethodCallExpression transformer)
            {
                this.e = e;

                var t = (UnaryExpression)transformer.Arguments[1];
                this.transformer = ((LambdaExpression)t.Operand).Compile();
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public TReturn Current { get { return (TReturn)transformer.DynamicInvoke(e.Current); } }
            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value>The current.</value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            object IEnumerator.Current { get { return transformer.DynamicInvoke(e.Current); } }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext() { return e.MoveNext(); }
            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset() { e.Reset(); }
        }

        /// <summary>
        /// Executes the query and returns a properly-typed IEnumerator for the result
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <returns>IEnumerator{``0}.</returns>
        protected virtual IEnumerator<TReturn> DoGetEnumerator<TReturn>()
        {
            var expVisitor = this.provider.Prepare(expression);
            var viewResult = (BigDGenericViewResult)expVisitor.Query.GetResult();

            var typeParams = new Type[] { 
                expVisitor.SelectExpression == null ?
                // no expression, everything is fine. T will match
                typeof(T) :
                // if there is a select expression, then the type of the query 
                // will be defined by the first type parameter to the first
                // method parameter in the selection expression:
                // TOurReturnValue Select(IQueriable<TOurType>)
                expVisitor
                    .SelectExpression
                    .Method
                    .GetParameters()[0]
                    .ParameterType
                    .GetGenericArguments()[0]
            };

            var dynamicResult =
                viewResult
                    .GetType()
                    .GetMethods()
                    .First(m => m.Name == "ValueDocuments" && m.IsGenericMethodDefinition)
                    .MakeGenericMethod(typeParams)
                    .Invoke(viewResult, null);

            if (expVisitor.SelectExpression == null)
                return ((IEnumerable<TReturn>)dynamicResult).GetEnumerator();

            return
                new TransformingEnumerator<TReturn>(
                    ((IEnumerable)dynamicResult).GetEnumerator(),
                    expVisitor.SelectExpression);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return DoGetEnumerator<T>();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return DoGetEnumerator<BigDDocument>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() { return this.provider.GetQueryText(this.expression); }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) { return obj != null && ToString().Equals(obj.ToString()); }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() { return ToString().GetHashCode(); }
    }
}
