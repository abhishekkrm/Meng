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

namespace QS._qss_x_.Backbone_.Namespace
{
    public sealed class ProviderRef : QS._qss_x_.Namespace_.IFolder
    {
        #region Constructor

        public ProviderRef(ControllerRef controllerref, Controller.IProvider provider, QS._qss_x_.Namespace_.INamespaceControl nscontrol, ulong namespaceidentifier)
        {
            this.controllerref = controllerref;
            this.namespaceidentifier = namespaceidentifier;
            this.provider = provider;
            this.nscontrol = nscontrol;
        }

        #endregion

        #region Fields

        private ulong namespaceidentifier;
        private Controller.IProvider provider;
        private QS._qss_x_.Namespace_.INamespaceControl nscontrol;
        private ControllerRef controllerref;

        private IDictionary<QS.Fx.Base.ID, TopicRef> topics = new Dictionary<QS.Fx.Base.ID, TopicRef>();

        #endregion

        #region IFolder Members

        bool QS._qss_x_.Namespace_.IFolder.HasObjects
        {
            get 
            {
                lock (this)
                {
                    _Resynchronize();
                    return topics.Count > 0;
                }
            }
        }

        IEnumerable<QS._qss_x_.Namespace_.IObject> QS._qss_x_.Namespace_.IFolder.Objects
        {
            get
            {
                lock (this)
                {
                    _Resynchronize();
                    List<QS._qss_x_.Namespace_.IObject> result = new List<QS._qss_x_.Namespace_.IObject>();
                    foreach (TopicRef topic in topics.Values)
                        result.Add(topic);
                    return result;
                }
            }
        }

        #endregion

        #region IObject Members

        ulong QS._qss_x_.Namespace_.IObject.Identifier
        {
            get { return namespaceidentifier; }
        }

        QS._qss_x_.Namespace_.Category QS._qss_x_.Namespace_.IObject.Category
        {
            get { return QS._qss_x_.Namespace_.Category.Scope; }
        }

        string QS._qss_x_.Namespace_.IObject.Name
        {
            get { return provider.Name; }
        }

        bool QS._qss_x_.Namespace_.IObject.IsFolder
        {
            get { return true; }
        }

        IEnumerable<QS._qss_x_.Namespace_.IAction> QS._qss_x_.Namespace_.IObject.Actions
        {
            get { return new QS._qss_x_.Namespace_.IAction[0]; }
        }

        bool QS._qss_x_.Namespace_.IObject.Invoke(ulong identifier, ulong context)
        {
            return true;
        }

        #endregion

        #region _Resynchronize

        private void _Resynchronize()
        {
            IEnumerable<Controller.ITopic> _topics = provider.Topics;
            List<QS.Fx.Base.ID> topicstoremove = new List<QS.Fx.Base.ID>(topics.Keys);
            foreach (Controller.ITopic topic in _topics)
            {
                if (!topicstoremove.Remove(topic.ID.Object))
                {
                    TopicRef topicref = new TopicRef(controllerref, topic, nscontrol, nscontrol.NewIdentifier);
                    nscontrol.Register(topicref);
                    topics.Add(topic.ID.Object, topicref);
                }
            }
            foreach (QS.Fx.Base.ID topicid in topicstoremove)
                topics.Remove(topicid);
        }

        #endregion
    }
}
