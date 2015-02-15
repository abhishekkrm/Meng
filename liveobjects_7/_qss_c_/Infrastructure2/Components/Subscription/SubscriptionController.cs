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

namespace QS._qss_c_.Infrastructure2.Components.Subscription
{
    public sealed class SubscriptionController : Interfaces.Subscription.ISubscriptionController, IDisposable
    {
        public SubscriptionController(QS.Fx.Logging.ILogger logger, Interfaces.SubscriptionService.ISubscriptionService subscriptionService)
        {
            this.logger = logger;
            this.subscriptionService = subscriptionService;
        }

        private QS.Fx.Logging.ILogger logger;
        private Interfaces.SubscriptionService.ISubscriptionService subscriptionService;

        #region ISubscriptionController Members

        IAsyncResult QS._qss_c_.Infrastructure2.Interfaces.Subscription.ISubscriptionController.BeginOpen(string group_name, 
            QS._qss_c_.Infrastructure2.Interfaces.Common.OpeningMode opening_mode, QS._qss_c_.Infrastructure2.Interfaces.Common.AccessMode access_mode, 
            IEnumerable<QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute> group_properties, 
            string type_name, IEnumerable<QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute> type_attributes, 
            IEnumerable<QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute> custom_attributes, 
            AsyncCallback asynchronousCallback, object asynchronousState)
        {
            Base3_.AsyncResult<OpenRequest> asyncResult = new QS._qss_c_.Base3_.AsyncResult<OpenRequest>(asynchronousCallback, asynchronousState, 
                new OpenRequest(group_name, opening_mode, access_mode, group_properties, type_name, type_attributes, custom_attributes));

            subscriptionService.BeginOpen(group_name, opening_mode, access_mode,
                ((group_properties != null) ? (new List<Interfaces.Common.Attribute>(group_properties)).ToArray() : new Interfaces.Common.Attribute[] {}),
                type_name, ((type_attributes != null) ? (new List<Interfaces.Common.Attribute>(type_attributes)).ToArray() : new Interfaces.Common.Attribute[] { }),
                ((custom_attributes != null) ? (new List<Interfaces.Common.Attribute>(custom_attributes)).ToArray() : new Interfaces.Common.Attribute[] { }), 
                new AsyncCallback(this.OpenCallback), asyncResult);

            return asyncResult;
        }

        QS._qss_c_.Infrastructure2.Interfaces.Subscription.IGroupReference 
            QS._qss_c_.Infrastructure2.Interfaces.Subscription.ISubscriptionController.EndOpen(IAsyncResult asynchronousResult)
        {
            logger.Log(this, "__SubscriptionController.EndOpen");

            Base3_.AsyncResult<OpenRequest> asyncResult = (Base3_.AsyncResult<OpenRequest>) asynchronousResult;
            OpenRequest openRequest = asyncResult.Context;

            GroupController groupController = new GroupController(openRequest.GroupName, openRequest.GroupID);

            
            // ................................

            


            return groupController;
        }

        #endregion

        #region Callbacks

        private void OpenCallback(IAsyncResult result)
        {
            Base3_.AsyncResult<OpenRequest> asyncResult = (Base3_.AsyncResult<OpenRequest>)result.AsyncState;
            OpenRequest openRequest = asyncResult.Context;

            try
            {
                openRequest.GroupID = subscriptionService.EndOpen(result);
                asyncResult.Completed(false, true, null);
            }
            catch (Exception exc)
            {
                asyncResult.Completed(false, false, exc);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
