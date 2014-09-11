// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : rlefever
// Created          : 11-22-2013
//
// Last Modified By : rlefever
// Last Modified On : 11-22-2013
// ***********************************************************************
// <copyright file="CouchDatabase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#if XAMARIN
#else
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract;
using Wulka.BigD.Contract.Interfaces;
using Wulka.BigD.Contract.Lucene;
using Wulka.Domain;
using Wulka.Interfaces;
using Wulka.Utils.Json;

namespace Wulka.BigD
{
	/// <summary>
	/// A CouchDatabase corresponds to a named CouchDB database in a specific CouchServer.
	/// This is the main API to work with CouchDB. One useful approach is to create your own subclasses
	/// for your different databases.
	/// </summary>
	public class BigDDatabase : IBigDDatabase
	{
		/// <summary>
		/// The name
		/// </summary>
		private string name;
		/// <summary>
		/// The design documents
		/// </summary>
		public readonly IList<BigDDesignDocument> DesignDocuments = new List<BigDDesignDocument>();

		/// <summary>
		/// Initializes a new instance of the <see cref="BigDDatabase"/> class.
		/// </summary>
		public BigDDatabase()
		{
			Name = "default";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BigDDatabase"/> class.
		/// </summary>
		/// <param name="server">The server.</param>
		public BigDDatabase(IBigDServer server)
			: this()
		{
			Server = server;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BigDDatabase"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="server">The server.</param>
		public BigDDatabase(string name, IBigDServer server)
		{
			Name = name;
			Server = server;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				if (Server == null)
					return name;
				return Server.DatabasePrefix + name;
			}
			set
			{
				name = value;
			}
		}

		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		/// <value>The server.</value>
		public IBigDServer Server { get; set; }

		/// <summary>
		/// Runnings the on mono.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public bool RunningOnMono()
		{
			return Server.RunningOnMono;
		}

		/// <summary>
		/// News the design document.
		/// </summary>
		/// <param name="aName">A name.</param>
		/// <returns>CouchDesignDocument.</returns>
		public BigDDesignDocument NewDesignDocument(string aName)
		{
			var newDoc = new BigDDesignDocument(aName, this);
			DesignDocuments.Add(newDoc);
			return newDoc;
		}

		/// <summary>
		/// Only to be used when developing.
		/// </summary>
		/// <param name="designDoc">The design document.</param>
		/// <param name="viewName">Name of the view.</param>
		/// <param name="mapText">The map text.</param>
		/// <returns>ICouchViewDefinition.</returns>
		public IBigDViewDefinition NewTempView(string designDoc, string viewName, string mapText)
		{
			var doc = NewDesignDocument(designDoc);
			var view = doc.AddView(viewName, "function (doc) {" + mapText + "}");
			doc.Synch();
			return view;
		}

		/// <summary>
		/// Currently the logic is that the code is always the master.
		/// And we also do not remove design documents in the database that
		/// we no longer have in code.
		/// </summary>
		public void SynchDesignDocuments()
		{
			foreach (var doc in DesignDocuments)
			{
				doc.Synch();
			}
		}

		/// <summary>
		/// Requests this instance.
		/// </summary>
		/// <returns>ICouchRequest.</returns>
		public IBigDRequest Request()
		{
			return new BigDRequest(this);
		}

		/// <summary>
		/// Requests the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>ICouchRequest.</returns>
		public IBigDRequest Request(string path)
		{
			return (new BigDRequest(this)).Path(path);
		}

		/// <summary>
		/// Counts the documents.
		/// </summary>
		/// <returns>System.Int32.</returns>
		public int CountDocuments()
		{
			return (Request().Parse())["doc_count"].Value<int>();
		}

		/// <summary>
		/// Requests all documents.
		/// </summary>
		/// <returns>ICouchRequest.</returns>
		public IBigDRequest RequestAllDocuments()
		{
			return Request("_all_docs");
		}

		/// <summary>
		/// Return all documents in the database as CouchJsonDocuments.
		/// This method is only practical for testing purposes.
		/// </summary>
		/// <returns>A list of all documents.</returns>
		public IEnumerable<BigDJsonDocument> GetAllDocuments()
		{
			return QueryAllDocuments().IncludeDocuments().GetResult().Documents<BigDJsonDocument>();
		}

		/// <summary>
		/// Return all documents in the database using a supplied
		/// document type implementing ICouchDocument.
		/// This method is only practical for testing purposes.
		/// </summary>
		/// <typeparam name="T">The document type to use.</typeparam>
		/// <returns>A list of all documents.</returns>
		public IEnumerable<T> GetAllDocuments<T>() where T : IBigDbDocument, new()
		{
			return QueryAllDocuments()
				.IncludeDocuments()
				.GetResult()
				.Documents<T>();
		}

		/// <summary>
		/// Return all documents in the database, but only with id and revision.
		/// CouchDocument does not contain the actual content.
		/// </summary>
		/// <returns>List of documents</returns>
		public IEnumerable<BigDDocument> GetAllDocumentsWithoutContent()
		{
			QueryAllDocuments().GetResult().ValueDocuments<BigDDocument>();
			var json = RequestAllDocuments().Parse();
			return (from JObject row in json["rows"] 
					select new BigDDocument(row["id"].ToString(), (row["value"])["rev"].ToString())).ToList();
		}

		/// <summary>
		/// Initialize CouchDB database by saving new or changed design documents into it.
		/// Override if needed in subclasses.
		/// </summary>
		public virtual void Initialize()
		{
			SynchDesignDocuments();
		}

		/// <summary>
		/// Existses this instance.
		/// </summary>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public bool Exists()
		{
			return Server.HasDatabase(Name);
		}

		/// <summary>
		/// Check first if database exists, and if it does not - create it and initialize it.
		/// </summary>
		public void Create()
		{
		    if (Exists()) return;
		    Server.CreateDatabase(Name);
		    Initialize();
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public void Delete()
		{
			if (Exists())
			{
				Server.DeleteDatabase(Name);
			}
		}

		/// <summary>
		/// Write a document given as plain JSON and a document id. A document may already exist in db and will then be overwritten.
		/// </summary>
		/// <param name="json">Document as a JSON string</param>
		/// <param name="documentId">Document identifier</param>
		/// <returns>A new CouchJsonDocument</returns>
		public IBigDbDocument WriteDocument(string json, string documentId)
		{
			return WriteDocument(new BigDJsonDocument(json, documentId));
		}

		/// <summary>
		/// Write a CouchDocument or ICouchDocument, it may already exist in db and will then be overwritten.
		/// </summary>
		/// <param name="document">Couch document</param>
		/// <returns>Couch Document with new Rev set.</returns>
		/// <remarks>This relies on the document to already have an id.</remarks>
		public IBigDbDocument
			WriteDocument(IBigDbDocument document)
		{
			return WriteDocument(document, false);
		}

		/// <summary>
		/// Saves the arbitrary document.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="document">The document.</param>
		/// <returns>``0.</returns>
		public T SaveArbitraryDocument<T>(T document)
		{
			return ((BigDDocumentWrapper<T>)SaveDocument(new BigDDocumentWrapper<T>(document))).Instance;
		}

		/// <summary>
		/// This is a convenience method that creates or writes a ICouchDocument depending on if
		/// it has an id or not. If it does not have an id we create the document and let CouchDB allocate
		/// an id. If it has an id we use WriteDocument which will overwrite the existing document in CouchDB.
		/// </summary>
		/// <param name="document">ICouchDocument</param>
		/// <returns>ICouchDocument with new Rev set.</returns>
		public IBigDbDocument SaveDocument(IBigDbDocument document)
		{
			var reconcilingDoc = document as IReconcilingDocument;
			IBigDbDocument savedDoc;
			try
			{
				savedDoc = document.Id == null ? 
					CreateDocument(document) : 
					WriteDocument(document);
			}
			catch (BigDConflictException)
			{
				if (reconcilingDoc == null)
					throw;

				// can't handle a brand-new document
				if (String.IsNullOrEmpty(reconcilingDoc.Rev))
					throw;
				switch (reconcilingDoc.ReconcileBy)
				{
					case ReconcileStrategy.None:
						throw;
					default:
						reconcilingDoc.Reconcile(reconcilingDoc.GetDatabaseCopy(this));
						SaveDocument(reconcilingDoc);
						break;
				}

				savedDoc = reconcilingDoc;
			}

			if (reconcilingDoc != null)
				reconcilingDoc.SaveCommited();
		//	SynchDesignDocuments();
			return savedDoc;
		}

		/// <summary>
		/// Write a CouchDocument or ICouchDocument, it may already exist in db and will then be overwritten.
		/// </summary>
		/// <param name="document">Couch document</param>
		/// <param name="batch">True if we don't want to wait for flush (commit).</param>
		/// <returns>Couch Document with new Rev set.</returns>
		/// <remarks>This relies on the document to already have an id.</remarks>
		public IBigDbDocument WriteDocument(IBigDbDocument document, bool batch)
		{
			if (document.Id == null)
			{
				throw BigDException.Create(
					"Failed to write document using PUT because it lacks an id, use CreateDocument instead to let CouchDB generate an id");
			}

			// Wulka additions

			var json = BigDDocument.WriteJson(document, false);

			// End Wulka additions


			JObject result =
				Request(document.Id).Query(batch ? "?batch=ok" : null).Data(json).Put().Check("Failed to write document").Result();
			document.Id = result["id"].Value<string>(); // Not really needed
			document.Rev = result["rev"].Value<string>();

			return document;
		}

		/// <summary>
		/// Add an attachment to an existing ICouchDocument, it may already exist in db and will then be overwritten.
		/// </summary>
		/// <param name="document">Couch document</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <param name="attachmentData">The attachment data.</param>
		/// <param name="mimeType">The MIME type for the attachment.</param>
		/// <returns>The document.</returns>
		/// <remarks>This relies on the document to already have an id.</remarks>
		public IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, string attachmentData, string mimeType)
		{
			var byteData = Encoding.UTF8.GetBytes(attachmentData);
			return WriteAttachment(document, attachmentName, byteData, mimeType);
		}

		/// <summary>
		/// Add an attachment to an existing ICouchDocument, it may already exist in db and will then be overwritten.
		/// </summary>
		/// <param name="document">Couch document</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <param name="attachmentData">The attachment data.</param>
		/// <param name="mimeType">The MIME type for the attachment.</param>
		/// <returns>The document.</returns>
		/// <remarks>This relies on the document to already have an id.</remarks>
		public IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, byte[] attachmentData, string mimeType)
		{
			if (document.Id == null)
			{
				throw BigDException.Create(
					"Failed to add attachment to document using PUT because it lacks an id");
			}

			JObject result =
				Request(document.Id + "/" + attachmentName).Query("?rev=" + document.Rev).Data(attachmentData).MimeType(mimeType).Put().Check("Failed to write attachment")
					.Result();
			document.Id = result["id"].Value<string>(); // Not really neeed
			document.Rev = result["rev"].Value<string>();

			return document;
		}

