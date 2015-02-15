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

// #define Using_VS2005Beta1
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
	public abstract class GroupSender : QS.Fx.Inspection.Inspectable, IGroupSender
	{
        public GroupSender(Base3_.GroupID groupID)
        {
            this.groupID = groupID;
        }

		[QS.Fx.Base.Inspectable("GroupID", QS.Fx.Base.AttributeAccess.ReadOnly)]
        protected Base3_.GroupID groupID;

        #region IGroupSender Members

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public abstract Base3_.IAsynchronousOperation BeginSend(
            uint destinationLOID, QS.Fx.Serialization.ISerializable data, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState);

        public abstract void EndSend(Base3_.IAsynchronousOperation asynchronousOperation);

        public abstract int MTU
        {
            get;
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            return (obj is Base3_.GroupID) && groupID.Equals((Base3_.GroupID)obj) || (obj is IGroupSender) && groupID.Equals(((IGroupSender)obj).GroupID);
        }

        public override int GetHashCode()
        {
            return groupID.GetHashCode();
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return groupID.CompareTo(obj);
        }

        #endregion

        #region IComparable<GroupID> Members

        int IComparable<Base3_.GroupID>.CompareTo(Base3_.GroupID other)
        {
            return groupID.CompareTo(other);
        }

#if Using_VS2005Beta1
        bool IComparable<Base3.GroupID>.Equals(Base3.GroupID other)
        {
            return groupID.Equals(other);
        }
#endif

        #endregion

        #region IComparable<IGroupSender> Members

        int IComparable<IGroupSender>.CompareTo(IGroupSender other)
        {
            return groupID.CompareTo(other.GroupID);
        }

#if Using_VS2005Beta1
        bool IComparable<IGroupSender>.Equals(IGroupSender other)
        {
            return groupID.Equals(other.GroupID);
        }
#endif

        #endregion

/*
		#region IAttribute Members

		string QS.TMS.Inspection.IAttribute.Name
		{
			get { return groupID.ToUInt32.ToString(); }
		}

		QS.TMS.Inspection.AttributeClass QS.TMS.Inspection.IAttribute.AttributeClass
		{
			get { return QS.TMS.Inspection.AttributeClass.SCALAR; }
		}

		#endregion
*/
	}
}
