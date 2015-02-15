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
    public sealed class TopicRef : QS._qss_x_.Namespace_.IFolder
    {
        #region Constructor

        public TopicRef(ControllerRef controllerref, Controller.ITopic topic, QS._qss_x_.Namespace_.INamespaceControl nscontrol, ulong namespaceidentifier)
        {
            this.controllerref = controllerref;
            this.namespaceidentifier = namespaceidentifier;
            this.topic = topic;
            this.nscontrol = nscontrol;
        }

        #endregion

        #region Fields

        private ControllerRef controllerref;
        private Controller.ITopic topic;
        private QS._qss_x_.Namespace_.INamespaceControl nscontrol;
        private ulong namespaceidentifier;

        private IDictionary<QS.Fx.Base.ID, SubscriptionRef> subscriptions = new Dictionary<QS.Fx.Base.ID, SubscriptionRef>();

        #endregion

        #region IFolder Members

        bool QS._qss_x_.Namespace_.IFolder.HasObjects
        {
            get 
            {
                lock (this)
                {
                    _Resynchronize();
                    return subscriptions.Count > 0;
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
                    foreach (SubscriptionRef subscription in subscriptions.Values)
                        result.Add(subscription);
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
            get { return QS._qss_x_.Namespace_.Category.Topic; }
        }

        string QS._qss_x_.Namespace_.IObject.Name
        {
            get { return topic.Name; }
        }

        bool QS._qss_x_.Namespace_.IObject.IsFolder
        {
            get { return true; }
        }

        IEnumerable<QS._qss_x_.Namespace_.IAction> QS._qss_x_.Namespace_.IObject.Actions
        {
            get 
            {
                List<QS._qss_x_.Namespace_.IAction> actions = new List<QS._qss_x_.Namespace_.IAction>();
                foreach (ClientRef clientref in controllerref.CompatibleClients(topic))
                {
                    actions.Add(
                        new QS._qss_x_.Namespace_.Action(1, "subscribe using client \"" + ((QS._qss_x_.Namespace_.IObject)clientref).Name + "\"", 
                            ((QS._qss_x_.Namespace_.IObject)clientref).Identifier));
                }
                return actions; 
            }
        }

        bool QS._qss_x_.Namespace_.IObject.Invoke(ulong identifier, ulong context)
        {
            switch (identifier)
            {
                case 1:
                    controllerref.Subscribe(topic, context);
                    break;

                default:
                    break;
            }

            return true;
        }

        #endregion

        #region _Resynchronize

        private void _Resynchronize()
        {
            IEnumerable<Controller.ISubscription> _subscriptions = topic.Subscriptions;
            List<QS.Fx.Base.ID> subscriptionstoremove = new List<QS.Fx.Base.ID>(subscriptions.Keys);
            foreach (Controller.ISubscription subscription in _subscriptions)
            {
                if (!subscriptionstoremove.Remove(subscription.ID))
                {
                    SubscriptionRef subscriptionref = new SubscriptionRef(controllerref, subscription, nscontrol.NewIdentifier);
                    nscontrol.Register(subscriptionref);
                    subscriptions.Add(subscription.ID, subscriptionref);
                }
            }
            foreach (QS.Fx.Base.ID subscriptionid in subscriptionstoremove)
                subscriptions.Remove(subscriptionid);
        }

        #endregion
    }
}
