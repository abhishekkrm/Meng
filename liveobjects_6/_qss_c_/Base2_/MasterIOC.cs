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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for MasterIOC.
	/// </summary>
	/// 
	public interface IMasterIOC : Collections_1_.ILinkable, IIdentifiableObjectContainer
	{
		void registerContainer(QS._qss_c_.Base2_.ContainerClass containerClass, Base2_.IIdentifiableObjectContainer container);
	}

	public class MasterIOC : Collections_1_.GenericLinkable, IMasterIOC 
	{
		private const uint defaultAnticipatedCapacityOfTheDefaultContainer	=	  1000;
		private const uint defaultAnticipatedNumberOfRegisteredContainers	=	    10;

		public MasterIOC() : this(defaultAnticipatedCapacityOfTheDefaultContainer, 
			defaultAnticipatedNumberOfRegisteredContainers)
		{
		}

		public MasterIOC(uint anticipatedCapacityOfTheDefaultContainer, 
			uint anticipatedNumberOfRegisteredContainers)
		{
			defaultContainer = new QS._qss_c_.Components_1_.IdentifiableObjectContainer(
				anticipatedCapacityOfTheDefaultContainer);
			registeredContainers = new QS._core_c_.Collections.Hashtable(anticipatedNumberOfRegisteredContainers);
		}

		private Components_1_.IdentifiableObjectContainer defaultContainer;
		private QS._core_c_.Collections.IDictionary registeredContainers;

		public void registerContainer(
			QS._qss_c_.Base2_.ContainerClass containerClass, Base2_.IIdentifiableObjectContainer container)
		{
			registeredContainers[containerClass] = container;
		}

		private Base2_.IIdentifiableObjectContainer lookupContainer(QS._qss_c_.Base2_.ContainerClass containerClass)
		{
			QS._core_c_.Collections.IDictionaryEntry dic_en = registeredContainers.lookup(containerClass);
			return (dic_en != null) ? ((Base2_.IIdentifiableObjectContainer) dic_en.Value) : defaultContainer;
		}

		#region IIdentifiableObjectContainer Members

		public void insert(IIdentifiableObject identifiableObject)
		{
			lookupContainer(identifiableObject.UniqueID.ContainerClass).insert(identifiableObject);
		}

		public IIdentifiableObject lookup(QS.ClassID classID, IIdentifiableKey uniqueID)
		{
			return lookupContainer(uniqueID.ContainerClass).lookup(classID, uniqueID);
		}

		public IIdentifiableObject remove(IIdentifiableObject identifiableObject)
		{
			return lookupContainer(identifiableObject.UniqueID.ContainerClass).remove(identifiableObject);
		}

		#endregion
	}
}
