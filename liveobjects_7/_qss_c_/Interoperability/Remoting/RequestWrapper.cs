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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;

namespace QS._qss_c_.Interoperability.Remoting
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Interoperability_Remoting_RequestWrapper)]
    public class RequestWrapper : QS.Fx.Serialization.ISerializable
    {
        public RequestWrapper()
        {
        }

        public RequestWrapper(ITransportHeaders requestHeaders, System.IO.Stream requestStream) : this(null, requestHeaders, requestStream)
        {
        }

        public RequestWrapper(string objectURI, ITransportHeaders requestHeaders, System.IO.Stream requestStream)
        {
            wrappedObjectURI = new QS._core_c_.Base2.StringWrapper((objectURI != null) ? objectURI : "");

            wrappedHeaders = new HeaderWrapper(requestHeaders);

            System.IO.MemoryStream fs = new System.IO.MemoryStream();
            byte[] buf = new byte[1000];
            int cnt = requestStream.Read(buf, 0, 1000);
            int bytecount = 0;
            while (cnt > 0)
            {
                fs.Write(buf, 0, cnt);
                bytecount += cnt;
                cnt = requestStream.Read(buf, 0, 1000);
            }

            string body =  Convert.ToBase64String(fs.GetBuffer(), 0, bytecount); 

            wrappedBody = new QS._core_c_.Base2.StringWrapper(body);
        }

        private HeaderWrapper wrappedHeaders;
        private QS._core_c_.Base2.StringWrapper wrappedBody, wrappedObjectURI;

       public string ObjectURI
       {
           get 
           {
               string s = wrappedObjectURI.String;
               return (s.Length > 0) ? s : null;
           }
       }

        public ITransportHeaders TransportHeaders
        {
            get { return wrappedHeaders.TransportHeaders; }
        }

        public System.IO.Stream Stream
        {
            get
            {
                return new System.IO.MemoryStream(Convert.FromBase64String(wrappedBody.String));
            }
        }

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
	        get 
            {
                QS.Fx.Serialization.SerializableInfo info1 = wrappedHeaders.SerializableInfo, info2 = wrappedBody.SerializableInfo, info3 = wrappedObjectURI.SerializableInfo;
                return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Interoperability_Remoting_RequestWrapper,
                    (ushort) (info1.HeaderSize + info2.HeaderSize + info3.HeaderSize), info1.Size + info2.Size + info3.Size, 
                    info1.NumberOfBuffers + info2.NumberOfBuffers + info3.NumberOfBuffers);
            }
        }

        public void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            wrappedObjectURI.SerializeTo(ref header, ref data);
            wrappedHeaders.SerializeTo(ref header, ref data);
            wrappedBody.SerializeTo(ref header, ref data);
        }

        public void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            wrappedObjectURI = new QS._core_c_.Base2.StringWrapper();
            wrappedObjectURI.DeserializeFrom(ref header, ref data);

            wrappedHeaders = new HeaderWrapper();
            wrappedHeaders.DeserializeFrom(ref header, ref data);

            wrappedBody = new QS._core_c_.Base2.StringWrapper();
            wrappedBody.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            System.Text.StringBuilder s = new StringBuilder();
            s.AppendLine("\nObjectURI: " + this.ObjectURI);
            s.AppendLine("\nHeaders:\n");
            foreach (System.Collections.DictionaryEntry entry in wrappedHeaders.TransportHeaders)
                s.AppendLine(entry.Key.ToString() + " = " + entry.Value.ToString());
            s.AppendLine("\nBody:\n\n" + wrappedBody.ToString() + "\n");
            return s.ToString();
        }
    }
}
