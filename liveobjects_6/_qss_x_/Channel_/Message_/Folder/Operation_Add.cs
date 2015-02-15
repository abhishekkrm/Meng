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

namespace QS._qss_x_.Channel_.Message_.Folder
{
    [QS.Fx.Printing.Printable("Operation \"Add\"", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Folder_Operation_Add)]
    public sealed class Operation_Add : IOperation
    {
        #region Constructors

        public Operation_Add(string _id, string _objectxml)
        {
            this._id = new QS.Fx.Value.UnicodeText(_id);
            this._objectxml = new QS.Fx.Value.UnicodeText(_objectxml);
        }

        public Operation_Add()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Value.UnicodeText _id, _objectxml;

        #endregion

        #region Accessors

        public string ID
        {
            get { return ((QS.Fx.Value.Classes.IText)_id).Text; }
            set { _id = new QS.Fx.Value.UnicodeText(value); }
        }

        public string ObjectXml
        {
            get { return ((QS.Fx.Value.Classes.IText)_objectxml).Text; }
            set { _objectxml = new QS.Fx.Value.UnicodeText(value); }
        }

        #endregion

        #region IOperation Members

        OperationType IOperation.OperationType
        {
            get { return OperationType.Add; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Channel_Message_Folder_Operation_Add);
                _info.AddAnother(((QS.Fx.Value.Classes.IText)this._id).SerializableInfo);
                _info.AddAnother(((QS.Fx.Value.Classes.IText)this._objectxml).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) this._id).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) this._objectxml).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this._id = new QS.Fx.Value.UnicodeText();
            ((QS.Fx.Serialization.ISerializable) this._id).DeserializeFrom(ref header, ref data);
            this._objectxml = new QS.Fx.Value.UnicodeText();
            ((QS.Fx.Serialization.ISerializable)this._objectxml).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
