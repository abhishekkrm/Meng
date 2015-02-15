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

#define DEBUG_EnableLogging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Controller
{
    public sealed class Provider : QS.Fx.Inspection.Inspectable, IProviderControl
    {
        #region Constructor

        public Provider(QS.Fx.Base.ID id, string name, IControllerControl controller)
        {
            this.controller = controller;
            this.name = name;
            this.id = id;

            _InitializeInspection();
        }

        #endregion

        #region Fields

        private IControllerControl controller;
        [QS.Fx.Base.Inspectable] private string name;
        [QS.Fx.Base.Inspectable] private QS.Fx.Base.ID id;

        private IDictionary<QS.Fx.Base.ID, ITopicControl> topics = new Dictionary<QS.Fx.Base.ID, ITopicControl>();
        
        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_topics")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl> __inspectable_topics;

        private void _InitializeInspection()
        {
            __inspectable_topics =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl>("_topics", topics,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));
        }

        #endregion

        #region IProvider Members

        string IProvider.Name
        {
            get { return name; }
        }

        QS.Fx.Base.ID IProvider.ID
        {
            get { return id; }
        }

        IEnumerable<ITopic> IProvider.Topics
        {
            get 
            {
                lock (this)
                {
                    List<ITopic> result = new List<ITopic>();
                    foreach (ITopicControl topic in topics.Values)
                        result.Add(topic);
                    return result;
                }
            }
        }

        #endregion

        #region IProviderControl Members

        IDictionary<QS.Fx.Base.ID, ITopicControl> IProviderControl.Topics
        {
            get { return topics; }
        }

        #endregion
    }
}
