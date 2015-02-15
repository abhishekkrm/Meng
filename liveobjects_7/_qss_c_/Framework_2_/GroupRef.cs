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

#define DEBUG_CheckDisposed

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Framework_2_
{
    public sealed class GroupRef : IGroup, IChannel
    {
        #region Constructor

        public GroupRef(Group group, GroupOptions options, QS.Fx.Logging.ILogger logger,
            QS.Fx.Base.ContextCallback<SendRequest> sendingCallback, QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback,
            QS.Fx.Base.ContextCallback<Feed> registerFeed, QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> mainWorker,
            QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> completionWorker)
        {
            this.options = options;
            this.group = group;
            this.logger = logger;
            this.sendingCallback = sendingCallback;
            this.completionCallback = completionCallback;
            this.registerFeed = registerFeed;
            this.mainWorker = mainWorker;
            this.completionWorker = completionWorker;
            this.ishybrid = ((options & GroupOptions.Hybrid) == GroupOptions.Hybrid);
        }

        #endregion

        #region Fields

        private Group group;
        private GroupOptions options;
        private QS.Fx.Logging.ILogger logger;
        private int disposed;
        private ICollection<IncomingCallback> receiveCallbacks = new System.Collections.ObjectModel.Collection<IncomingCallback>();
        private QS.Fx.Base.ContextCallback<SendRequest> sendingCallback;
        private QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback;
        private QS.Fx.Base.ContextCallback<Feed> registerFeed;
        private QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent> mainWorker, completionWorker;
        private bool ishybrid;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                group.RemoveReference(this, false);
            }
        }

        private void _CheckDisposed()
        {
#if DEBUG_CheckDisposed
            if (disposed > 0)
                throw new ObjectDisposedException("Reference to Group " + group.ID.ToString());
#endif
        }

        #endregion

        #region Accessors

        public Group Group
        {
            get { return group; }
        }

        public GroupOptions Options
        {
            get { return options; }
        }

//        public bool IsHybrid
//        {
//            get { return ishybrid; }
//        }

        #endregion

        #region IGroup Members

        public QS._qss_c_.Base3_.GroupID ID
        {
            get { return group.ID; }
        }

        public event IncomingCallback OnReceive
        {
            add 
            {
                _CheckDisposed();
                mainWorker.Process(new ChangeReceiveCallbackRequest(this, value, true));
            }

            remove
            {
                _CheckDisposed();
                mainWorker.Process(new ChangeReceiveCallbackRequest(this, value, false));
            }
        }

        public IChannel BufferedChannel
        {
            get { return this; }
        }

        public void Send(QS.Fx.Serialization.ISerializable message)
        {
            Send(message, null, null);
        }

        public void Send(QS.Fx.Serialization.ISerializable message, CompletionCallback callback, object context)
        {
            _CheckDisposed();
            mainWorker.Process(new SendRequest(
                message, (uint) ReservedObjectID.Framework2_Group, group, this, callback, context, sendingCallback, completionCallback));
        }

        public void ScheduleSend(OutgoingCallback callback)
        {
            _CheckDisposed();
            mainWorker.Process(new QS.Fx.Base.Event<Feed>(registerFeed, 
                new Feed(this, group, ishybrid, callback, sendingCallback, completionCallback)));
        }

        #endregion

        #region ChangeReceiveCallbackRequest

        private class ChangeReceiveCallbackRequest : QS.Fx.Base.IEvent
        {
            public ChangeReceiveCallbackRequest(GroupRef groupRef, IncomingCallback callback, bool adding)
            {
                this.groupRef = groupRef;
                this.callback = callback;
                this.adding = adding;
            }

            private GroupRef groupRef;
            private IncomingCallback callback;
            private bool adding;
            private QS.Fx.Base.IEvent next;

            #region IRequest Members

            void QS.Fx.Base.IEvent.Handle()
            {
                if (adding)
                {
                    if (!groupRef.receiveCallbacks.Contains(callback))
                        groupRef.receiveCallbacks.Add(callback);
                }
                else
                    groupRef.receiveCallbacks.Remove(callback);
            }

            #endregion

            #region IItem<IRequest> Members

            QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
            {
                get { return next; }
                set { next = value; }
            }

            #endregion

            QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
            {
                get { return QS.Fx.Base.SynchronizationOption.None; }
            }

        }

        #endregion

        #region DispatchReceivedMessage

        public void DispatchReceivedMessage(QS._core_c_.Base3.InstanceID sender, QS.Fx.Serialization.ISerializable message, bool safe)
        {
            if (safe || ((options & GroupOptions.FastReceiveCallback) == GroupOptions.FastReceiveCallback))
            {
                foreach (IncomingCallback callback in receiveCallbacks)
                {
                    try
                    {
                        callback(sender, message);
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, "Error dispatching message to " + callback.Target.GetType().ToString() + "." + callback.Method.Name + "\n" + exc.ToString());
                    }
                }
            }
            else
                completionWorker.Process(new DispatchRequest(this, sender, message));
        }

        #endregion
    }
}
