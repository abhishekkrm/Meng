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

namespace QS._qss_x_.Runtime_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    public sealed class Import_ : IOperation_
    {
        public Import_(QS.Fx.Base.IHandler _handler, object _context, int _id, QS.Fx.Object.Classes.IObject _object)
        {
            this._handler = _handler;
            this._context = _context;
            this._id = _id;
            this._object = _object;
        }

        public Import_()
        {
        }

        [QS.Fx.Printing.Printable]
        private int _id;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Object.Classes.IObject _object;
        [NonSerialized]
        private QS.Fx.Base.IHandler _handler;
        [NonSerialized]
        private object _context;
        [NonSerialized]
        private QS.Fx.Base.IEvent _next;
        [NonSerialized]
        private SerializationType_ _serializationtype = SerializationType_._Binary;

        SerializationType_ IOperation_._Serialization
        {
            get { return this._serializationtype; }
        }

        public QS.Fx.Object.Classes.IObject _Object
        {
            get { return this._object; }
        }

        OperationType_ IOperation_._Type
        {
            get { return OperationType_._Import; }
        }

        object IOperation_._Context
        {
            get { return this._context; }
        }

        int IOperation_._Id
        {
            get { return this._id; }
        }

        QS.Fx.Base.IHandler IOperation_._Handler
        {
            get { return this._handler; }
            set { this._handler = value; }
        }

        void QS.Fx.Base.IEvent.Handle()
        {
            this._handler.Handle(this);
        }

        QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
        {
            get { return QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded; }
        }
    }
}
