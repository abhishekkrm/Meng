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

namespace QS.Fx.Base
{
    public sealed class Event : IEvent
    {
        public Event(ContextCallback callback, object context, SynchronizationOption option)
        {
            this.callback = callback;
            this.context = context;
            this.option = option;
        }

        public Event(ContextCallback callback, object context)
            : this(callback, context, SynchronizationOption.Synchronous | SynchronizationOption.Singlethreaded)
        {
        }

        public Event(ContextCallback callback)
            : this(callback, null, SynchronizationOption.Synchronous | SynchronizationOption.Singlethreaded)
        {
        }

        private ContextCallback callback;
        private object context;
        private IEvent next;
        private SynchronizationOption option;

        #region IEvent Members

        void IEvent.Handle()
        {
            callback(context);
        }

        IEvent IEvent.Next
        {
            get { return this.next; }
            set { this.next = value; }
        }

        SynchronizationOption IEvent.SynchronizationOption
        {
            get { return this.option; }
        }

        #endregion
    }

    public sealed class Event<C> : IEvent
    {
        public Event(ContextCallback<C> callback, C context, SynchronizationOption option)
        {
            this.callback = callback;
            this.context = context;
            this.option = option;
        }

        public Event(ContextCallback<C> callback, C context) 
            : this(callback, context, SynchronizationOption.Synchronous | SynchronizationOption.Singlethreaded)
        {
        }

        private ContextCallback<C> callback;
        private C context;
        private IEvent next;
        private SynchronizationOption option;

        #region IEvent Members

        void IEvent.Handle()
        {
            callback(context);
        }

        IEvent IEvent.Next
        {
            get { return this.next; }
            set { this.next = value; }
        }

        SynchronizationOption IEvent.SynchronizationOption
        {
            get { return this.option; }
        }

        #endregion
    }

    public sealed class Event<C1, C2> : IEvent
    {
        public Event(ContextCallback<C1, C2> callback, C1 context1, C2 context2, SynchronizationOption option)
        {
            this.callback = callback;
            this.context1 = context1;
            this.context2 = context2;
            this.option = option;
        }

        public Event(ContextCallback<C1, C2> callback, C1 context1, C2 context2)
            : this(callback, context1, context2, SynchronizationOption.Synchronous | SynchronizationOption.Singlethreaded)
        {
        }

        private ContextCallback<C1, C2> callback;
        private C1 context1;
        private C2 context2;
        private IEvent next;
        private SynchronizationOption option;

        #region IEvent Members

        void IEvent.Handle()
        {
            callback(context1, context2);
        }

        IEvent IEvent.Next
        {
            get { return this.next; }
            set { this.next = value; }
        }

        SynchronizationOption IEvent.SynchronizationOption
        {
            get { return this.option; }
        }

        #endregion
    }

    public sealed class Event<C1, C2, C3> : IEvent
    {
        public Event(ContextCallback<C1, C2, C3> callback, C1 context1, C2 context2, C3 context3, SynchronizationOption option)
        {
            this.callback = callback;
            this.context1 = context1;
            this.context2 = context2;
            this.context3 = context3;
            this.option = option;
        }

        public Event(ContextCallback<C1, C2, C3> callback, C1 context1, C2 context2, C3 context3)
            : this(callback, context1, context2, context3, SynchronizationOption.Synchronous | SynchronizationOption.Singlethreaded)
        {
        }

        private ContextCallback<C1, C2, C3> callback;
        private C1 context1;
        private C2 context2;
        private C3 context3;
        private IEvent next;
        private SynchronizationOption option;

        #region IEvent Members

        void IEvent.Handle()
        {
            callback(context1, context2, context3);
        }

        IEvent IEvent.Next
        {
            get { return this.next; }
            set { this.next = value; }
        }

        SynchronizationOption IEvent.SynchronizationOption
        {
            get { return this.option; }
        }

        #endregion
    }
}
