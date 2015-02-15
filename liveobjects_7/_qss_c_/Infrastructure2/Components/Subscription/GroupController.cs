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
    public sealed class GroupController : Interfaces.Subscription.IGroupReference
    {
        public GroupController(string name, Base3_.GroupID groupID)
        {
            this.name = name;
            this.groupID = groupID;
        }

        private string name;
        private Base3_.GroupID groupID;

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IGroupReference Members

        string QS._qss_c_.Infrastructure2.Interfaces.Subscription.IGroupReference.Name
        {
            get { return name; }
        }

        IDisposable QS._qss_c_.Infrastructure2.Interfaces.Subscription.IGroupReference.Register(uint service_id, QS._qss_c_.Base3_.ReceiveCallback callback)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ISender Members

        IAsyncResult QS._qss_c_.Base8_.ISender.BeginSend(QS._core_c_.Base3.Message message, AsyncCallback callback, object context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void QS._qss_c_.Base8_.ISender.EndSend(IAsyncResult operation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void QS._qss_c_.Base8_.ISender.Send(QS._core_c_.Base3.Message message)
        {
            ((Base8_.ISender)this).BeginSend(message, null, null);
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICaller Members

        QS.Fx.Serialization.ISerializable QS._qss_c_.Base8_.ICaller.Call(QS._core_c_.Base3.Message message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IAsyncResult QS._qss_c_.Base8_.ICaller.BeginCall(QS._core_c_.Base3.Message message, AsyncCallback callback, object context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        QS.Fx.Serialization.ISerializable QS._qss_c_.Base8_.ICaller.EndCall(IAsyncResult operation)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ISink<IAsynchronous<Message,object,ISerializable>> Members

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object, QS.Fx.Serialization.ISerializable>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object, QS.Fx.Serialization.ISerializable>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object, QS.Fx.Serialization.ISerializable>> getObjectsCallback)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