		/// <summary>
		/// Writes the attachment.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <param name="attachmentData">The attachment data.</param>
		/// <param name="mimeType">Type of the MIME.</param>
		/// <returns>The document.</returns>
		/// <remarks>This relies on the document to already have an id.</remarks>
		public IBigDbDocument WriteAttachment(IBigDbDocument document, string attachmentName, Stream attachmentData, string mimeType)
		{
			if (document.Id== null)
			{
				throw BigDException.Create(
					"Failed to add attachment to document using PUT because it lacks an id");
			}

			JObject result =
				Request(document.Id+ "/" + attachmentName).Query("?rev=" + document.Rev).Data(attachmentData).MimeType(mimeType).Put().Check("Failed to write attachment")
					.Result();
			document.Id= result["id"].Value<string>(); // Not really neeed
			document.Rev = result["rev"].Value<string>();

			return document;
		}

		/// <summary>
		/// Read a ICouchDocument with an id even if it has not changed revision.
		/// </summary>
		/// <param name="document">Document to fill.</param>
		public void ReadDocument(IBigDbDocument document)
		{
			document.ReadJson(ReadDocumentJObject(document.Id));
		}

		/// <summary>
		/// Read the attachment for an ICouchDocument.
		/// </summary>
		/// <param name="document">Document to read.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <returns>WebResponse.</returns>
		public WebResponse ReadAttachment(IBigDbDocument document, string attachmentName)
		{
			return ReadAttachment(document.Id, attachmentName);
		}

