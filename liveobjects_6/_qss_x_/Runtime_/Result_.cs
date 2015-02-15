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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Runtime_Result)]
    public sealed class Result_ : IOperation_, QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public Result_(QS.Fx.Base.IHandler _handler, object _context, int _id, int _sequenceno, QS.Fx.Object.Classes.IObject _object)
        {
            this._handler = _handler;
            this._context = _context;
            this._id = _id;
            this._sequenceno = _sequenceno;
            this._object = _object;
            if (QS.Fx.Object.Runtime.SerializationType == 3)
            {
                QS.Fx.Serialization.ISerializable _serializable = this._object as QS.Fx.Serialization.ISerializable;
                if (_serializable != null)
                    this._serializationtype = SerializationType_._Internal;
                else
                    this._serializationtype = SerializationType_._Binary;
            }
            else
                this._serializationtype = SerializationType_._Binary;
        }

        public Result_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private int _id;
        [QS.Fx.Printing.Printable]
        private int _sequenceno;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Object.Classes.IObject _object;

        [NonSerialized]
        private QS.Fx.Base.IHandler _handler;
        [NonSerialized]
        private object _context;
        [NonSerialized]
        private QS.Fx.Base.IEvent _next;
        [NonSerialized]
        private SerializationType_ _serializationtype;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Runtime_Result, 2 * sizeof(int) + sizeof(ushort), 2 * sizeof(int) + sizeof(ushort), 0);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._object).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = this._id;
                *((int*)(_pbuffer + sizeof(int))) = this._sequenceno;
                *((ushort*)(_pbuffer + 2 * sizeof(int))) =
                    ((this._object != null) ? ((QS.Fx.Serialization.ISerializable)this._object).SerializableInfo.ClassID : ((ushort)0));
            }
            _header.consume(2 * sizeof(int) + sizeof(ushort));
            ((QS.Fx.Serialization.ISerializable)this._object).SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            ushort _objectclass;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                this._id = *((int*)_pbuffer);
                this._sequenceno = *((int*)(_pbuffer + sizeof(int)));
                _objectclass = *((ushort*)(_pbuffer + 2 * sizeof(int)));
            }
            _header.consume(2 * sizeof(int) + sizeof(ushort));
            if (_objectclass > 0)
            {
                this._object = (QS.Fx.Object.Classes.IObject)QS.Fx.Serialization.Serializer.Internal.CreateObject(_objectclass);
                ((QS.Fx.Serialization.ISerializable)this._object).DeserializeFrom(ref _header, ref _data);
            }
            else
                this._object = null;
        }

        #endregion

        SerializationType_ IOperation_._Serialization
        {
            get { return this._serializationtype; }
        }

        public int _SequenceNo
        {
            get { return this._sequenceno; }
        }

        public QS.Fx.Object.Classes.IObject _Object
        {
            get { return this._object; }
        }

        OperationType_ IOperation_._Type
        {
            get { return OperationType_._Result; }
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
