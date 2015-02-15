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

namespace QS._qss_c_.Glued.Clients
{
    public sealed class Client1
        : Base3_.ClassOf<Membership_3_.Abstractions.IClient, Membership_3_.Abstractions.IClientContext>
    {
        #region Class Client

        private sealed class Client : Membership_3_.Abstractions.IClient,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
        {
            public Client(Client1 owner, Membership_3_.Abstractions.IClientContext context)
            {
                this.owner = owner;
                this.context = context;
            }

            private Client1 owner;
            private Membership_3_.Abstractions.IClientContext context;

            #region IClient Members

            void Membership_3_.Abstractions.IClient.Signal()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region ISink<IAsynchronous<Message>> Members

            void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
                QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
            {
                



            }

            int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        #endregion

        public Client1()
        {
        }

        #region ClassOf<IClient,IClientContext> Members

        Membership_3_.Abstractions.IClient 
            Base3_.ClassOf<Membership_3_.Abstractions.IClient, Membership_3_.Abstractions.IClientContext>.Create(
            Membership_3_.Abstractions.IClientContext context)
        {
            return new Client(this, context);
        }

        #endregion
    }
}