		/// <summary>
		/// First use HEAD to see if it has indeed changed.
		/// </summary>
		/// <param name="document">Document to fill.</param>
		public void FetchDocumentIfChanged(IBigDbDocument document)
		{
			if (HasDocumentChanged(document))
			{
				ReadDocument(document);
			}
		}

		/// <summary>
		/// Read a CouchDocument or ICouchDocument, this relies on the document to obviously have an id.
		/// We also check the revision so that we can avoid parsing JSON if the document is unchanged.
		/// </summary>
		/// <param name="document">Document to fill.</param>
		public void ReadDocumentIfChanged(IBigDbDocument document)
		{
			var result = Request(document.Id).Etag(document.Rev).Parse();
			if (result == null) return;
			document.ReadJson(result);
		}

		/// <summary>
		/// Read a couch document given an id, this method does not have enough information to do caching.
		/// </summary>
		/// <param name="documentId">Document identifier</param>
		/// <returns>Document Json as JObject</returns>
		public JObject ReadDocumentJObject(string documentId)
		{
			try
			{
				return Request(documentId).Parse();
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to read document", e);
			}
		}

		/// <summary>
		/// Read a couch document given an id, this method does not have enough information to do caching.
		/// </summary>
		/// <param name="documentId">Document identifier</param>
		/// <returns>Document Json as string</returns>
		public string ReadDocumentString(string documentId)
		{
			try
			{
				return Request(documentId).String();
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to read document: " + e.Message, e);
			}
		}

