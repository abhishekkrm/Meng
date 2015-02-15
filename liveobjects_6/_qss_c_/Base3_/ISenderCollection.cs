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
    public interface ISenderCollection<AddressClass, SenderInterface> : QS.Fx.Inspection.IAttributeCollection
    {
        SenderInterface this[AddressClass destinationAddress]
        {
            get;
        }
    }

    public interface ISenderCollection<SenderInterface> : QS.Fx.Inspection.IAttributeCollection
    {
        SenderInterface this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get;
        }
    }

	public class SenderCollectionCast<InnerInterface, OuterInterface> 
		: ISenderCollection<OuterInterface> where InnerInterface : OuterInterface
	{
		public SenderCollectionCast(ISenderCollection<InnerInterface> actualCollection)
		{
			this.actualCollection = actualCollection;
		}

		private ISenderCollection<InnerInterface> actualCollection;

		public OuterInterface this[QS.Fx.Network.NetworkAddress destinationAddress]
		{
			get { return actualCollection[destinationAddress]; }
		}

		#region IAttributeCollection Members

		IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get { return actualCollection.AttributeNames; }
		}

		QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get { return actualCollection[attributeName]; }
		}

		#endregion

		#region IAttribute Members

		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return actualCollection.Name; }
		}

		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return actualCollection.AttributeClass; }
		}

		#endregion
	}
}
