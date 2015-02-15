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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    public abstract class SerializableSender : ISerializableSender
    {
        protected SerializableSender(QS._qss_c_.Base3_.ISerializableSender underlyingSender)
        {
            this.underlyingSender = underlyingSender;
        }

        protected QS._qss_c_.Base3_.ISerializableSender underlyingSender;

        #region QS.CMS.Base3.ISerializableSender Members

        public QS.Fx.Network.NetworkAddress Address
        {
            get
            {
                return underlyingSender.Address;
            }
        }

        public abstract void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data);
        
        public abstract int MTU
        {
            get;
        }        

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return underlyingSender.CompareTo(obj);
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            return underlyingSender.Equals(obj);
        }

        public override int GetHashCode()
        {
            return underlyingSender.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().Name.ToString() + "(" + underlyingSender.Address.ToString() + ")";
        }

        #endregion
    }
}
