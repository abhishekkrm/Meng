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

namespace QS._qss_e_.Inspection_
{
	public class AttributeNode : System.Windows.Forms.TreeNode
	{
		#region Class ImageAssignment

		private static ImageAssignment defaultImageAssignment = new ImageAssignment(0, 0, 0, 0);
		public class ImageAssignment
		{
			public ImageAssignment(int scalar, int scalarSelected, int collection, int collectionSelected)
			{
				this.scalar = scalar;
				this.scalarSelected = scalarSelected;
				this.collection = collection;
				this.collectionSelected = collectionSelected;
			}

			public int scalar, scalarSelected, collection, collectionSelected;
		}

		#endregion

        public AttributeNode(QS.Fx.Inspection.IAttribute attribute)
            : this(attribute, defaultImageAssignment)
		{
		}

        public AttributeNode(QS.Fx.Inspection.IAttribute attribute, ImageAssignment imageAssignment)
            : base(attribute.Name)
		{
			this.attribute = attribute;
			this.imageAssignment = imageAssignment;

			switch (attribute.AttributeClass)
			{
                case QS.Fx.Inspection.AttributeClass.SCALAR:
				{
					ImageIndex = imageAssignment.scalar;
					SelectedImageIndex = imageAssignment.scalarSelected;
				}
				break;

                case QS.Fx.Inspection.AttributeClass.COLLECTION:
				{
					ImageIndex = imageAssignment.collection;
					SelectedImageIndex = imageAssignment.collectionSelected;
				}
				break;
			}

			// this.Rebuild();
		}

        private QS.Fx.Inspection.IAttribute attribute;
		private ImageAssignment imageAssignment;

		public bool Unroll()
		{
			return Unroll(false);
		}

		public bool Unroll(bool recursively)
		{
			bool unrolled = true;

			Nodes.Clear();
			switch (attribute.AttributeClass)
			{
                case QS.Fx.Inspection.AttributeClass.SCALAR:
				{
					object v = this.CachedValue;
					if (v != null)
					{
                        if (v is QS.Fx.Inspection.IAttribute)
                            Nodes.Add(new AttributeNode(v as QS.Fx.Inspection.IAttribute, imageAssignment));
						else if (v is QS.Fx.Inspection.IInspectable)
							Nodes.Add(new AttributeNode((v as QS.Fx.Inspection.IInspectable).Attributes, imageAssignment));
						else
							unrolled = false;
					}
				}
				break;

                case QS.Fx.Inspection.AttributeClass.COLLECTION:
				{
					foreach (string subitemName in ((attribute as QS.Fx.Inspection.IAttributeCollection).AttributeNames))
					{
						Nodes.Add(new AttributeNode((attribute as QS.Fx.Inspection.IAttributeCollection)[subitemName], imageAssignment));
					}
				}
				break;
			}

			if (recursively)
				foreach (AttributeNode childNode in Nodes)
					childNode.Unroll(true);

			return unrolled;
		}

		public string AsString
		{
			get 
			{
                if (attribute.AttributeClass == QS.Fx.Inspection.AttributeClass.SCALAR)
				{
					object v = this.CachedValue;
                    return (v == null || (v is QS.Fx.Inspection.IAttribute) || (v is QS.Fx.Inspection.IInspectable)) ? string.Empty : v.ToString();
				}
				else
					return string.Empty;
			}
		}

		public void Refresh()
		{
			cachedValue = null;
		}

		private object cachedValue = null;
		public object CachedValue
		{
			get 
			{ 
				if (cachedValue == null)
                    cachedValue = (attribute as QS.Fx.Inspection.IScalarAttribute).Value; 
				return cachedValue;
			}
		}

		public QS.Fx.Inspection.AttributeClass AttributeClass
		{
			get { return attribute.AttributeClass; }
		}

        public QS.Fx.Inspection.IAttribute Attribute
        {
            get { return attribute; }
        }
	}
}
