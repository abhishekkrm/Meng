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

// #define DEBUG_KeepRemoved

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Components_1_
{
	public class SeqCollection<C> : QS.Fx.Inspection.Inspectable, ISeqCollection<C> where C : class
	{
		public delegate C Constructor(int seqno);

		public static readonly Base3_.Constructor<ISeqCollection<C>> CollectionConstructor =
			new Base3_.Constructor<ISeqCollection<C>>(createCollection);
		private static ISeqCollection<C> createCollection()
		{
			return new SeqCollection<C>();
		}

		public SeqCollection() : this(null)
		{		
		}

		public SeqCollection(Constructor constructorCallback)
		{
			this.constructorCallback = constructorCallback;

			inspectableElementsProxy = 
				new QS._qss_e_.Inspection_.DictionaryWrapper1<int, SeqCollection<C>.MyElement>("Elements", elements, 
				new QS._qss_e_.Inspection_.DictionaryWrapper1<int, SeqCollection<C>.MyElement>.ConversionCallback(
				Helpers_.FromString.Int32));

#if DEBUG_KeepRemoved
			inspectableRemovedElementsProxy = 
				new QS.TMS.Inspection.DictionaryWrapper1<int, C>("Removed Elements", removed_elements,
				new QS.TMS.Inspection.DictionaryWrapper1<int, C>.ConversionCallback(Helpers.FromString.Int32));
#endif
		}

		private class MyElement : QS.Fx.Inspection.IScalarAttribute
		{
			public MyElement(C element)
			{
				this.element = element;
			}

			public C element;
			public bool removed = false;

			#region IScalarAttribute Members

			object QS.Fx.Inspection.IScalarAttribute.Value
			{
				get { return element; }
			}

			#endregion

			#region IAttribute Members

			string QS.Fx.Inspection.IAttribute.Name
			{
				get { return (element != null) ? element.GetType().Name : "(null)"; }
			}

			QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
			}

			#endregion
		}

		private Constructor constructorCallback;
		private System.Collections.Generic.IDictionary<int, MyElement> elements = 
			new  System.Collections.Generic.Dictionary<int, MyElement>();

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		private int maxremoved = 0;

		[QS.Fx.Base.Inspectable("Elements", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper1<int, MyElement> inspectableElementsProxy;

#if DEBUG_KeepRemoved
		private System.Collections.Generic.IDictionary<int, C> removed_elements =
			new System.Collections.Generic.Dictionary<int, C>();
		[TMS.Inspection.Inspectable("Removed Elements", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private TMS.Inspection.DictionaryWrapper1<int, C> inspectableRemovedElementsProxy;
#endif

		[QS.Fx.Base.Inspectable]
		public int Count
		{
			get { return elements.Count; }
		}

		#region ISeqCollection Members

		Constructor ISeqCollection<C>.ConstructorCallback
		{
			set { constructorCallback = value; }
		}

		C ISeqCollection<C>.lookup(int seqno)
		{
			lock (this)
			{
				if (seqno > maxremoved)
				{
					if (elements.ContainsKey(seqno))
					{
						MyElement e = elements[seqno];
						return e.removed ? null : e.element;
					}
					else
					{
						C element = constructorCallback(seqno);
						elements[seqno] = new MyElement(element);
						return element;
					}
				}
				else
					return null;
			}
		}

		C ISeqCollection<C>.remove(int seqno)
		{
			lock (this)
			{
				if (seqno > maxremoved)
				{
					if (elements.ContainsKey(seqno))
					{
						MyElement wrapped_element = elements[seqno];
						if (wrapped_element.removed)
							return null;
						else
						{
							if (seqno == maxremoved + 1)
							{
								do
								{
									maxremoved++;
									elements.Remove(seqno);
								}
								while (elements.ContainsKey(seqno = (maxremoved + 1)) && elements[seqno].removed);
							}
							else
								wrapped_element.removed = true;

							C element = wrapped_element.element;
							wrapped_element.element = null;

#if DEBUG_KeepRemoved
							removed_elements[seqno] = element;
#endif

							return element;
						}
					}
					else
						return null;
				}
				else
					return null;
			}
		}

		#endregion
	}
}
