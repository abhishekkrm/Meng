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
    public sealed class ControllerRef : QS._qss_x_.Namespace_.IFolder
    {
        #region Constructor

        public ControllerRef(Controller.IController controller, QS._qss_x_.Namespace_.INamespaceControl nscontrol)
        {
            this.controller = controller;
            this.nscontrol = nscontrol;

            namespaceidentifier = nscontrol.NewIdentifier;
            nscontrol.Register(this);
            nscontrol.RootControl.Add(this);

            _Resynchronize();
        }

        #endregion

        #region Fields

        private Controller.IController controller;
        private QS._qss_x_.Namespace_.INamespaceControl nscontrol;
        private ulong namespaceidentifier;

        private IDictionary<QS.Fx.Base.ID, ProviderRef> providers = new Dictionary<QS.Fx.Base.ID, ProviderRef>();
        private IDictionary<QS.Fx.Base.ID, TopicRef> topics = new Dictionary<QS.Fx.Base.ID, TopicRef>();
        private IDictionary<QS.Fx.Base.ID, ClientRef> clients = new Dictionary<QS.Fx.Base.ID, ClientRef>();

        #endregion

        #region IFolder Members

        bool QS._qss_x_.Namespace_.IFolder.HasObjects
        {
            get 
            {
                lock (this)
                {
                    _Resynchronize();
                    return providers.Count > 0 || topics.Count > 0;
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
                    foreach (ProviderRef provider in providers.Values)
                        result.Add(provider);
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
            get { return QS._qss_x_.Namespace_.Category.Folder; }
        }

        string QS._qss_x_.Namespace_.IObject.Name
        {
            get { return "QuickSilver(\"" + controller.Name + ")"; }
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

        #region Internal Interface

        internal IEnumerable<ClientRef> CompatibleClients(Controller.ITopic topic)
        {
            List<ClientRef> _clients = new List<ClientRef>();
            IEnumerable<Controller.IClient> __clients = controller.CompatibleClients(topic);
            lock (this)
            {
                foreach (Controller.IClient client in __clients)
                {
                    ClientRef clientref;
                    if (!clients.TryGetValue(client.ID, out clientref))
                    {
                        clientref = new ClientRef(client, nscontrol.NewIdentifier);
                        nscontrol.Register(clientref);
                        clients.Add(client.ID, clientref);
                    }

                    _clients.Add(clientref);
                }
            }
            return _clients;
        }

        internal void Subscribe(Controller.ITopic topic, ulong clientidentifier)
        {
            QS._qss_x_.Namespace_.IObject _object;
            if (nscontrol.Lookup(clientidentifier, out _object))
            {
                ClientRef clientref = _object as ClientRef;
                if (clientref != null)
                {
                    controller.Subscribe(topic, clientref.Client);
                    _Resynchronize();
                }
                else
                    throw new Exception("No such client exists.");
            }
            else
                throw new Exception("No such client exists.");
        }

        internal void Unsubscribe(Controller.ISubscription subscription)
        {
            controller.Unsubscribe(subscription);
        }

        #endregion

        #region _Resynchronize

        private void _Resynchronize()
        {
            IEnumerable<Controller.IProvider> _providers = controller.Providers;
            List<QS.Fx.Base.ID> providerstoremove = new List<QS.Fx.Base.ID>(providers.Keys);
            foreach (Controller.IProvider provider in _providers)
            {
                if (!providerstoremove.Remove(provider.ID))
                {
                    ProviderRef providerref = new ProviderRef(this, provider, nscontrol, nscontrol.NewIdentifier);
                    nscontrol.Register(providerref);
                    providers.Add(provider.ID, providerref);
                }
            }
            foreach (QS.Fx.Base.ID providerid in providerstoremove)
                providers.Remove(providerid);

            IEnumerable<Controller.ITopic> _topics = controller.Topics;
            List<QS.Fx.Base.ID> topicstoremove = new List<QS.Fx.Base.ID>(topics.Keys);
            foreach (Controller.ITopic topic in _topics)
            {
                if (!topicstoremove.Remove(topic.ID.Object))
                {
                    TopicRef topicref = new TopicRef(this, topic, nscontrol, nscontrol.NewIdentifier);
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
