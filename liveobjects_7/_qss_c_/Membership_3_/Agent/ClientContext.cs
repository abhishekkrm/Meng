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

namespace QS._qss_c_.Membership_3_.Agent
{
/*
    public sealed class ClientContext : Abstractions.IClientContext
    {
        public ClientContext()
        {
        }

        private Abstractions.IClient client;
        private bool closed;
        private Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>> getObjectsCallback; 

        #region Interface used by other infrastructure components

        public void Signal()
        {
            client.Signal();
        }

        public void Close()
        {
            lock (this)
            {
                closed = true;
            }

            // ..........should initiate flushing or something...
        }

        #endregion

        #region IClientContext Members

        QS.CMS.Membership3.Abstractions.IMetaNodeRef[] 
            QS.CMS.Membership3.Abstractions.IClientContext.MetaNodeMembers
        {
            get 
            { 
                
                throw new Exception("The method or operation is not implemented."); 
            }
        }

        void QS.CMS.Membership3.Abstractions.IClientContext.GetMessagesToSend(
            Queue<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>> destinationQueue, 
            int maximumNumberOfObjectsToReturn, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
        {
            if (closed)
            {
                numberOfObjectsReturned = 0;
                moreObjectsAvailable = false;
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        #endregion
    }
*/ 
}
