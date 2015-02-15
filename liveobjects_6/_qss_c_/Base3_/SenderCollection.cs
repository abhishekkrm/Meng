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
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class SenderCollection<SenderInterface> : QS._qss_e_.Base_1_.IStatisticsCollector, ISenderCollection<SenderInterface>
    {
        private const int defaultAnticipatedNumberOfSenders = 10;

        public delegate SenderInterface CreateCallback(QS.Fx.Network.NetworkAddress destinationAddress);

        public SenderCollection(CreateCallback createCallback) : this(createCallback, defaultAnticipatedNumberOfSenders)
        {
        }

        public SenderCollection(CreateCallback createCallback, int anticipatedNumberOfSenders)
        {
            this.createCallback = createCallback;
            senderCollection = new Dictionary<QS.Fx.Network.NetworkAddress, SenderInterface>(anticipatedNumberOfSenders);
            // senderCollection = new Collections.Hashtable((uint) anticipatedNumberOfSenders);
        }

        private CreateCallback createCallback;

        [QS._core_c_.Diagnostics.ComponentCollection("Senders")]
        private IDictionary<QS.Fx.Network.NetworkAddress, SenderInterface> senderCollection;
        // private Collections.Hashtable senderCollection;

        #region ISenderCollection Members

        public SenderInterface this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get
            {
                if (senderCollection.ContainsKey(destinationAddress))
                    return senderCollection[destinationAddress];
                else
                {
                    SenderInterface sender = createCallback(destinationAddress);
                    senderCollection[destinationAddress] = sender;
                    return sender;
                }
/*
                Collections.IDictionaryEntry dic_en = senderCollection.lookupOrCreate(destinationAddress);
                SenderInterface sender = (SenderInterface) dic_en.Value;
                if (dic_en.Value == null)
                {
                    sender = createCallback(destinationAddress);
                    dic_en.Value = sender;
                }
                return sender;
*/

            }
        }

        #endregion

		#region IAttributeCollection Member

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
            get 
            {
                foreach (QS.Fx.Network.NetworkAddress address in senderCollection.Keys)
                    yield return ((QS.Fx.Serialization.IStringSerializable)address).AsString;
                // return senderCollection.Keys; ((TMS.Inspection.IAttributeCollection)senderCollection).AttributeNames; 
            }
		}

		QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get 
            {
                QS.Fx.Network.NetworkAddress address = new QS.Fx.Network.NetworkAddress();
                ((QS.Fx.Serialization.IStringSerializable)address).AsString = attributeName;
                return new QS._core_c_.Components.Attribute(attributeName, senderCollection[address]);
                // return (TMS.Inspection.IAttribute)senderCollection.lookup(new QS.Fx.Network.NetworkAddress(attributeName)); 
            }
		}

		#endregion

		#region IAttribute Members

		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Sender Collection"; }
		}

		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
/*
				foreach (QS.Fx.Network.NetworkAddress address in senderCollection.Keys)
				{					
					TMS.Base.IStatisticsCollector sender_asStatisticsCollector = 
						senderCollection[address] as TMS.Base.IStatisticsCollector;

					if (sender_asStatisticsCollector != null)
						statistics.Add(TMS.Base.StatisticsCollector.AsAttribute(address.ToString(), sender_asStatisticsCollector));
				}
*/ 
				return statistics;
			}
		}

		#endregion
	}

    [QS._core_c_.Diagnostics.ComponentContainer]
    public abstract class SenderCollection<K, C> : QS.Fx.Inspection.Inspectable, Base3_.ISenderCollection<K, C>
        where K : QS.Fx.Serialization.IStringSerializable, new()
        where C : QS._qss_c_.Base3_.ISerializableSender
    {
        public SenderCollection()
        {
        }

        protected abstract C CreateSender(K address);

        [QS._core_c_.Diagnostics.ComponentCollection]
        protected System.Collections.Generic.IDictionary<K, C> senders = new System.Collections.Generic.Dictionary<K, C>();

        [QS.Fx.Base.Inspectable("Senders", QS.Fx.Base.AttributeAccess.ReadOnly)]
        public QS.Fx.Inspection.IAttributeCollection Senders_AttributeCollection
        {
            get { return this; }
        }

        #region ISenderCollection<K, C> Members

        C QS._qss_c_.Base3_.ISenderCollection<K, C>.this[K address]
        {
            get
            {
                lock (this)
                {
                    if (senders.ContainsKey(address))
                        return senders[address];
                    else
                    {
                        C sender = this.CreateSender(address);
                        senders[address] = sender;
                        return sender;
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
                foreach (K address in senders.Keys)
                    yield return address.AsString;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get
            {
                try
                {
                    K address = new K();
                    address.AsString = attributeName;
                    return new QS.Fx.Inspection.ScalarAttribute(attributeName, senders[address]);
                }
                catch (Exception exc)
                {
                    throw new Exception("Could not find attribute \"" + attributeName + "\".", exc);
                }
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "SenderCollection"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
