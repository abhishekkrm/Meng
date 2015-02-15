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

using System.Windows.Forms;

namespace QS.GUI.Components
{
	public class ManagedNode : TreeNode, IManagedNode 
	{
		private static Resources.IImageCollection imageCollection = Resources.ImageCollection.Collection;

		public ManagedNode() : base(string.Empty)
		{
			ImageIndex = SelectedImageIndex = imageCollection[Resources.Image.DOCUMENT];
		}

		public ManagedNode(object associatedObject) : base()
		{
			((IManagedNode) this).AssociatedObject = associatedObject;
		}

#pragma warning disable 0649

		private string name;
		private object associatedObject, valueObject;

#pragma warning restore 0649

		#region Internal Stuff

		private void Initialize()
		{
			if (associatedObject is QS._qss_e_.Management_.IManagedComponent)
			{


			}


			// if (associatedObject

			// component -> log, subcomponents
			// attribute -> value, maybe children



		}

		#endregion

		#region IManagedNode Members

		void IManagedNode.RefreshValue()
		{
			// ................................................................................................................................................................
		}

		object IManagedNode.AssociatedObject
		{
			get { return associatedObject; }
			set
			{
				associatedObject = value;
				Initialize();
			}
		}

		string IManagedNode.Name
		{
			get { return name; }
		}

		System.Collections.Generic.IEnumerable<IManagedNode> IManagedNode.Children
		{
			get 
			{
				foreach (IManagedNode node in Nodes)
				{
					if (node != null)
						yield return node;
				}
			}
		}

		object IManagedNode.Value
		{
			get { return valueObject; }
		}

		#endregion
	}
}