		/// <summary>
		/// Read a couch attachment given a document id, this method does not have enough information to do caching.
		/// </summary>
		/// <param name="documentId">Document identifier</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <returns>Document attachment</returns>
		public WebResponse ReadAttachment(string documentId, string attachmentName)
		{
			try
			{
				return Request(documentId + "/" + attachmentName).Response();
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to read document: " + e.Message, e);
			}
		}

		/// <summary>
		/// Create a CouchDocument given JSON as a string. Uses POST and CouchDB will allocate a new id.
		/// </summary>
		/// <param name="json">Json data to store.</param>
		/// <returns>Couch document with data, id and rev set.</returns>
		/// <remarks>POST which may be problematic in some environments.</remarks>
		public BigDJsonDocument CreateDocument(string json)
		{
			return (BigDJsonDocument)CreateDocument(new BigDJsonDocument(json));
		}

		/// <summary>
		/// Create a given ICouchDocument in CouchDB. Uses POST and CouchDB will allocate a new id and overwrite any existing id.
		/// </summary>
		/// <param name="document">Document to store.</param>
		/// <returns>Document with Id and Rev set.</returns>
		/// <remarks>POST which may be problematic in some environments.</remarks>
		public IBigDbDocument CreateDocument(IBigDbDocument document)
		{
			try
			{
				// Wulka mods
				var json = BigDDocument.WriteJson(document, false);
				// end Wulka mods

				JObject result = Request().Data(json).PostJson().Check("Failed to create document").Result();
				document.Id= result["id"].Value<string>();
				document.Rev = result["rev"].Value<string>();
				return document;
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to create document", e);
			}
		}

		/// <summary>
		/// Saves the arbitrary documents.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documents">The documents.</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		public void SaveArbitraryDocuments<T>(IEnumerable<T> documents, bool allOrNothing)
		{
			SaveDocuments(documents.Select(doc => new BigDDocumentWrapper<T>(doc)).Cast<IBigDbDocument>(), allOrNothing);
		}

