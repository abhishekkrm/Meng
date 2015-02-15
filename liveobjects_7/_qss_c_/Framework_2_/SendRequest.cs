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

namespace QS._qss_c_.Framework_2_
{
    public sealed class SendRequest : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS._core_c_.Synchronization.IItem<SendRequest>, QS.Fx.Base.IEvent, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public SendRequest(QS.Fx.Serialization.ISerializable message, uint channel, Group group, GroupRef groupRef, 
            CompletionCallback appcallback, object appcontext, 
            QS.Fx.Base.ContextCallback<SendRequest> sendingCallback, QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback)
        {
            this.message = message;
            this.channel = channel;
            this.group = group;
            this.groupRef = groupRef;
            this.appcallback = appcallback;
            this.appcontext = appcontext;
            this.sendingCallback = sendingCallback;
            this.completionCallback = completionCallback;
        }

        public SendRequest()
        {
        }

        #endregion

        #region Fields

        private static readonly QS._core_c_.Base6.CompletionCallback<object> _completionCallback =
            new QS._core_c_.Base6.CompletionCallback<object>(_CompletionCallback);

        private QS.Fx.Serialization.ISerializable message;
        private uint channel;
        private Group group;
        private GroupRef groupRef;
        private CompletionCallback appcallback;
        private object appcontext;
        private QS.Fx.Base.ContextCallback<SendRequest> sendingCallback;
        private QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback;
        private bool completed, succeeded;
        private Exception exception;
        private SendRequest next;
        private QS.Fx.Base.IEvent corenext;

        #endregion

        #region Accessors

        public QS.Fx.Serialization.ISerializable Message
        {
            get { return message; }
        }

        public GroupRef GroupRef
        {
            get { return groupRef; }
        }

        public CompletionCallback ApplicationCallback
        {
            get { return appcallback; }
        }

        public object ApplicationContext
        {
            get { return appcontext; }
        }

        public bool Completed
        {
            get { return completed; }
            set { completed = value; }
        }

        public bool Succeeded
        {
            get { return succeeded; }
            set { succeeded = value; }
        }

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message(channel, this); }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return _completionCallback; }
        }

        #endregion

        #region _CompletionCallback

        private static void _CompletionCallback(bool succeded, Exception exception, object context)
        {
            SendRequest request = (SendRequest) context;
            if (request != null)
            {
                QS._core_c_.Base6.CompletionCallback<SendRequest> callback = request.completionCallback;
                if (callback != null)
                    callback(succeded, exception, request);
            }
            else
                throw new Exception("The \"context\" argument is null.");
        }

        #endregion

        #region IItem<SendRequest> Members

        SendRequest QS._core_c_.Synchronization.IItem<SendRequest>.Next
        {
            get { return next; }
            set { next = value; }
        }

        #endregion

        #region IRequest Members

        void QS.Fx.Base.IEvent.Handle()
        {
            if (completed)
                appcallback(succeeded, exception, this);
            else
                sendingCallback(this);
        }

        #endregion

        #region IItem<IRequest> Members

        QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
        {
            get { return corenext; }
            set { corenext = value; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Framework2_Message, sizeof(ushort));
                info.AddAnother(group.ID.SerializableInfo);
                info.AddAnother(message.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            group.ID.SerializeTo(ref header, ref data);
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((ushort*)pheader) = message.SerializableInfo.ClassID;
            }
            header.consume(sizeof(ushort));
            message.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotSupportedException();        
        }

        #endregion

        QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
        {
            get { return QS.Fx.Base.SynchronizationOption.None; }
        }

    }
}
