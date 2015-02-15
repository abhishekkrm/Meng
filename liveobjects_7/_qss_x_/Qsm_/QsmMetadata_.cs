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
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace QS._qss_x_.Qsm_
{
    [QS.Fx.Printing.Printable("Metadata", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.QsmMetadata)]
    [XmlType("Metadata")]
    public sealed class QsmMetadata_ : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public QsmMetadata_(string _id, string _name, string _comment, 
            QS.Fx.Reflection.Xml.ValueClass _messageclass, QS.Fx.Reflection.Xml.ValueClass _checkpointclass, int _index)
        {
            this._id = new QS.Fx.Value.UnicodeText((_id != null) ? _id : string.Empty);
            this._name = new QS.Fx.Value.UnicodeText((_name != null) ? _name : string.Empty);
            this._comment = new QS.Fx.Value.UnicodeText((_comment != null) ? _comment : string.Empty);
            XmlSerializer _serializer = new XmlSerializer(typeof(QS.Fx.Reflection.Xml.ValueClass));
            StringBuilder _sb = new StringBuilder();
            using (StringWriter _writer = new StringWriter(_sb))
            {
                _serializer.Serialize(_writer, _messageclass);
            }
            this._messageclass = new QS.Fx.Value.UnicodeText(_sb.ToString());
            _sb = new StringBuilder();
            using (StringWriter _writer = new StringWriter(_sb))
            {
                _serializer.Serialize(_writer, _checkpointclass);
            }
            this._checkpointclass = new QS.Fx.Value.UnicodeText(_sb.ToString()); this._index = _index;
        }

        public QsmMetadata_()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Value.UnicodeText _id, _name, _comment, _messageclass, _checkpointclass;
        private int _index;

        #endregion

        #region Accessors

        [XmlElement("ID")]
        public string _ID
        {
            get { return ((QS.Fx.Value.Classes.IText) this._id).Text; }
            set { this._id = new QS.Fx.Value.UnicodeText((value != null) ? value : string.Empty); }
        }

        [XmlElement("Name")]
        public string _Name
        {
            get { return ((QS.Fx.Value.Classes.IText) this._name).Text; }
            set { this._name = new QS.Fx.Value.UnicodeText((value != null) ? value : string.Empty); }
        }

        [XmlElement("Comment")]
        public string _Comment
        {
            get { return ((QS.Fx.Value.Classes.IText) this._comment).Text; }
            set { this._comment = new QS.Fx.Value.UnicodeText((value != null) ? value : string.Empty); }
        }

        [XmlElement("MessageClass")]
        public QS.Fx.Reflection.Xml.ValueClass _MessageClass
        {
            get
            {
                using (StringReader _reader = new StringReader(((QS.Fx.Value.Classes.IText)_messageclass).Text))
                {
                    return (QS.Fx.Reflection.Xml.ValueClass)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.ValueClass))).Deserialize(_reader);
                }
            }

            set 
            {
                XmlSerializer _serializer = new XmlSerializer(typeof(QS.Fx.Reflection.Xml.ValueClass));
                StringBuilder _sb = new StringBuilder();
                using (StringWriter _writer = new StringWriter(_sb))
                {
                    _serializer.Serialize(_writer, value);
                }
                this._messageclass = new QS.Fx.Value.UnicodeText(_sb.ToString());
            }
        }

        [XmlElement("CheckpointClass")]
        public QS.Fx.Reflection.Xml.ValueClass _CheckpointClass
        {
            get
            {
                using (StringReader _reader = new StringReader(((QS.Fx.Value.Classes.IText)_checkpointclass).Text))
                {
                    return (QS.Fx.Reflection.Xml.ValueClass)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.ValueClass))).Deserialize(_reader);
                }
            }

            set
            {
                XmlSerializer _serializer = new XmlSerializer(typeof(QS.Fx.Reflection.Xml.ValueClass));
                StringBuilder _sb = new StringBuilder();
                using (StringWriter _writer = new StringWriter(_sb))
                {
                    _serializer.Serialize(_writer, value);
                }
                this._checkpointclass = new QS.Fx.Value.UnicodeText(_sb.ToString());
            }
        }

        [XmlElement("Index")]
        public int _Index
        {
            get { return this._index; }
            set { this._index = value; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.QsmMetadata);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._id).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._name).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._comment).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._messageclass).SerializableInfo);
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._checkpointclass).SerializableInfo);
                _info.ExtendHeader(sizeof(int));
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)this._id).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._name).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._comment).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._messageclass).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._checkpointclass).SerializeTo(ref header, ref data);
            fixed (byte* pheaderblock = header.Array)
            {
                byte* pheader = pheaderblock + header.Offset;
                *((int*) pheader) = this._index;
            }
            header.consume(sizeof(int));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this._id = new QS.Fx.Value.UnicodeText();
            this._name = new QS.Fx.Value.UnicodeText();
            this._comment = new QS.Fx.Value.UnicodeText();
            this._messageclass = new QS.Fx.Value.UnicodeText();
            this._checkpointclass = new QS.Fx.Value.UnicodeText();
            ((QS.Fx.Serialization.ISerializable)this._id).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._name).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._comment).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._messageclass).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._checkpointclass).DeserializeFrom(ref header, ref data);
            fixed (byte* pheaderblock = header.Array)
            {
                byte* pheader = pheaderblock + header.Offset;
                this._index = *((int*)pheader);
            }
            header.consume(sizeof(int));
        }

        #endregion    
    }
}
