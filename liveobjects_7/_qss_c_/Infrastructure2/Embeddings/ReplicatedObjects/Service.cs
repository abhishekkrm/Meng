/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace QS._qss_c_.Infrastructure2.Embeddings.ReplicatedObjects
{
    public sealed class Service : IHost, IFactory, IDisposable
    {
        public Service(QS.Fx.Logging.ILogger logger, string uri)
        {
            this.logger = logger;

            ChannelFactory<Interfaces.SubscriptionService.ISubscriptionService> factory =
                new ChannelFactory<Interfaces.SubscriptionService.ISubscriptionService>(new WSHttpBinding(), new EndpointAddress(new Uri(uri)));
            subscriptionService = factory.CreateChannel();
            subscriptionController = new QS._qss_c_.Infrastructure2.Components.Subscription.SubscriptionController(logger, subscriptionService);            
        }

        private QS.Fx.Logging.ILogger logger;
        private Interfaces.SubscriptionService.ISubscriptionService subscriptionService;
        private Interfaces.Subscription.ISubscriptionController subscriptionController;

        #region Accessors

        public IHost Host
        {
            get { return this; }
        }

        public IFactory Factory
        {
            get { return this; }
        }

        #endregion

        #region IHost Members

        IAsyncResult IHost.BeginOpen<C>(
            C serviceObject, string groupName, Interfaces.Common.OpeningMode openingMode, AsyncCallback callback, object state)
        {
            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult =
                new QS._qss_c_.Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>(callback, state, null);
            subscriptionController.BeginOpen(groupName, openingMode, QS._qss_c_.Infrastructure2.Interfaces.Common.AccessMode.Member, null, 
                ReplicatedObjectGroupType.TypeName, new Interfaces.Common.Attribute[] { new Interfaces.Common.Attribute("interface", typeof(C).FullName) },
                null, new AsyncCallback(this.HostCompletionCallback), asynchronousResult);


            // .......................we should also somehow register the given object "serviceObject", so that it can respond to incoming messages


            return asynchronousResult;
        }

        IDisposable IHost.EndOpen<C>(IAsyncResult result)
        {
            logger.Log(this, "_____ReplicatedObjects.Service.EndOpen");

            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult =
                (Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>)result;

            if (!((IAsyncResult)asynchronousResult).IsCompleted)
                throw new Exception("Sanity check: not completed.");

            if (!asynchronousResult.Succeeded)
                throw asynchronousResult.Exception;

            return asynchronousResult.Context;
        }

        IDisposable IHost.Open<C>(C serviceObject, string groupName, Interfaces.Common.OpeningMode openingMode)
        {
            IAsyncResult asyncResult = ((IHost)this).BeginOpen<C>(serviceObject, groupName, openingMode, null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            return ((IHost)this).EndOpen<C>(asyncResult);
        }

        #endregion

        #region IFactory Members

        IAsyncResult IFactory.BeginOpen<C>(string groupName, Interfaces.Common.OpeningMode openingMode, AsyncCallback callback, object state)
        {
            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult =
                new QS._qss_c_.Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>(callback, state, null);
            subscriptionController.BeginOpen(groupName, openingMode, QS._qss_c_.Infrastructure2.Interfaces.Common.AccessMode.Client, null, 
                ReplicatedObjectGroupType.TypeName, new Interfaces.Common.Attribute[] { new Interfaces.Common.Attribute("interface", typeof(C).FullName) },
                null, new AsyncCallback(this.FactoryCompletionCallback), asynchronousResult);
            return asynchronousResult;
        }
        
        Base3_.Disposable<C> IFactory.EndOpen<C>(IAsyncResult result)
        {
            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult =
                (Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>) result;

            if (!((IAsyncResult) asynchronousResult).IsCompleted)
                throw new Exception("Sanity check: not completed.");

            if (!asynchronousResult.Succeeded)
                throw asynchronousResult.Exception;

            return new Base3_.Disposable<C>(
                (C) (new Proxy(typeof(C), asynchronousResult.Context)).GetTransparentProxy(), 
                asynchronousResult.Context);
        }

        Base3_.Disposable<C> IFactory.Open<C>(string groupName, Interfaces.Common.OpeningMode openingMode)
        {
            IAsyncResult asyncResult = ((IFactory)this).BeginOpen<C>(groupName, openingMode, null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            return ((IFactory)this).EndOpen<C>(asyncResult);
        }

        #endregion

        #region Callbacks

        private void FactoryCompletionCallback(IAsyncResult result)
        {
            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult = 
                (Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>) result.AsyncState;

            try
            {
                asynchronousResult.Context = subscriptionController.EndOpen(result);
                asynchronousResult.Completed(false, true, null);
            }
            catch (Exception exc)
            {
                asynchronousResult.Completed(false, false, exc);
            }            
        }

        private void HostCompletionCallback(IAsyncResult result)
        {
            Base3_.AsyncResult<Interfaces.Subscription.IGroupReference> asynchronousResult =
                (Base3_.AsyncResult<Interfaces.Subscription.IGroupReference>)result.AsyncState;

            try
            {
                asynchronousResult.Context = subscriptionController.EndOpen(result);
                asynchronousResult.Completed(false, true, null);
            }
            catch (Exception exc)
            {
                asynchronousResult.Completed(false, false, exc);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (subscriptionService != null)
            {
                ((IChannel)subscriptionService).Close();
                ((IDisposable)subscriptionService).Dispose();
            }
        }

        #endregion
    }
}
