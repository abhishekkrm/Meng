/*

Copyright (c) 2010 Qi Huang. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the
following conditions
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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using QS.Fx.Value;
using QS.Fx.Serialization;

namespace Quilt.Transmitter
{
    [QS.Fx.Reflection.ValueClass("A37F160CE3EA401bA6EB8E732B3566B7", "Quilt Transmitter Message")]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.TransmitterMsg)]
    [XmlType(TypeName = "TransmitterMsg")]
    public class TransmitterMsg
        : ISerializable, IEUIDable
    {
        #region Constructor

        public TransmitterMsg()
        {
        }

        public TransmitterMsg(EUIDAddress _euid, ISerializable _msg)
        {
            this._msg = _msg;
            this._src_euid = _euid;
        }

        #endregion

        #region Field

        private EUIDAddress _src_euid;
        private ISerializable _msg;

        #endregion

        #region Accessor

        public ISerializable Message
        {
            get { return _msg; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IEUIDable Members

        EUIDAddress IEUIDable.GetEUID()
        {
            return this._src_euid;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ISerializable Members

        SerializableInfo ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.TransmitterMsg), sizeof(ushort));
                info.AddAnother(_msg.SerializableInfo);
                info.AddAnother(((ISerializable)_src_euid).SerializableInfo);
                return info;
            }
        }

        unsafe void ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((ushort*)(_pheader)) = _msg.SerializableInfo.ClassID;
                }

                header.consume(sizeof(ushort));

                ((ISerializable)_src_euid).SerializeTo(ref header, ref data);

                if (this._msg != null)
                    _msg.SerializeTo(ref header, ref data);                
            }
            catch (Exception exc)
            {
                throw new Exception("Quilt.Transmitter.TransmitterMsg SerializeTo Exception! " + exc.Message);
            }
        }

        unsafe void ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            try
            {
                ushort msg_classid;

                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    msg_classid = *((ushort*)(_pheader));
                }

                header.consume(sizeof(ushort));
                
                this._src_euid = new EUIDAddress();
                ((ISerializable)_src_euid).DeserializeFrom(ref header, ref data);

                this._msg = (QS._core_c_.Base3.Serializer.CreateObject(msg_classid));
                _msg.DeserializeFrom(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("Quilt.Transmitter.TransmitterMsg DeserializeFrom Exception! " + exc.Message);
            }
        }

        #endregion
    }
}
