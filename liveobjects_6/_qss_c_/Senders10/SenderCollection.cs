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

namespace QS._qss_c_.Senders10
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class SenderCollection<AddressClass>
        : QS.Fx.Inspection.Inspectable, Base3_.ISenderCollection<AddressClass, Base3_.IReliableSerializableSender>
        where AddressClass : QS.Fx.Serialization.IStringSerializable, new()
    {
        public SenderCollection(QS.Fx.Logging.ILogger logger, ISenderControllerClass<AddressClass> senderControllerClass)
        {
            this.logger = logger;
            this.senderControllerClass = senderControllerClass;

            senderCollectionInspectableWrapper = 
                new QS._qss_e_.Inspection_.DictionaryWrapper3<AddressClass, ISenderController<AddressClass>>(
                "Senders", senderCollection);
        }

        private QS.Fx.Logging.ILogger logger;
        private ISenderControllerClass<AddressClass> senderControllerClass;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<AddressClass, ISenderController<AddressClass>> senderCollection =
            new Dictionary<AddressClass, ISenderController<AddressClass>>();
        [QS.Fx.Base.Inspectable("Senders")]
        private QS._qss_e_.Inspection_.DictionaryWrapper3<AddressClass, ISenderController<AddressClass>> 
            senderCollectionInspectableWrapper;

        #region CreateSender

        private ISenderController<AddressClass> CreateSender(AddressClass destinationAddress)
        {
            return senderControllerClass.CreateSender(destinationAddress);
        }

        #endregion

        #region GetSender

        private ISenderController<AddressClass> GetSender(AddressClass destinationAddress)
        {
            lock (this)
            {
                if (senderCollection.ContainsKey(destinationAddress))
                    return senderCollection[destinationAddress];
                else
                {
                    ISenderController<AddressClass> sender = this.CreateSender(destinationAddress);
                    senderCollection.Add(destinationAddress, sender);
                    return sender;
                }
            }
        }

        #endregion

        #region ISenderCollection<AddressClass,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender 
            QS._qss_c_.Base3_.ISenderCollection<AddressClass, QS._qss_c_.Base3_.IReliableSerializableSender>.this[AddressClass destinationAddress]
        {
            get { return this.GetSender(destinationAddress); }
        }

        #endregion
    }
}
