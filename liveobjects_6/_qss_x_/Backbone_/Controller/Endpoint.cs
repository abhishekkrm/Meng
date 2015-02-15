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

namespace QS._qss_x_.Backbone_.Controller
{
    public sealed class Endpoint : QS.Fx.Inspection.Inspectable, Scope.IEndpoint, IEndpointControl
    {
        #region Constructor

        public Endpoint(IControllerControl controller, Scope.IEndpointControl endpointcontrol)
        {
            this.controller = controller;
            this.endpointcontrol = endpointcontrol;
            endpointcontrol.Associate(this);

            _InitializeInspection();
        }

        #endregion

        #region Fields

        private IControllerControl controller;
        private Scope.IEndpointControl endpointcontrol;
        private IDictionary<Base1_.QualifiedID, _Channel> channels = new Dictionary<Base1_.QualifiedID, _Channel>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_channels")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, _Channel> __inspectable_channels;

        private void _InitializeInspection()
        {
            __inspectable_channels =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, _Channel>("_channels", channels,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, _Channel>.ConversionCallback(
                        delegate(string s) { return Base1_.QualifiedID.FromString(s); }));
        }

        #endregion

        #region IEndpointControl Members

        void IEndpointControl.Subscribe(ISubscriptionControl subscription)
        {
            lock (this)
            {
                bool subscribe_endpoint = false;

                _Channel channel;
                if (!channels.TryGetValue(subscription.Topic.ID, out channel))
                {
                    channel = new _Channel(this, (ITopicControl) subscription.Topic);
                    channels.Add(subscription.Topic.ID, channel);
                    subscribe_endpoint = true;
                }

                if (!channel.Subscriptions.ContainsKey(subscription.ID))
                    channel.Subscriptions.Add(subscription.ID, subscription);
                else
                    throw new Exception("Cannot add this subscription because such subscription already exists in this endpoint.");

                if (subscribe_endpoint)
                    endpointcontrol.Subscribe(subscription.Topic.ID);
            }
        }

        void IEndpointControl.Unsubscribe(ISubscriptionControl subscription)
        {
            lock (this)
            {
                _Channel channel;
                if (channels.TryGetValue(subscription.Topic.ID, out channel))
                {
                    if (channel.Subscriptions.ContainsKey(subscription.ID))
                    {
                        if (channel.Subscriptions.Count == 1)
                            endpointcontrol.Unsubscribe(subscription.Topic.ID);

                        channel.Subscriptions.Remove(subscription.ID);
                    }
                    else
                        throw new Exception("Cannot remove this subscription because no such subscription exists in this endpoint.");

                    if (channel.Subscriptions.Count == 0)
                        channels.Remove(subscription.Topic.ID);
                }
                else
                    throw new Exception("Cannot remove subscription because no subscriptions for this topic are registered in the endpoint.");
            }
        }

        #endregion

        #region Class _Channel

        private sealed class _Channel : QS.Fx.Inspection.Inspectable
        {
            #region Constructor

            internal _Channel(Endpoint endpoint, ITopicControl topic)
            {
                this.endpoint = endpoint;
                this.topic = topic;

                _InitializeInspection();
            }

            #endregion

            #region Fields

            private Endpoint endpoint;
            private ITopicControl topic;
            private IDictionary<QS.Fx.Base.ID, ISubscriptionControl> subscriptions = new Dictionary<QS.Fx.Base.ID, ISubscriptionControl>();

            #endregion

            #region Inspection

            [QS.Fx.Base.Inspectable("_subscriptions")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl> __inspectable_subscriptions;

            private void _InitializeInspection()
            {
                __inspectable_subscriptions =
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl>("_subscriptions", subscriptions,
                        new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl>.ConversionCallback(
                            delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));
            }

            #endregion

            #region Internal Interface

            internal IDictionary<QS.Fx.Base.ID, ISubscriptionControl> Subscriptions
            {
                get { return subscriptions; }
            }

            #endregion
        }

        #endregion
    }
}
