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

namespace QS._qss_c_.Helpers_
{
    public class SenderCollectionOf 
        : Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>, Base3_.ISenderCollection<Base3_.IReliableSerializableSender>
    {
        public SenderCollectionOf(Base3_.ISenderCollection<Base3_.IReliableSerializableSender> collection)
        {
            this.collection = collection;
        }

        private Base3_.ISenderCollection<Base3_.IReliableSerializableSender> collection;

        #region ISenderCollection<ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender 
            QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return collection[destinationAddress]; }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { return ((QS.Fx.Inspection.IAttributeCollection) collection).AttributeNames; }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { return ((QS.Fx.Inspection.IAttributeCollection) collection)[attributeName]; }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return ((QS.Fx.Inspection.IAttribute)collection).Name; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return ((QS.Fx.Inspection.IAttribute)collection).AttributeClass; }
        }

        #endregion

        #region ISenderCollection<IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender 
            QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return collection[destinationAddress]; }
        }

        #endregion
    }
}
