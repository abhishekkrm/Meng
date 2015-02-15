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

namespace QS._qss_c_.Membership_3_.Glue
{
    public class GroupController : Groups_.IGroup
    {
        public GroupController(Base3_.GroupID groupID)
        {
            this.groupID = groupID;
        }

        private Base3_.GroupID groupID;

        #region ISender<GroupID> Members

        QS._qss_c_.Base3_.GroupID QS._qss_c_.Base8_.ISender<QS._qss_c_.Base3_.GroupID>.Address
        {
            get { return groupID; }
        }

        void QS._qss_c_.Base8_.ISender.Send(QS._core_c_.Base3.Message message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IAsyncResult QS._qss_c_.Base8_.ISender.BeginSend(QS._core_c_.Base3.Message message, AsyncCallback callback, object context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void QS._qss_c_.Base8_.ISender.EndSend(IAsyncResult operation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }


        #endregion
    }
}
