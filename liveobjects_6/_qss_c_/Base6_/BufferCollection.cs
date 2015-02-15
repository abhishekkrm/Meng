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

namespace QS._qss_c_.Base6_
{
    public class BufferCollection<AddressClass> 
        : Base3_.ISenderCollection<AddressClass, Base3_.IReliableSerializableSender>, QS._core_c_.Diagnostics2.IModule 
        where AddressClass : QS.Fx.Serialization.IStringSerializable, new()
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public BufferCollection(Base6_.ICollectionOf<AddressClass, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection, QS.Fx.Clock.IClock clock)
        {
            this.sinkCollection = sinkCollection;
            this.clock = clock;
        }

        private Base6_.ICollectionOf<AddressClass, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection;
        private QS.Fx.Clock.IClock clock;
        private IDictionary<AddressClass, Base3_.IReliableSerializableSender> senders =
            new Dictionary<AddressClass, Base3_.IReliableSerializableSender>();

        #region ISenderCollection<AddressClass,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender 
            QS._qss_c_.Base3_.ISenderCollection<AddressClass, QS._qss_c_.Base3_.IReliableSerializableSender>.this[
                AddressClass destinationAddress]
        {
            get 
            {
                lock (this)
                {
                    if (senders.ContainsKey(destinationAddress))
                        return senders[destinationAddress];
                    else
                    {
                        Buffer buffer = new Buffer(sinkCollection[destinationAddress], clock);
                        senders[destinationAddress] = buffer;

                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(destinationAddress.ToString(), ((QS._core_c_.Diagnostics2.IModule)buffer).Component);
                        return buffer;
                    }
                }                
            }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get 
            {
                List<string> names = new List<string>();
                foreach (AddressClass address in senders.Keys)
                    names.Add(address.AsString);
                return names; 
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            { 
                AddressClass address = new AddressClass();
                address.AsString = attributeName;
                return new QS.Fx.Inspection.ScalarAttribute(attributeName, senders[address]);
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Buffer Collection"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
