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

namespace QS._qss_x_.Base1_
{
/*
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_ServiceID)]
    public sealed class ServiceID : QS.Fx.Serialization.ISerializable
    {
        public ServiceID(Uri address, int incarnation)
        {
            this.address = address;
            this.incarnation = incarnation;
        }

        public ServiceID()
        {
        }

        private Uri address;
        private int incarnation;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public Uri Address
        {
            get { return address; }
        }

        [QS.Fx.Printing.Printable]
        public int Incarnation
        {
            get { return incarnation; }
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            ServiceID other = obj as ServiceID;
            return (other != null) && address.Equals(other.address) && incarnation.Equals(other.incarnation);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode() ^ incarnation.GetHashCode();
        }

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Base_ServiceID,
                    sizeof(ushort) + sizeof(int), sizeof(ushort) + sizeof(int) + Encoding.ASCII.GetBytes(address.OriginalString).Length, 1);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.CMS.Base3.WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
        {
            byte[] address_bytes = Encoding.ASCII.GetBytes(address.OriginalString);
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((ushort*)pheader) = (ushort) address_bytes.Length;
                *((int*)(pheader + sizeof(ushort))) = incarnation;
            }
            header.consume(sizeof(ushort) + sizeof(int));
            data.Add(new ArraySegment<byte>(address_bytes));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int address_length;
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                address_length = (int) (*((ushort*)pheader));
                incarnation = *((int*)(pheader + sizeof(ushort)));
            }
            header.consume(sizeof(ushort) + sizeof(int));
            address = new Uri(Encoding.ASCII.GetString(data.Array, data.Offset, address_length));
            data.consume(address_length);
        }

        #endregion
    }
 */
}
