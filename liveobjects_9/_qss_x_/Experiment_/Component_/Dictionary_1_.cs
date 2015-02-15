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

#define DISABLED

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;

namespace QS._qss_x_.Experiment_.Component_
{
#if !DISABLED
    [QS.Fx.Reflection.ComponentClass("14A7BFA8F5C64CD29D864AB44079D764")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    public sealed class Dictionary_1_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IDictionary_,
        QS._qss_x_.Experiment_.Interface_.IDictionary_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Dictionary_1_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                    QS._qss_x_.Experiment_.Interface_.IDictionary_>(this);
        }

        public Dictionary_1_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                QS._qss_x_.Experiment_.Interface_.IDictionary_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, ICollection<string>> _dictionary = new Dictionary<string, ICollection<string>>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                QS._qss_x_.Experiment_.Interface_.IDictionary_>
                    QS._qss_x_.Experiment_.Object_.IDictionary_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Add(string _key, string _value)
        {
            lock (this)
            {
                ICollection<string> _values;
                if (!this._dictionary.TryGetValue(_key, out _values))
                {
                    _values = new Collection<string>();
                    this._dictionary.Add(_key, _values);
                }
                if (!_values.Contains(_value))
                    _values.Add(_value);
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Clear()
        {
            lock (this)
            {
                this._dictionary.Clear();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members


        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Get(string _key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
#endif
}
