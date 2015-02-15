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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Base.Inspectable]
    public class PlainSender : QS.Fx.Inspection.Inspectable, Multicasting3.IGroupSender
    {
        public PlainSender(Base3_.GroupID groupID)
        {
            this.groupID = groupID;
        }

        private Base3_.GroupID groupID;

        #region IGroupSender Members

        QS._qss_c_.Base3_.GroupID QS._qss_c_.Multicasting3.IGroupSender.GroupID
        {
            get { return groupID; }
        }

        QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Multicasting3.IGroupSender.BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void QS._qss_c_.Multicasting3.IGroupSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int QS._qss_c_.Multicasting3.IGroupSender.MTU
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IComparable<GroupID> Members

        int IComparable<QS._qss_c_.Base3_.GroupID>.CompareTo(QS._qss_c_.Base3_.GroupID other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IComparable<IGroupSender> Members

        int IComparable<QS._qss_c_.Multicasting3.IGroupSender>.CompareTo(QS._qss_c_.Multicasting3.IGroupSender other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
