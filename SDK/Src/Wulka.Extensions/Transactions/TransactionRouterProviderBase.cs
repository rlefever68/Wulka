// ***********************************************************************
// Assembly         : Iris.Fx.Extensions
// Author           : Rafael Lefever
// Created          : 12-20-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 12-20-2013
// ***********************************************************************
// <copyright file="TransactionRouterProviderBase.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Wulka.Domain;
using Wulka.Exceptions;
using Wulka.Interfaces;
using Wulka.Logging;
using Wulka.Networking.Wcf;
using Wulka.Utils;

namespace Wulka.Transactions
{
    /// <summary>
    /// Class TransactionRouterProviderBase.
    /// </summary>
    public abstract class TransactionRouterProviderBase : IDisposable, ITransactionRouter
    {
        /// <summary>
        /// EntryPoint to TransactionProcessor, Filter the file and send transactions to a queue with the 'SendToQueue' method
        /// </summary>
        /// <param name="file">File that requires filtering</param>
        /// <param name="fileName">Name of the original file</param>
        public abstract void FilterFile(byte[] file, string fileName);

        /// <summary>
        /// Gets or sets the queue proxy collection.
        /// </summary>
        /// <value>The queue proxy collection.</value>
        private Dictionary<string, ITransactionProcessor> QueueProxyCollection { get; set; }

        /// <summary>
        /// Ctr for TransactionProcessorProviderBase
        /// </summary>
        protected TransactionRouterProviderBase()
        {
            QueueProxyCollection = new Dictionary<string, ITransactionProcessor>();
        }

        /// <summary>
        /// Sends a file to a queue of choise, the queue will be created if it does not yet exists.
        /// </summary>
        /// <param name="transaction">The transaction (string format)</param>
        /// <param name="queueName">The name of the queue</param>
        protected void SendToQueue(TransactionItem transaction, string queueName)
        {
            queueName = queueName.ToLower().Replace("service", "");
            FxLog.Debug(GetType(),"Sending To Queue: {0}", queueName);
            // Create the transacted MSMQ queue if necessary.
            var queuePath = @".\private$\" + queueName;
            if (!MessageQueue.Exists(queuePath))
            {
                FxLog.Debug(GetType(), "{0} does not yet exist", queuePath);
                var queue = MessageQueue.Create(queuePath, true);
                queue.Authenticate = false;
                queue.SetPermissions("EveryOne", MessageQueueAccessRights.FullControl);
                FxLog.Debug(GetType(),"{0} Created", queuePath);
            }

            // Create proxy for the MSMQ if necessary.
            if (!QueueProxyCollection.ContainsKey(queueName))
            {
                var proxy = CreateProxyForQueue(queueName);
                QueueProxyCollection.Add(queueName, proxy);
            }

            //Get proxy to for MSMQ
            var msmqProxy = QueueProxyCollection[queueName];
            msmqProxy.SendTransactionMessage(transaction);

        }

        /// <summary>
        /// Handles the file in case of a zip File
        /// </summary>
        /// <param name="file">The File</param>
        /// <param name="fileName">Name of the zip file.</param>
        /// <returns>Returns true if the code should continue, false when the processing stops here</returns>
        protected virtual bool HandleZipFile(byte[] file, string fileName)
        {
            try
            {
                var stream = new MemoryStream(file);
                ZipHelper.Decompress(stream, fileName, TransactionConfigurationHelper.TemporaryPath);
                var files = Directory.GetFiles(TransactionConfigurationHelper.TemporaryPath);
                foreach (var tempFile in files)
                {
                    File.Move(tempFile, Path.Combine(TransactionConfigurationHelper.InboxPath, new FileInfo(tempFile).Name));
                }

            }
            catch (Exception ex)
            {
                FxLog.Debug(GetType(), ex.GetCombinedMessages());
                throw;
            }

            return false;
        }

        /// <summary>
        /// Creates the proxy for queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>ITransactionProcessor.</returns>
        private ITransactionProcessor CreateProxyForQueue(string queueName)
        {
            //Create EndPointAddress
            var endpointAddress = new EndpointAddress("net.msmq://"+ TransactionConfigurationHelper.QueueHost +"/private/" + queueName);
            //Create Binding
            var binding = BindingFactory.CreateBindingFromKey(BindingFactory.Key.UnsecureNetMsmqBinding);
            var channelFactory = new ChannelFactory<ITransactionProcessor>(binding, endpointAddress);
            var client = channelFactory.CreateChannel(endpointAddress);
            return client;
        }

        /// <summary>
        /// Dispose of internal proxy library
        /// </summary>
        public void Dispose()
        {
            if (QueueProxyCollection == null) return;
            foreach (var transactionProcessorProcessingService in QueueProxyCollection)
            {
                ((IChannel)transactionProcessorProcessingService.Value).Close();
            }
        }

        /// <summary>
        /// Implemantation of ITransactionRouter
        /// </summary>
        /// <param name="transactionFile">TransactionFile</param>
        public void SubmitTransactionFile(TransactionFileItem transactionFile)
        {
            if (!transactionFile.IsZip || HandleZipFile(transactionFile.File, transactionFile.FileName))
            {
                FilterFile(transactionFile.File, transactionFile.FileName);
            }

        }
    }
}
