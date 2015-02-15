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
    public abstract class SenderClass<SenderInterface> 
		: QS.Fx.Inspection.Inspectable, QS._qss_e_.Base_1_.IStatisticsCollector, ISenderClass<SenderInterface> 
        // , Diagnostics.IComponentContainer
    {
        public SenderClass()
        {
        }
        
        private ISenderCollection<SenderInterface> senderCollection = null;

		protected abstract SenderInterface createSender(QS.Fx.Network.NetworkAddress destinationAddress);

        #region ISenderCollection Members

        [QS._core_c_.Diagnostics.Component("Sender Collection")]
		public ISenderCollection<SenderInterface> SenderCollection
		{
            get
            {
                lock (this)
                {
                    if (senderCollection == null)
                    {
                        senderCollection = new Base3_.SenderCollection<SenderInterface>(
                            new Base3_.SenderCollection<SenderInterface>.CreateCallback(this.createSender));
                    }
                }

                return senderCollection;
            }
        }

        public SenderInterface CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
        {
            throw new NotImplementedException();
            // return this.createSender(destinationAddress);
        }

        #endregion

		#region IStatisticsCollector Members

        public IList<QS._core_c_.Components.Attribute> Statistics // QS.TMS.Base.IStatisticsCollector.Statistics
		{
			get 
			{ 
				QS._qss_e_.Base_1_.IStatisticsCollector sc = senderCollection as QS._qss_e_.Base_1_.IStatisticsCollector;
                return (sc != null) ? sc.Statistics : Helpers_.ListOf<QS._core_c_.Components.Attribute>.Nothing;
			}
		}

		#endregion

/*
        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.Container; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return true; }
            set { throw new NotSupportedException(); }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,IDiagnosticsComponent>> Members

        IEnumerator<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>> 
            IEnumerable<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>>.GetEnumerator()
        {
            ISenderCollection<SenderInterface> sc = this.SenderCollection;
            TMS.Inspection.IAttributeCollection asc = (TMS.Inspection.IAttributeCollection) sc;
            foreach (string name in asc.AttributeNames)
                yield return new KeyValuePair<string, IDiagnosticsComponent> ((TMS.Inspection.IScalarAttribute) asc[name]).Value
            foreach (SenderInterface s in sc)
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>>)this).GetEnumerator();
        }

        #endregion
*/ 
    }
}
