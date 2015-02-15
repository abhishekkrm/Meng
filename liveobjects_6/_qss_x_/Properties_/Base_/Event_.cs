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

namespace QS._qss_x_.Properties_.Base_
{
    [QS.Fx.Printing.Printable("Event_", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Event_ : IEvent_
    {
        #region Constructor

        public Event_(EventCallback_ _callback)
        {
            this._callback = _callback;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private IEvent_ _next;
        [QS.Fx.Printing.Printable]
        private EventCallback_ _callback;

        #endregion

        #region IEvent_ Members

        IEvent_ IEvent_._Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        EventCallback_ IEvent_._Callback
        {
            get { return this._callback; }
        }

        #endregion
    }

    [QS.Fx.Printing.Printable("Event_", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Event_<C> : IEvent_<C>
    {
        #region Constructor

        public Event_(EventCallback_ _callback, C _object)
        {
            this._callback = _callback;
            this._object = _object;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private IEvent_ _next;
        [QS.Fx.Printing.Printable]
        private EventCallback_ _callback;
        [QS.Fx.Printing.Printable]
        private C _object;

        #endregion

        #region IEvent_ Members

        IEvent_ IEvent_._Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        EventCallback_ IEvent_._Callback
        {
            get { return this._callback; }
        }

        C IEvent_<C>._Object
        {
            get { return this._object; }
        }

        #endregion
    }

    [QS.Fx.Printing.Printable("Event_", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Event_<C1, C2> : IEvent_<C1, C2>
    {
        #region Constructor

        public Event_(EventCallback_ _callback, C1 _object1, C2 _object2)
        {
            this._callback = _callback;
            this._object1 = _object1;
            this._object2 = _object2;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private IEvent_ _next;
        [QS.Fx.Printing.Printable]
        private EventCallback_ _callback;
        [QS.Fx.Printing.Printable]
        private C1 _object1;
        [QS.Fx.Printing.Printable]
        private C2 _object2;

        #endregion

        #region IEvent_ Members

        IEvent_ IEvent_._Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        EventCallback_ IEvent_._Callback
        {
            get { return this._callback; }
        }

        C1 IEvent_<C1, C2>._Object1
        {
            get { return this._object1; }
        }

        C2 IEvent_<C1, C2>._Object2
        {
            get { return this._object2; }
        }

        #endregion
    }

    [QS.Fx.Printing.Printable("Event_", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Event_<C1, C2, C3> : IEvent_<C1, C2, C3>
    {
        #region Constructor

        public Event_(EventCallback_ _callback, C1 _object1, C2 _object2, C3 _object3)
        {
            this._callback = _callback;
            this._object1 = _object1;
            this._object2 = _object2;
            this._object3 = _object3;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private IEvent_ _next;
        [QS.Fx.Printing.Printable]
        private EventCallback_ _callback;
        [QS.Fx.Printing.Printable]
        private C1 _object1;
        [QS.Fx.Printing.Printable]
        private C2 _object2;
        [QS.Fx.Printing.Printable]
        private C3 _object3;

        #endregion

        #region IEvent_ Members

        IEvent_ IEvent_._Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        EventCallback_ IEvent_._Callback
        {
            get { return this._callback; }
        }

        C1 IEvent_<C1, C2, C3>._Object1
        {
            get { return this._object1; }
        }

        C2 IEvent_<C1, C2, C3>._Object2
        {
            get { return this._object2; }
        }

        C3 IEvent_<C1, C2, C3>._Object3
        {
            get { return this._object3; }
        }

        #endregion
    }
}