		/// <summary>
		/// Create or update a list of ICouchDocuments in CouchDB. Uses POST and CouchDB will
		/// allocate new ids if the documents lack them.
		/// </summary>
		/// <param name="documents">List of documents to store.</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		/// <remarks>POST may be problematic in some environments.</remarks>
		public void SaveDocuments(IEnumerable<IBigDbDocument> documents, bool allOrNothing)
		{
			var bulk = new BigDBulkDocuments(documents);
			try
			{
				
				var json = BigDDocument.WriteJson(bulk, false);

				var result = Request("_bulk_docs")
					.Data(json)
					.Query("?all_or_nothing=" + allOrNothing.ToString().ToLower())
					.PostJson()
					.Parse<JArray>();

				int index = 0;
				foreach (var document in documents)
				{
					document.Id= (result[index])["id"].Value<string>();
					document.Rev = (result[index])["rev"].Value<string>();
					++index;
				}                
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to create bulk documents", e);
			}
		}

		/// <summary>
		/// Saves the arbitrary documents.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documents">The documents.</param>
		/// <param name="chunkCount">The chunk count.</param>
		/// <param name="views">The views.</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		public void SaveArbitraryDocuments<T>(IEnumerable<T> documents, int chunkCount, IEnumerable<IBigDViewDefinition> views, bool allOrNothing)
		{
			SaveDocuments(
				documents.Select(doc => new BigDDocumentWrapper<T>(doc)).Cast<IBigDbDocument>(),
				chunkCount,
				views,
				allOrNothing);
		}

		/// <summary>
		/// Create or updates documents in bulk fashion, chunk wise. Optionally access given view
		/// after each chunk to trigger reindexing.
		/// </summary>
		/// <param name="documents">List of documents to store.</param>
		/// <param name="chunkCount">Number of documents to store per "POST"</param>
		/// <param name="views">List of views to touch per chunk.</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		public void SaveDocuments(IEnumerable<IBigDbDocument> documents, int chunkCount, IEnumerable<IBigDViewDefinition> views, bool allOrNothing)
		{
			var chunk = new List<IBigDbDocument>(chunkCount);
			int counter = 0;

			foreach (IBigDbDocument doc in documents)
			{
				// Do we have a chunk ready to create?
				if (counter == chunkCount)
				{
					counter = 0;
					SaveDocuments(chunk, allOrNothing);
					TouchViews(views);
					/* Skipping separate thread for now, ASP.Net goes bonkers...
					(new Thread(
						() => GetView<CouchPermanentViewResult>(designDocumentName, viewName, ""))
					{
						Name = "View access in background", Priority = ThreadPriority.BelowNormal
					}).Start(); */

					chunk = new List<IBigDbDocument>(chunkCount);
				}
				counter++;
				chunk.Add(doc);
			}

			SaveDocuments(chunk, allOrNothing);
			TouchViews(views);
		}

		/// <summary>
		/// Touches the views.
		/// </summary>
		/// <param name="views">The views.</param>
		public void TouchViews(IEnumerable<IBigDViewDefinition> views)
		{
			//var timer = new Stopwatch();
			if (views != null)
			{
				foreach (var view in views)
				{
					if (view != null)
					{
						//timer.Reset();
						//timer.Start();
						view.Touch();
						//timer.Stop();
						//Server.Debug("Update view " + view.Path() + ":" + timer.ElapsedMilliseconds + " ms");
					}
				}
			}
		}

		/// <summary>
		/// Create documents in bulk fashion, chunk wise.
		/// </summary>
		/// <param name="documents">List of documents to store.</param>
		/// <param name="chunkCount">Number of documents to store per "POST"</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		public void SaveDocuments(IEnumerable<IBigDbDocument> documents, int chunkCount, bool allOrNothing)
		{
			SaveDocuments(documents, chunkCount, null, allOrNothing);
		}

		/// <summary>
		/// Saves the arbitrary documents.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documents">The documents.</param>
		/// <param name="chunkCount">The chunk count.</param>
		/// <param name="allOrNothing">if set to <c>true</c> [all or nothing].</param>
		public void SaveArbitraryDocuments<T>(IEnumerable<T> documents, int chunkCount, bool allOrNothing)
		{
			SaveArbitraryDocuments(documents, chunkCount, null, allOrNothing);
		}

