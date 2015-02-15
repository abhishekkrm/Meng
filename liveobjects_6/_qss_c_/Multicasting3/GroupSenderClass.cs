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

namespace QS._qss_c_.Multicasting3
{
    public abstract class GroupSenderClass<SenderInterface, ImplementingClass> : QS.Fx.Inspection.Inspectable, IGroupSenderClass<SenderInterface>
        where ImplementingClass : SenderInterface where SenderInterface : IGroupSender 
    {
        private const int defaultAnticipatedNumberOfGroups = 20;

        public GroupSenderClass() : this(defaultAnticipatedNumberOfGroups)
        {
        }

        public GroupSenderClass(int anticipatedNumberOfGroups)
        {
            senderCollection = new QS._core_c_.Collections.Hashtable((uint) anticipatedNumberOfGroups);
			senderCollectionProxy = new GroupSenderClass<SenderInterface, ImplementingClass>.SenderCollectionProxy(this);
		}

		[QS.Fx.Base.Inspectable("SenderCollection", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected SenderCollectionProxy senderCollectionProxy;

		#region Proxy Inspection Collection for Senders

		protected class SenderCollectionProxy : QS.Fx.Inspection.IAttributeCollection
		{
			public SenderCollectionProxy(GroupSenderClass<SenderInterface, ImplementingClass> associatedSenderClass)
			{
				this.associatedSenderClass = associatedSenderClass;
			}

			private GroupSenderClass<SenderInterface, ImplementingClass> associatedSenderClass;

			#region IAttributeCollection Members

			IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
			{
				get
				{
					foreach (Base3_.GroupID groupID in associatedSenderClass.senderCollection.Keys)
						yield return groupID.ToUInt32.ToString();
				}
			}

			QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
			{
				get { return (QS.Fx.Inspection.IAttribute) associatedSenderClass[new Base3_.GroupID(Convert.ToUInt32(attributeName))]; }
			}

			#endregion

			#region IAttribute Members

			string QS.Fx.Inspection.IAttribute.Name
			{
				get { return "Senders"; }
			}

			QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
			}

			#endregion
		}

		#endregion

		protected QS._core_c_.Collections.Hashtable senderCollection;

        protected abstract ImplementingClass createSender(Base3_.GroupID groupID);

        private ImplementingClass lookup(Base3_.GroupID groupID)
        {
            lock (this)
            {
                QS._core_c_.Collections.IDictionaryEntry dic_en = senderCollection.lookupOrCreate(groupID);
                ImplementingClass sender = (ImplementingClass)dic_en.Value;
                if (dic_en.Value == null)
                {
                    sender = createSender(groupID);
                    dic_en.Value = sender;
                }

                return sender;
            }
        }

        public ImplementingClass this[Base3_.GroupID groupID]
        {
            get { return lookup(groupID); }
        }

        #region IGroupSenderClass Members

        SenderInterface IGroupSenderClass<SenderInterface>.this[Base3_.GroupID groupID]
        {
            get { return lookup(groupID); }
        }

        #endregion
	}
}
