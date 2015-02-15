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

namespace QS._qss_c_.Infrastructure2.Components.SubscriptionService
{
    public sealed class SubscriptionService : Interfaces.SubscriptionService.ISubscriptionService
    {
        public SubscriptionService(QS.Fx.Logging.ILogger logger, Interfaces.Types.IGroupTypeLibrary groupTypeLibrary)
        {
            this.logger = logger;
            this.groupTypeLibrary = groupTypeLibrary;
        }

        private QS.Fx.Logging.ILogger logger;
        private Interfaces.Types.IGroupTypeLibrary groupTypeLibrary;

        #region ISubscriptionService Members

        IAsyncResult QS._qss_c_.Infrastructure2.Interfaces.SubscriptionService.ISubscriptionService.BeginOpen(string group_name, 
            QS._qss_c_.Infrastructure2.Interfaces.Common.OpeningMode opening_mode, QS._qss_c_.Infrastructure2.Interfaces.Common.AccessMode access_mode, 
            QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute[] group_properties, 
            string type_name, QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute[] type_attributes, 
            QS._qss_c_.Infrastructure2.Interfaces.Common.Attribute[] custom_attributes,
            System.AsyncCallback asynchronousCallback, object asynchronousState)
        {
            logger.Log(this, "__________SubscriptionService.Open(\"" + group_name + "\")_Begin");

            OpenRequest openRequest = 
                new OpenRequest(group_name, opening_mode, access_mode, group_properties, type_name, type_attributes, custom_attributes);
            Base3_.AsyncResult<OpenRequest> asyncResult = 
                new QS._qss_c_.Base3_.AsyncResult<OpenRequest>(asynchronousCallback, asynchronousState, openRequest);

            Interfaces.Types.IGroupType groupType;
            if (!groupTypeLibrary.GetType(type_name, out groupType))
                throw new Exception("Type \"" + type_name + "\" is not recognized.");

            // ................................................................................................................................................................................POO
            openRequest.GroupID = 666;

            asyncResult.Completed(true, true, null);

            logger.Log(this, "__________SubscriptionService.Open(\"" + group_name + "\")_End");

            return asyncResult;
        }

        Base3_.GroupID QS._qss_c_.Infrastructure2.Interfaces.SubscriptionService.ISubscriptionService.EndOpen(IAsyncResult asynchronousResult)
        {
            Base3_.AsyncResult<OpenRequest> openRequest = (Base3_.AsyncResult<OpenRequest>)asynchronousResult;
            return openRequest.Context.GroupID;
        }

        #endregion
    }
}
