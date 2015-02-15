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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Diagnostics_1_
{
    public class ComponentContainer<K, C> : QS._core_c_.Diagnostics.IComponentContainer where C : QS.Fx.Diagnostics.IDiagnosticsComponent
    {
        public ComponentContainer(IDictionary<K, C> dictionary)
        {
            this.dictionary = dictionary;
        }

        private bool enabled;
        private IDictionary<K, C> dictionary;

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.Container; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,IDiagnosticsComponent>> Members

        IEnumerator<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>>
            IEnumerable<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>>.GetEnumerator()
        {
            foreach (KeyValuePair<K, C> element in dictionary)
                yield return new KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>(
                    element.Key.ToString(), (QS.Fx.Diagnostics.IDiagnosticsComponent)element.Value);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent>>)this).GetEnumerator();
        }

        #endregion
    }
}
