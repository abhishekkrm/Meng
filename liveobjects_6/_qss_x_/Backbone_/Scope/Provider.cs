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

namespace QS._qss_x_.Backbone_.Scope
{
    public sealed class Provider : Scope
    {
        #region Constructor

        public Provider(QS.Fx.Base.ID id, string name) : base(id, name)
        {
            _InitializeInspection();
        }

        #endregion

        #region Fields

        private Dictionary<QS.Fx.Base.ID, Topic> topics = new Dictionary<QS.Fx.Base.ID, Topic>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_topics")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic> __inspectable_topics;

        private void _InitializeInspection()
        {
            __inspectable_topics =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic>("_topics", topics,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));
        }

        #endregion

        #region Scope Overrides

        internal override ScopeType Type
        {
            get { return ScopeType.Super; }
        }

        #endregion

        // internal interface

        #region Internal Interface

        internal string Name
        {
            get { return name; }
        }

        internal IDictionary<QS.Fx.Base.ID, Topic> Topics
        {
            get { return topics; }
        }

        #endregion
    }
}