		/// <summary>
		/// Gets the documents.
		/// </summary>
		/// <param name="documentIds">The document ids.</param>
		/// <returns>IEnumerable{CouchJsonDocument}.</returns>
		public IEnumerable<BigDJsonDocument> GetDocuments(IEnumerable<string> documentIds)
		{
			return GetDocuments<BigDJsonDocument>(documentIds);
		}

		/// <summary>
		/// Gets the documents.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documentIds">The document ids.</param>
		/// <returns>IEnumerable{``0}.</returns>
		public IEnumerable<T> GetDocuments<T>(IEnumerable<string> documentIds) where T : IBigDbDocument, new()
		{
			var bulk = new BigDBulkKeys(documentIds.Cast<object>());
			var json = BigDDocument.WriteJson(bulk,false);
			return QueryAllDocuments()
				.Data(json)
				.IncludeDocuments()
				.GetResult()
				.Documents<T>();
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>``0.</returns>
		public T GetDocument<T>(string documentId) where T : IBigDbDocument, new()
		{
			var doc = new T { Id = documentId };
			try
			{
				JObject obj = ReadDocumentJObject(documentId);
                doc = JSonHelper<T>.ReadJSon(obj);
                ReadDocument(doc);
			}
			catch (BigDNotFoundException)
			{
				return default(T);
			}
			return doc;
		}

		/// <summary>
		/// Gets the arbitrary document.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="ctor">The ctor.</param>
		/// <returns>``0.</returns>
		public T GetArbitraryDocument<T>(string documentId, Func<T> ctor)
		{
			var doc = new BigDDocumentWrapper<T>(ctor);
			doc.Id = documentId;
			try
			{
				ReadDocument(doc);
			}
			catch (BigDNotFoundException)
			{
				return default(T);
			}
			return doc.Instance;
		}

		/// <summary>
		/// Gets the arbitrary documents.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="documentIds">The document ids.</param>
		/// <param name="ctor">The ctor.</param>
		/// <returns>IEnumerable{``0}.</returns>
		public IEnumerable<T> GetArbitraryDocuments<T>(IEnumerable<string> documentIds, Func<T> ctor)
		{
			var bulk = new BigDBulkKeys(documentIds.Cast<object>());
			var json = BigDDocument.WriteJson(bulk,false);
			return QueryAllDocuments()
				.Data(json)
				.IncludeDocuments()
				.GetResult()
				.ArbitraryDocuments(ctor);
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns>CouchJsonDocument.</returns>
		public BigDJsonDocument GetDocument(string documentId)
		{
			try
			{
				try
				{
					return new BigDJsonDocument(Request(documentId).Parse());
				}
				catch (WebException e)
				{
					throw BigDException.Create("Failed to get document", e);
				}
			}
			catch (BigDNotFoundException)
			{
				return null;
			}
		}
		/// <summary>
		/// Query a view by name (that we know exists in CouchDB). This method then creates
		/// a CouchViewDefinition on the fly. Better to use existing CouchViewDefinitions.
		/// </summary>
		/// <param name="designName">Name of the design.</param>
		/// <param name="viewName">Name of the view.</param>
		/// <returns>CouchQuery.</returns>
		public BigDQuery Query(string designName, string viewName)
		{
			return Query(new BigDViewDefinition(viewName, NewDesignDocument(designName)));
		}

		public BigDQuery Query(RequestBase req)
		{
			return Query(req.DocName, req.ViewName);
		}

		/// <summary>
		/// Queries the specified view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <returns>CouchQuery.</returns>
		public BigDQuery Query(IBigDViewDefinition view)
		{
			return new BigDQuery(view);
		}

		#if XAMARIN
		#else

		/// <summary>
		/// Queries the specified view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <returns>CouchLuceneQuery.</returns>
		public BigDLuceneQuery Query(BigDLuceneViewDefinition view)
		{
			return new BigDLuceneQuery(view);
		}
		#endif

		/// <summary>
		/// Queries all documents.
		/// </summary>
		/// <returns>CouchQuery.</returns>
		public BigDQuery QueryAllDocuments()
		{
			return Query(null, "_all_docs");
		}

		/// <summary>
		/// Touches the view.
		/// </summary>
		/// <param name="designDocumentId">The design document identifier.</param>
		/// <param name="viewName">Name of the view.</param>
		public void TouchView(string designDocumentId, string viewName)
		{
			Query(designDocumentId, viewName).Limit(0).GetResult();
		}

		/// <summary>
		/// Deletes the document.
		/// </summary>
		/// <param name="document">The document.</param>
		public void DeleteDocument(IBigDbDocument document)
		{
			DeleteDocument(document.Id, document.Rev);
		}

		/// <summary>
		/// Deletes the attachment.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <returns>ICouchDocument.</returns>
		public IBigDbDocument DeleteAttachment(IBigDbDocument document, string attachmentName)
		{
			JObject result = Request(document.Id+ "/" + attachmentName).Query("?rev=" + document.Rev).Delete().Check("Failed to delete attachment").Result();
			document.Id= result["id"].Value<string>(); // Not really neeed
			document.Rev = result["rev"].Value<string>();
			return document;
		}

		/// <summary>
		/// Deletes the attachment.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="rev">The rev.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		public void DeleteAttachment(string id, string rev, string attachmentName)
		{
			Request(id + "/" + attachmentName).Query("?rev=" + rev).Delete().Check("Failed to delete attachment");
		}

		/// <summary>
		/// Deletes the document.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="rev">The rev.</param>
		public void DeleteDocument(string id, string rev)
		{
			Request(id).Query("?rev=" + rev).Delete().Check("Failed to delete document");
			SynchDesignDocuments();
		}

		/// <summary>
		/// Delete documents in key range. This method needs to retrieve
		/// revisions and then use them to post a bulk delete. Couch can not
		/// delete documents without being told about their revisions.
		/// </summary>
		/// <param name="startKey">The start key.</param>
		/// <param name="endKey">The end key.</param>
		public void DeleteDocuments(string startKey, string endKey)
		{
			var docs = QueryAllDocuments().StartKey(startKey).EndKey(endKey).GetResult().RowDocuments().Cast<IBigDbDocument>();
			DeleteDocuments(docs);
		}

		/// <summary>
		/// Delete documents in bulk fashion.
		/// </summary>
		/// <param name="documents">Array of documents to delete.</param>
		public void DeleteDocuments(IEnumerable<IBigDbDocument> documents)
		{
			DeleteDocuments(new BigDBulkDeleteDocuments(documents));
		}

		/// <summary>
		/// Delete documents in bulk fashion.
		/// </summary>
		/// <param name="bulk">The bulk.</param>
		public void DeleteDocuments(ICanJson bulk)
		{
			try
			{
				var json = BigDDocument.WriteJson(bulk,false);

				var result = Request("_bulk_docs")
					.Data(json)
					.PostJson()
					.Parse<JArray>();
				for (var i = 0; i < result.Count(); i++)
				{
					//documents[i]._id= (result[i])["id"].Value<string>();
					//documents[i]._rev = (result[i])["rev"].Value<string>();
					if ((result[i])["error"] == null) continue;
					throw BigDException.Create(string.Format(CultureInfo.InvariantCulture,
						"Document with id {0} was not deleted: {1}: {2}",
						(result[i])["id"].Value<string>(), (result[i])["error"], (result[i])["reason"]));
				}
			}
			catch (WebException e)
			{
				throw BigDException.Create("Failed to bulk delete documents", e);
			}
		}

		/// <summary>
		/// Determines whether the specified document has document.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns><c>true</c> if the specified document has document; otherwise, <c>false</c>.</returns>
		public bool HasDocument(IBigDbDocument document)
		{
			return HasDocument(document.Id);
		}

		/// <summary>
		/// Determines whether the specified document has attachment.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <returns><c>true</c> if the specified document has attachment; otherwise, <c>false</c>.</returns>
		public bool HasAttachment(IBigDbDocument document, string attachmentName)
		{
			return HasAttachment(document.Id, attachmentName);
		}

		/// <summary>
		/// Determines whether [has document changed] [the specified document].
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns><c>true</c> if [has document changed] [the specified document]; otherwise, <c>false</c>.</returns>
		public bool HasDocumentChanged(IBigDbDocument document)
		{
			return HasDocumentChanged(document.Id, document.Rev);
		}

		/// <summary>
		/// Determines whether [has document changed] [the specified document identifier].
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="rev">The rev.</param>
		/// <returns><c>true</c> if [has document changed] [the specified document identifier]; otherwise, <c>false</c>.</returns>
		public bool HasDocumentChanged(string documentId, string rev)
		{
			return Request(documentId).Head().Send().Etag() != rev;
		}

		/// <summary>
		/// Determines whether the specified document identifier has document.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="revision">The revision.</param>
		/// <returns><c>true</c> if the specified document identifier has document; otherwise, <c>false</c>.</returns>
		public bool HasDocument(string documentId, string revision)
		{
			try
			{
				Request(documentId).QueryOptions(new Dictionary<string, string> {{"Rev", revision}}).Head().Send();
				return true;
			}
			catch (WebException)
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified document identifier has document.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <returns><c>true</c> if the specified document identifier has document; otherwise, <c>false</c>.</returns>
		public bool HasDocument(string documentId)
		{
			try
			{
				Request(documentId).Head().Send();
				return true;
			}
			catch (WebException)
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified document identifier has attachment.
		/// </summary>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="attachmentName">Name of the attachment.</param>
		/// <returns><c>true</c> if the specified document identifier has attachment; otherwise, <c>false</c>.</returns>
		public bool HasAttachment(string documentId, string attachmentName)
		{
			try
			{
				Request(documentId + "/" + attachmentName).Head().Send();
				return true;
			}
			catch (WebException)
			{
				return false;
			}
		}

		/// <summary>
		/// Copies a document based on its document id.
		/// </summary>
		/// <param name="sourceDocumentId">The source document id.</param>
		/// <param name="destinationDocumentId">The destination document id.</param>
		/// <exception cref="BigDException"></exception>
		/// <remarks>Use this method when the destination document does not exist.</remarks>
		public void Copy(string sourceDocumentId, string destinationDocumentId)
		{
			try
			{
				Request(sourceDocumentId)
					.AddHeader("Destination", destinationDocumentId)
					.Copy()
					.Send()
					.Parse();

				// TODO add the following check statement.
				// Currently on Windows the COPY command does not return an ok=true pair. This might be
				// a bug in the implementation, but once it is sorted out the check should be added.
				//.Check("Error copying document");
			}
			catch (WebException e)
			{
				throw new BigDException(e.Message, e);
			}
		}

		/// <summary>
		/// Copies a document based on its document id and replaces another existing document.
		/// </summary>
		/// <param name="sourceDocumentId">The source document id.</param>
		/// <param name="destinationDocumentId">The destination document id.</param>
		/// <param name="destinationRev">The destination rev.</param>
		/// <exception cref="BigDException"></exception>
		/// <remarks>Use this method when the destination document already exists</remarks>
		public void Copy(string sourceDocumentId, string destinationDocumentId, string destinationRev)
		{
			try
			{
				Request(sourceDocumentId)
					.AddHeader("Destination", destinationDocumentId + "?rev=" + destinationRev)
					.Copy()
					.Parse();

				// TODO add the following check statement.
				// Currently on Windows the COPY command does not return an ok=true pair. This might be
				// a bug in the implementation, but once it is sorted out the check should be added.
				//.Check("Error copying document");
			}
			catch (WebException e)
			{
				throw new BigDException(e.Message, e);
			}
		}

		/// <summary>
		/// Copies the specified source document to the destination document, replacing it.
		/// </summary>
		/// <param name="sourceDocument">The source document.</param>
		/// <param name="destinationDocument">The destination document.</param>
		/// <remarks>This method does not update the destinationDocument object.</remarks>
		public void Copy(IBigDbDocument sourceDocument, IBigDbDocument destinationDocument)
		{
			Copy(sourceDocument.Id, destinationDocument.Id, destinationDocument.Rev);
		}
	}
}
