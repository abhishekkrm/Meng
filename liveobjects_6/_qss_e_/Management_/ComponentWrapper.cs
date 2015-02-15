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

namespace QS._qss_e_.Management_
{
	/// <summary>
	/// Summary description for ComponentWrapper.
	/// </summary>
	public class ComponentWrapper : QS.Fx.Inspection.Inspectable, IManagedComponent
	{
		public ComponentWrapper(object component, string name, QS._core_c_.Base.IOutputReader log, IManagedComponent[] subcomponents)
		{
			this.component = component;
			this.log = log;
			this.name = name;
			this.subcomponents = subcomponents;

			inspectableSubcomponents = new QS._qss_e_.Inspection_.Array("Subcomponents", subcomponents);
		}

		private string name;
		private QS._core_c_.Base.IOutputReader log;
		private object component;
		private IManagedComponent[] subcomponents;

		[QS.Fx.Base.Inspectable("Subcomponents", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Inspection_.Array inspectableSubcomponents;

		[QS.Fx.Base.Inspectable]
		public string CurrentLogContents
		{
			get { return log.CurrentContents; }
		}

		#region IManagedComponent Members

		public object Component
		{
			set { component = value; }
			get { return component; }
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public IManagedComponent[] Subcomponents
		{
			get
			{
				return subcomponents;
			}
		}

		public QS._core_c_.Base.IOutputReader Log
		{
			get
			{
				return this.log;
			}
		}

		#endregion
	}
}
