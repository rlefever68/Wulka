// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-20-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-24-2014
// ***********************************************************************
// <copyright file="IBigDDatabase.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#if XAMARIN
#else
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Lucene;
using Wulka.Interfaces;

namespace Wulka.BigD.Contract.Interfaces
{
    /// <summary>
    /// Interface IBigDDatabase
    /// </summary>
	public interface IBigDDatabase
	{
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
		string Name { get; set; }
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
		IBigDServer Server { get; set; }
        /// <summary>
        /// Copies the specified source document identifier.
        /// </summary>
        /// <param name="sourceDocumentId">The source document identifier.</param>
        /// <param name="destinationDocumentId">The destination document identifier.</param>
		void Copy(string sourceDocumentId, string destinationDocumentId);
        /// <summary>
        /// Copies the specified source document identifier.
        /// </summary>
        /// <param name="sourceDocumentId">The source document identifier.</param>
        /// <param name="destinationDocumentId">The destination document identifier.</param>
        /// <param name="destinationRev">The destination rev.</param>
		void Copy(string sourceDocumentId, string destinationDocumentId, string destinationRev);
        /// <summary>
        /// Copies the specified source document.
        /// </summary>
        /// <param name="sourceDocument">The source document.</param>
        /// <param name="destinationDocument">The destination document.</param>
		void Copy(IBigDbDocument sourceDocument, IBigDbDocument destinationDocument);
        /// <summary>
        /// Counts the documents.
        /// </summary>
        /// <returns>System.Int32.</returns>
		int CountDocuments();
        /// <summary>
        /// Creates this instance.
        /// </summary>
		void Create();
        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument CreateDocument(IBigDbDocument document);
        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>BigDJsonDocument.</returns>
		BigDJsonDocument CreateDocument(string json);
        /// <summary>
        /// Deletes this instance.
        /// </summary>
		void Delete();
        /// <summary>
        /// Deletes the attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rev">The rev.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
		void DeleteAttachment(string id, string rev, string attachmentName);
        /// <summary>
        /// Deletes the attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument DeleteAttachment(IBigDbDocument document, string attachmentName);
        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rev">The rev.</param>
		void DeleteDocument(string id, string rev);
        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="document">The document.</param>
		void DeleteDocument(IBigDbDocument document);
        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
		void DeleteDocuments(string startKey, string endKey);
        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
		void DeleteDocuments(IEnumerable<IBigDbDocument> documents);
        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="bulk">The bulk.</param>
		void DeleteDocuments(ICanJson bulk);
        /// <summary>
        /// Existses this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		bool Exists();
        /// <summary>
        /// Fetches the document if changed.
        /// </summary>
        /// <param name="document">The document.</param>
		void FetchDocumentIfChanged(IBigDbDocument document);
        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>IEnumerable&lt;BigDJsonDocument&gt;.</returns>
		IEnumerable<BigDJsonDocument> GetAllDocuments();
        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
		IEnumerable<T> GetAllDocuments<T>() where T : IBigDbDocument, new();
        /// <summary>
        /// Gets the content of all documents without.
        /// </summary>
        /// <returns>IEnumerable&lt;BigDDocument&gt;.</returns>
		IEnumerable<BigDDocument> GetAllDocumentsWithoutContent();
        /// <summary>
        /// Gets the arbitrary document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="ctor">The ctor.</param>
        /// <returns>T.</returns>
		T GetArbitraryDocument<T>(string documentId, Func<T> ctor);
        /// <summary>
        /// Gets the arbitrary documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentIds">The document ids.</param>
        /// <param name="ctor">The ctor.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
		IEnumerable<T> GetArbitraryDocuments<T>(IEnumerable<string> documentIds, Func<T> ctor);
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>T.</returns>
		T GetDocument<T>(string documentId) where T : IBigDbDocument, new();
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>BigDJsonDocument.</returns>
		BigDJsonDocument GetDocument(string documentId);
        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <param name="documentIds">The document ids.</param>
        /// <returns>IEnumerable&lt;BigDJsonDocument&gt;.</returns>
		IEnumerable<BigDJsonDocument> GetDocuments(IEnumerable<string> documentIds);
        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentIds">The document ids.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
		IEnumerable<T> GetDocuments<T>(IEnumerable<string> documentIds) where T : IBigDbDocument, new();
        /// <summary>
        /// Determines whether the specified document identifier has attachment.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns><c>true</c> if the specified document identifier has attachment; otherwise, <c>false</c>.</returns>
		bool HasAttachment(string documentId, string attachmentName);
        /// <summary>
        /// Determines whether the specified document has attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns><c>true</c> if the specified document has attachment; otherwise, <c>false</c>.</returns>
		bool HasAttachment(IBigDbDocument document, string attachmentName);
        /// <summary>
        /// Determines whether the specified document has document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns><c>true</c> if the specified document has document; otherwise, <c>false</c>.</returns>
		bool HasDocument(IBigDbDocument document);
        /// <summary>
        /// Determines whether the specified document identifier has document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns><c>true</c> if the specified document identifier has document; otherwise, <c>false</c>.</returns>
		bool HasDocument(string documentId);
        /// <summary>
        /// Determines whether [has document changed] [the specified document identifier].
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="rev">The rev.</param>
        /// <returns><c>true</c> if [has document changed] [the specified document identifier]; otherwise, <c>false</c>.</returns>
		bool HasDocumentChanged(string documentId, string rev);
        /// <summary>
        /// Determines whether [has document changed] [the specified document].
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns><c>true</c> if [has document changed] [the specified document]; otherwise, <c>false</c>.</returns>
		bool HasDocumentChanged(IBigDbDocument document);
        /// <summary>
        /// Initializes this instance.
        /// </summary>
		void Initialize();
        /// <summary>
        /// News the design document.
        /// </summary>
        /// <param name="aName">A name.</param>
        /// <returns>BigDDesignDocument.</returns>
		BigDDesignDocument NewDesignDocument(string aName);
        /// <summary>
        /// News the temporary view.
        /// </summary>
        /// <param name="designDoc">The design document.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="mapText">The map text.</param>
        /// <returns>IBigDViewDefinition.</returns>
		IBigDViewDefinition NewTempView(string designDoc, string viewName, string mapText);
        /// <summary>
        /// Queries the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>BigDQuery.</returns>
		BigDQuery Query(IBigDViewDefinition view);
        /// <summary>
        /// Queries the specified design name.
        /// </summary>
        /// <param name="designName">Name of the design.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>BigDQuery.</returns>
		BigDQuery Query(string designName, string viewName);
		#if XAMARIN
		#else
        /// <summary>
        /// Queries the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>BigDLuceneQuery.</returns>
		BigDLuceneQuery Query(BigDLuceneViewDefinition view);
		#endif
        /// <summary>
        /// Queries all documents.
        /// </summary>
        /// <returns>BigDQuery.</returns>
		BigDQuery QueryAllDocuments();
        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns>WebResponse.</returns>
		WebResponse ReadAttachment(string documentId, string attachmentName);
        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns>WebResponse.</returns>
		WebResponse ReadAttachment(IBigDbDocument document, string attachmentName);
        /// <summary>
        /// Reads the document.
        /// </summary>
        /// <param name="document">The document.</param>
		void ReadDocument(IBigDbDocument document);
        /// <summary>
        /// Reads the document j object.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>JObject.</returns>
		JObject ReadDocumentJObject(string documentId);
        /// <summary>
        /// Reads the document if changed.
        /// </summary>
        /// <param name="document">The document.</param>
		void ReadDocumentIfChanged(IBigDbDocument document);
        /// <summary>
        /// Reads the document string.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>System.String.</returns>
		string ReadDocumentString(string documentId);
        /// <summary>
        /// Requests the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IBigDRequest.</returns>
		IBigDRequest Request(string path);
        /// <summary>
        /// Requests this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
		IBigDRequest Request();
        /// <summary>
        /// Requests all documents.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
		IBigDRequest RequestAllDocuments();
        /// <summary>
        /// Runnings the on mono.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		bool RunningOnMono();
        /// <summary>
        /// Saves the arbitrary document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        /// <returns>T.</returns>
		T SaveArbitraryDocument<T>(T document);
        /// <summary>
        /// Saves the arbitrary documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents">The documents.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveArbitraryDocuments<T>(IEnumerable<T> documents, bool allOrNothing);
        /// <summary>
        /// Saves the arbitrary documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents">The documents.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveArbitraryDocuments<T>(IEnumerable<T> documents, int chunkCount, bool allOrNothing);
        /// <summary>
        /// Saves the arbitrary documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents">The documents.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="views">The views.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveArbitraryDocuments<T>(IEnumerable<T> documents, int chunkCount, IEnumerable<IBigDViewDefinition> views, bool allOrNothing);
        /// <summary>
        /// Saves the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument SaveDocument(IBigDbDocument document);
        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="views">The views.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveDocuments(IEnumerable<IBigDbDocument> documents, int chunkCount, IEnumerable<IBigDViewDefinition> views, bool allOrNothing);
        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveDocuments(IEnumerable<IBigDbDocument> documents, bool allOrNothing);
        /// <summary>
        /// Saves the documents.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		void SaveDocuments(IEnumerable<IBigDbDocument> documents, int chunkCount, bool allOrNothing);
        /// <summary>
        /// Synches the design documents.
        /// </summary>
		void SynchDesignDocuments();
        /// <summary>
        /// Touches the view.
        /// </summary>
        /// <param name="designDocumentId">The design document identifier.</param>
        /// <param name="viewName">Name of the view.</param>
		void TouchView(string designDocumentId, string viewName);
        /// <summary>
        /// Touches the views.
        /// </summary>
        /// <param name="views">The views.</param>
		void TouchViews(IEnumerable<IBigDViewDefinition> views);
        /// <summary>
        /// Writes the attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <param name="attachmentData">The attachment data.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, Stream attachmentData, string mimeType);
        /// <summary>
        /// Writes the attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <param name="attachmentData">The attachment data.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, byte[] attachmentData, string mimeType);
        /// <summary>
        /// Writes the attachment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <param name="attachmentData">The attachment data.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, string attachmentData, string mimeType);
        /// <summary>
        /// Writes the document.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteDocument(string json, string documentId);
        /// <summary>
        /// Writes the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteDocument(IBigDbDocument document);
        /// <summary>
        /// Writes the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="batch">if set to <c>true</c> [batch].</param>
        /// <returns>IBigDbDocument.</returns>
		IBigDbDocument WriteDocument(IBigDbDocument document, bool batch);
	}
}