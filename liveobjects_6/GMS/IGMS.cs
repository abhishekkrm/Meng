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
using System.Collections;
using System.Net;
using System.IO;

namespace QS
{

	namespace GMS
	{
		/// <summary>
		/// Group Identifier class
		/// </summary>
		

		/// <summary>
		/// Delegate used for wrapping callback methods to be invoked
		/// whenever the membership of a group changes.
		/// Parameter is an ICollection of NodeId objects.
		/// </summary>
		public delegate void ViewChangeUpcall(GroupId gid, IView view);

		public delegate void ViewChangeRequest(GroupId gid, IView view);
		public delegate void ViewChangeGoAhead(GroupId gid, uint seqno);
		public delegate void ViewChangeAllDone(GroupId gid, uint seqno);
		public delegate void ViewChangeCleanup(GroupId gid);

		/// <summary>
		/// Interface for Local GMS representative used by entities to interact
		/// with the group membership service.
		/// </summary>
		public interface IGMS
		{
			/// <summary>
			/// Allows an entity with the passed in node identifier to join
			/// a group, and register an upcall to be invoked whenever the 
			/// membership of the group changes.
			/// </summary>
			/// <param name="gid">Identifier for group to join</param>
			/// <param name="nid">Identifier for joining node</param>
			/// <param name="vcu">Upcall to be invoked when membership of group changes</param>
			void joinGroup(GroupId gid, uint nid, ViewChangeUpcall vcu);

			/// <summary>
			/// Allows an entity with the passed in node identifier to leave
			/// a group.
			/// </summary>
			/// <param name="gid">Identifier for group to leave</param>
			/// <param name="nid">Identifier for leaving node</param>
			void leaveGroup(GroupId gid, uint nid);


			ViewChangeGoAhead linkCMSToGMS(ViewChangeRequest vcr, ViewChangeAllDone vcad, ViewChangeCleanup vcc);
		}
		
		public interface IFD
		{
            
		}
	}
}
