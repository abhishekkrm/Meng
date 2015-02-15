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

namespace QS._qss_x_.Channel_.Message_.Centralized_CC
{
    [QS.Fx.Printing.Printable("Message", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Centralized_CC_Message)]
    public sealed class Message : IMessage
    {
        #region Constructors

        public Message(
            QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType _messagetype, 
            QS.Fx.Base.ID _channel, 
            uint _sequenceno, 
            QS.Fx.Serialization.ISerializable _dataobject)
        {
            this._messagetype = _messagetype;
            this._channel = _channel;
            this._sequenceno = _sequenceno;
            this._dataobject = _dataobject;
        }

        public Message()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("MessageType")]
        private QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType _messagetype;
        [QS.Fx.Printing.Printable("Channel")]
        private QS.Fx.Base.ID _channel;
        [QS.Fx.Printing.Printable("SequenceNo")]        
        private uint _sequenceno;
        [QS.Fx.Printing.Printable("DataObject")]
        private QS.Fx.Serialization.ISerializable _dataobject;

        #endregion

        #region IMessage Members

        QS.Fx.Base.ID IMessage.Channel
        {
            get { return _channel; }
        }

        QS._qss_x_.Channel_.Message_.Centralized_CC.MessageType IMessage.MessageType
        {
	        get { return _messagetype; }
        }

        uint  IMessage.SequenceNo
        {
	        get { return _sequenceno; }
        }

        QS.Fx.Serialization.ISerializable IMessage.Object
        {
	        get { return _dataobject; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Centralized_CC_Message, sizeof(byte) + sizeof(ushort) + sizeof(uint));
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)_channel).SerializableInfo);
                if (_dataobject != null)
                    _info.AddAnother(_dataobject.SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte *parray = header.Array)
            {
                byte *pheader = parray + header.Offset;
                *((byte*)pheader) = (byte)_messagetype;
                pheader += sizeof(byte);
                *((uint*)pheader) = _sequenceno;
                pheader += sizeof(uint);
                *((ushort*)pheader) = (_dataobject != null) ? ((ushort) _dataobject.SerializableInfo.ClassID) : ((ushort) 0);
            }
            header.consume(sizeof(byte) + sizeof(ushort) + sizeof(uint));
            ((QS.Fx.Serialization.ISerializable)_channel).SerializeTo(ref header, ref data);
            if (_dataobject != null)
                _dataobject.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort _classid;
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                _messagetype = (MessageType)(*((byte*)pheader));
                pheader += sizeof(byte);
                _sequenceno = *((uint*)pheader);
                pheader += sizeof(uint);
                _classid = *((ushort*)pheader);
            }
            header.consume(sizeof(byte) + sizeof(ushort) + sizeof(uint));
            _channel = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable)_channel).DeserializeFrom(ref header, ref data);
            if (_classid != 0)
            {
                _dataobject = QS._core_c_.Base3.Serializer.CreateObject(_classid);
                _dataobject.DeserializeFrom(ref header, ref data);
            }
        }

        #endregion    
    }
}
