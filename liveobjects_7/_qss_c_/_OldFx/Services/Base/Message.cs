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
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_c_._OldFx.Services.Base
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Fx_Services_Base_Message)]
    public class Message : QS.Fx.Serialization.ISerializable
    {
        public Message()
        {
        }

        public Message(object request_obj, string service_path, int cookie)
        {
            this.request_obj = request_obj;
            this.service_path = service_path;
            this.cookie = cookie;
            this.binary_requestobj = new QS._qss_c_.Base3_.BinaryObject(request_obj);
            this.binary_servicepath = new QS._core_c_.Base2.StringWrapper(service_path);
        }

        [QS.Fx.Printing.Printable("Object")]
        private object request_obj;
        [QS.Fx.Printing.Printable("Path")]
        private string service_path;
        [QS.Fx.Printing.Printable("Cookie")]
        private int cookie;
        private Base3_.BinaryObject binary_requestobj;
        private QS._core_c_.Base2.StringWrapper binary_servicepath;

        #region Accessors

        public string Path
        {
            get { return service_path; }
        }

        public object Object
        {
            get { return request_obj; }
        }

        public int Cookie
        {
            get { return cookie; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Fx_Services_Base_Message, sizeof(int), sizeof(int), 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)binary_servicepath).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)binary_requestobj).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = cookie;
            }
            header.consume(sizeof(int));
            ((QS.Fx.Serialization.ISerializable)binary_requestobj).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)binary_servicepath).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                cookie = *((int*)pheader);
            }
            header.consume(sizeof(int));
            binary_requestobj = new QS._qss_c_.Base3_.BinaryObject();
            ((QS.Fx.Serialization.ISerializable)binary_requestobj).DeserializeFrom(ref header, ref data);
            binary_servicepath = new QS._core_c_.Base2.StringWrapper();
            ((QS.Fx.Serialization.ISerializable)binary_servicepath).DeserializeFrom(ref header, ref data); 
            request_obj = binary_requestobj.Object;
            service_path = binary_servicepath.String;
        }

        #endregion
    }
}
