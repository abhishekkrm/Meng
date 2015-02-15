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
    public sealed class Controller : QS.Fx.Inspection.Inspectable, Node.IController, IControllerControl, Scope.IExternalControlContext
    {
        #region Constructor

        public Controller(Node.IControllerContext context, Configuration configuration)
        {
            this.context = context;

            localscope = new QS._qss_x_.Backbone_.Scope.LocalScope(this);

            endpoint = new Endpoint(this, ((Scope.IExternalControl) localscope).EndpointControl);

            // loading configuration

            if (configuration != null && configuration.Topics != null)
            {
                foreach (Configuration.Topic _topic in configuration.Topics)
                {
                    QS.Fx.Base.ID topicid = QS.Fx.Base.ID.NewID();
                    ITopicControl topic = new Topic(new Base1_.QualifiedID(context.ID, topicid), _topic.Name, true, this, null);
                    localtopics.Add(topicid, topic);
                    ((Scope.IExternalControl)localscope).RegisterTopic(topic.ID, topic.Name);
                }
            }

            // some bogus initialization code

            foreach (string name in new string[] { "Client1", "Client2" })
            {
                IClientControl client = new Client(QS.Fx.Base.ID.NewID(), name, this);
                clients.Add(client.ID, client);                
            }
            
            _InitializeInspection();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IDisposable) localscope).Dispose();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Node.IControllerContext context;

        [QS.Fx.Base.Inspectable]
        private IEndpointControl endpoint;

        [QS.Fx.Base.Inspectable]
        private Scope.LocalScope localscope;

        private IDictionary<QS.Fx.Base.ID, IConnectionControl> incoming = new Dictionary<QS.Fx.Base.ID, IConnectionControl>();
        private IDictionary<QS.Fx.Base.ID, IConnectionControl> outgoing = new Dictionary<QS.Fx.Base.ID, IConnectionControl>();
        private IDictionary<QS.Fx.Base.ID, IProviderControl> providers = new Dictionary<QS.Fx.Base.ID, IProviderControl>();
        private IDictionary<QS.Fx.Base.ID, ITopicControl> localtopics = new Dictionary<QS.Fx.Base.ID, ITopicControl>();
        private IDictionary<Base1_.QualifiedID, ITopicControl> remotetopics = new Dictionary<Base1_.QualifiedID, ITopicControl>();
        private IDictionary<QS.Fx.Base.ID, IClientControl> clients = new Dictionary<QS.Fx.Base.ID, IClientControl>();
        private IDictionary<QS.Fx.Base.ID, ISubscriptionControl> subscriptions = new Dictionary<QS.Fx.Base.ID, ISubscriptionControl>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_incoming")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl> __inspectable_incoming;
        [QS.Fx.Base.Inspectable("_outgoing")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl> __inspectable_outgoing;
        [QS.Fx.Base.Inspectable("_providers")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IProviderControl> __inspectable_providers;
        [QS.Fx.Base.Inspectable("_localtopics")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl> __inspectable_localtopics;
        [QS.Fx.Base.Inspectable("_remotetopics")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, ITopicControl> __inspectable_remotetopics;
        [QS.Fx.Base.Inspectable("_clients")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IClientControl> __inspectable_clients;
        [QS.Fx.Base.Inspectable("_subscriptions")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl> __inspectable_subscriptions;

        private void _InitializeInspection()
        {
            __inspectable_incoming =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl>("_incoming", incoming,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_outgoing =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl>("_outgoing", outgoing,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IConnectionControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_providers =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IProviderControl>("_providers", providers,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IProviderControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_localtopics =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl>("_localtopics", localtopics,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ITopicControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_remotetopics =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, ITopicControl>("_remotetopics", remotetopics,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, ITopicControl>.ConversionCallback(
                        delegate(string s) { return Base1_.QualifiedID.FromString(s); }));

            __inspectable_clients =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IClientControl>("_clients", clients,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, IClientControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_subscriptions =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl>("_subscriptions", subscriptions,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, ISubscriptionControl>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));
        }

        #endregion

        #region Node.IController Members

        Node.IControllerConnection Node.IController.Create(Node.IControllerConnectionContext context)
        {
            Connection connection = new Connection(this, context);
            lock (this)
            {                
                (context.IsSuper ? outgoing : incoming).Add(context.ID, connection);
            }
            return connection;
        }

        #endregion

        #region IController Members

        #region Name

        string IController.Name
        {
            get { return context.Name; }
        }

        #endregion

        #region ID

        QS.Fx.Base.ID IController.ID
        {
            get { return context.ID; }
        }

        #endregion

        #region CompatibleClients

        IEnumerable<IClient> IController.CompatibleClients(ITopic _topic)
        {
            ITopicControl topic = _topic as ITopicControl;
            if (topic != null)
            {
                lock (this)
                {
                    if (topic.IsLocal && localtopics.ContainsKey(topic.ID.Object) || remotetopics.ContainsKey(topic.ID))
                    {
                        List<IClient> result = new List<IClient>();
                        foreach (IClientControl client in clients.Values)
                        {
                            if (_Compatible(client, topic))
                                result.Add(client);
                        }
                        return result;
                    }
                    else
                        throw new Exception("No such topic exists.");
                }
            }
            else
                throw new Exception("The topic passed as argument IS not managed by this controller.");
        }

        #endregion

        #region Subscribe(topic,client)

        void IController.Subscribe(ITopic _topic, IClient _client)
        {
            ((IController)this).BeginSubscribe(_topic, _client, null, null);
        }

        #endregion

        #region BeginSubscribe(topic,client,callback,context)

        IAsyncResult IController.BeginSubscribe(ITopic _topic, IClient _client, AsyncCallback callback, object callbackcontext)
        {
            ITopicControl topic = _topic as ITopicControl;
            IClientControl client = _client as IClientControl;

            if (topic != null && client != null)
            {
                lock (this)
                {
                    if ((topic.IsLocal && localtopics.ContainsKey(topic.ID.Object) || remotetopics.ContainsKey(topic.ID)) && clients.ContainsKey(client.ID))
                    {
                        ISubscriptionControl subscription = new Subscription(this, QS.Fx.Base.ID.NewID(), client, topic);
                        subscriptions.Add(subscription.ID, subscription);

                        lock (topic)
                        {
                            topic.Subscriptions.Add(subscription.ID, subscription);
                        }

                        lock (client)
                        {
                            client.Subscriptions.Add(subscription.ID, subscription);
                        }

                        QS._qss_c_.Base3_.AsyncResult<ISubscriptionControl> request =
                            new QS._qss_c_.Base3_.AsyncResult<ISubscriptionControl>(callback, callbackcontext, subscription);

                        context.Scheduler.BeginExecute(new AsyncCallback(this._Subscribe), request);

                        return request;
                    }
                    else
                        throw new Exception("Either the topic or the client is no longer registered at this scope.");
                }
            }
            else
                throw new Exception("The topic and client passed as arguments are not managed by this controller.");
        }

        private void _Subscribe(IAsyncResult _request)
        {
            lock (this)
            {
                QS._qss_c_.Base3_.AsyncResult<ISubscriptionControl> request = 
                    (QS._qss_c_.Base3_.AsyncResult<ISubscriptionControl>) _request.AsyncState;
                context.Scheduler.EndExecute(_request);

                ISubscriptionControl subscription = request.Context;
                try
                {
                    _StartSubscription(subscription);
                    request.Completed(false, true, null);
                }
                catch (Exception exc)
                {
                    request.Completed(false, false, exc);
                }
            }
        }

        #endregion

        #region EndSubscribe(request)

        ISubscription IController.EndSubscribe(IAsyncResult request)
        {
            return ((QS._qss_c_.Base3_.AsyncResult<ISubscriptionControl>) request).Context;
        }

        #endregion

        #region Unsubscribe (subscription)

        void IController.Unsubscribe(ISubscription _subscription)
        {
            ISubscriptionControl subscription = _subscription as ISubscriptionControl;
            if (subscription != null)
                context.Scheduler.BeginExecute(new AsyncCallback(this._Unsubscribe), subscription);
        }

        private void _Unsubscribe(IAsyncResult _request)
        {
            lock (this)
            {
                ISubscriptionControl subscription = (ISubscriptionControl) _request.AsyncState;
                context.Scheduler.EndExecute(_request);

                try
                {
                    if (subscriptions.ContainsKey(subscription.ID))
                    {
                        _StopSubscription(subscription);

                        lock (subscription.Client)
                        {
                            ((IClientControl)subscription.Client).Subscriptions.Remove(subscription.ID);
                        }

                        lock (subscription.Topic)
                        {
                            ((ITopicControl)subscription.Topic).Subscriptions.Remove(subscription.ID);
                        }

                        subscriptions.Remove(subscription.ID);
                    }
                    else
                        throw new Exception("No such subscription could be found.");
                }
                catch (Exception exc)
                {
                    // for now, do nothing...........
                }
            }
        }

        #endregion

        #region Providers

        IEnumerable<IProvider> IController.Providers
        {
            get
            {
                lock (this)
                {
                    List<IProvider> result = new List<IProvider>();
                    foreach (IProviderControl provider in providers.Values)
                        result.Add(provider);
                    return result;
                }
            }
        }

        #endregion

        #region Topics

        IEnumerable<ITopic> IController.Topics
        {
            get
            {
                lock (this)
                {
                    List<ITopic> result = new List<ITopic>();
                    foreach (ITopicControl topic in localtopics.Values)
                        result.Add(topic);
                    return result;
                }
            }
        }

        #endregion

        #endregion

        #region IControllerControl Members

        #region ActivateChannel(connection)

        void IControllerControl.ActivateChannel(IConnectionControl connection)
        {
            lock (this)
            {
                lock (connection)
                {
                    if (connection.IsActivated)
                        throw new Exception("Connection \"" + connection.Name + "\" is already activated.");

                    if (!connection.Context.IsSuper)
                    {
                        List<ProviderUpdate> providerupdates = new List<ProviderUpdate>();
                        foreach (IProviderControl provider in providers.Values)
                        {
                            List<TopicUpdate> topicupdates = new List<TopicUpdate>();
                            foreach (ITopicControl topic in provider.Topics.Values)
                                topicupdates.Add(new TopicUpdate(topic.Name, topic.ID.Object, UpdateType.Add));

                            providerupdates.Add(
                                new ProviderUpdate(provider.Name, provider.ID, UpdateType.Add, topicupdates.ToArray()));
                        }

                        List<TopicUpdate> mytopicupdates = new List<TopicUpdate>();
                        foreach (ITopicControl topic in localtopics.Values)
                            mytopicupdates.Add(new TopicUpdate(topic.Name, topic.ID.Object, UpdateType.Add));

                        providerupdates.Add(
                            new ProviderUpdate(context.Name, context.ID, UpdateType.Add, mytopicupdates.ToArray()));

                        Update update = new Update(providerupdates.ToArray());
                            
                        connection.Context.RequestChannel.Submit(update, QS._qss_x_.Backbone_.Node.MessageOptions.None, null, null);

                        ((Scope.IExternalControl) localscope).RegisterSubscope(connection.ID, connection.Name);
                    }

                    connection.IsActivated = true;
                }
            }
        }

        #endregion

        #region Handle(connection,message)

        void IControllerControl.Handle(IConnectionControl connection, Node.IIncoming message)
        {
            lock (this)
            {
                switch (message.Message.SerializableInfo.ClassID)
                {
                    case ((ushort)ClassID.Fx_Backbone_Controller_Update):
                        _Update(connection, (Update) message.Message);
                        break;

                    case ((ushort) ClassID.Fx_Backbone_Controller_Register):
                        _Register(connection, (Register) message.Message);
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Scope.IExternalControlContext Members

        #region ID

        QS.Fx.Base.ID Scope.IExternalControlContext.ID
        {
            get { return context.ID; }
        }

        #endregion

        #region Name

        string Scope.IExternalControlContext.Name
        {
            get { return context.Name; }
        }

        #endregion

        #region Logger

        QS.Fx.Logging.ILogger Scope.IExternalControlContext.Logger
        {
            get { return context.Logger; }
        }

        #endregion

        #region RegisterSubdomain

        void Scope.IExternalControlContext.RegisterSubdomain(
            QS.Fx.Base.ID localdomainid, string localdomainname, Base1_.QualifiedID topicid, Scope.MembershipType registrationtype)
        {
            lock (this)
            {
                if (topicid.Context.Equals(context.ID))
                    throw new Exception("Cannot register subdomain; this is a local topic and it should have been handled within this scope.");

                IProviderControl provider;
                if (providers.TryGetValue(topicid.Context, out provider))
                {
                    ITopicControl topic;
                    if (provider.Topics.TryGetValue(topicid.Object, out topic))
                    {
                        IEnumerator<KeyValuePair<QS.Fx.Base.ID, IConnectionControl>> conn_enum = topic.Connections.GetEnumerator();
                        conn_enum.Reset();
                        if (!conn_enum.MoveNext())
                            throw new Exception("Topic does not have any active connections associated with it.");

                        IConnectionControl connection = conn_enum.Current.Value;
                        lock (connection)
                        {
                            if (!connection.IsActivated)
                                throw new Exception("Connection is not activated, cannot send....");

                            connection.Context.RequestChannel.Submit(
                                new Register(localdomainid, localdomainname, topicid.Context, topicid.Object, registrationtype), 
                                QS._qss_x_.Backbone_.Node.MessageOptions.None, null, null);
                        }
                    }
                    else
                        throw new Exception("No such topic exists.");
                }
                else
                    throw new Exception("No such provider exists.");
            }
        }

        #endregion

        #region UnregisterSubdomain

        void Scope.IExternalControlContext.UnregisterSubdomain(QS.Fx.Base.ID localdomainid, Base1_.QualifiedID topicid)
        {
            throw new NotImplementedException();

/*
            lock (this)
            {

                IProviderControl provider;
                if (providers.TryGetValue(topicid.Context, out provider))
                {
                    ITopicControl topic;
                    if (provider.Topics.TryGetValue(topicid.Object, out topic))
                    {
                        IEnumerator<KeyValuePair<Base.Name, IConnectionControl>> conn_enum = topic.Connections.GetEnumerator();
                        conn_enum.Reset();
                        if (!conn_enum.MoveNext())
                            throw new Exception("Topic does not have any active connections associated with it.");

                        IConnectionControl connection = conn_enum.Current.Value;
                        lock (connection)
                        {
                            if (!connection.IsActivated)
                                throw new Exception("Connection is not activated, cannot send....");

                            connection.Context.RequestChannel.Submit(
                                new Register(localdomainid, topicid.Context, topicid.Object, registrationtype),
                                QS.Fx.Backbone.Node.MessageOptions.None, null, null);
                        }
                    }
                    else
                        throw new Exception("No such topic exists.");
                }
                else
                    throw new Exception("No such provider exists.");
            }
*/
        }

        #endregion

        #endregion

        // private members

        #region _Update

        private void _Update(IConnectionControl connection, Update update)
        {
            foreach (ProviderUpdate providerupdate in update.Providers)
            {
                switch (providerupdate.UpdateType)
                {
                    case UpdateType.Add:
                        {
                            if (providers.ContainsKey(providerupdate.ID))
                                context.Logger.Log("Cannot add provider " + providerupdate.ID.ToString() + " because it already exists.");
                            else
                            {
                                IProviderControl provider = new Provider(providerupdate.ID, providerupdate.Name, this);
                                providers.Add(providerupdate.ID, provider);
                                ((Scope.IExternalControl)localscope).RegisterProvider(providerupdate.ID, providerupdate.Name);

#if DEBUG_EnableLogging
                                context.Logger.Log("Added a new provider : \"" + provider.Name + "\"");
#endif

                                foreach (TopicUpdate topicupdate in providerupdate.Topics)
                                {
                                    if (topicupdate.UpdateType == UpdateType.Add)
                                    {
                                        ITopicControl topic = new Topic(
                                            new Base1_.QualifiedID(providerupdate.ID, topicupdate.ID), topicupdate.Name, false, this, provider);
                                        provider.Topics.Add(topicupdate.ID, topic);
                                        remotetopics.Add(topic.ID, topic);
                                        topic.Connections.Add(connection.ID, connection);
                                        ((Scope.IExternalControl) localscope).RegisterTopic(topic.ID, topic.Name);

#if DEBUG_EnableLogging
                                        context.Logger.Log("Added in provider \"" + provider.Name + "\" a new topic \"" + topic.Name + "\"");
#endif
                                    }
                                    else
                                        context.Logger.Log("Cannot update a topic in a newly added provider.");
                                }
                            }
                        }
                        break;

                    default:
                        {
                            context.Logger.Log("Cannot process update because it is not implemented.\n" +
                                QS.Fx.Printing.Printable.ToString(update));
                        }
                        break;
                }
            }

            foreach (IConnectionControl incomingconnection in incoming.Values)
            {
                lock (incomingconnection)
                {
                    if (incomingconnection.IsActivated)
                        incomingconnection.Context.RequestChannel.Submit(update, QS._qss_x_.Backbone_.Node.MessageOptions.None, null, null);
                }
            }
        }

        #endregion

        #region _Compatible

        private bool _Compatible(IClientControl client, ITopicControl topic)
        {
            return true;
        }

        #endregion

        #region _StartSubscription

        private void _StartSubscription(ISubscriptionControl subscription)
        {
            context.Logger.Log("_StartSubscription : " + subscription.ID);
            endpoint.Subscribe(subscription);
        }

        #endregion

        #region _StopSubscription

        private void _StopSubscription(ISubscriptionControl subscription)
        {
            context.Logger.Log("_StopSubscription : " + subscription.ID);
            endpoint.Unsubscribe(subscription);
        }

        #endregion

        #region _Register

        private void _Register(IConnectionControl connection, Register register)
        {
            ((Scope.IExternalControl) localscope).RegisterSubdomain(new Base1_.QualifiedID(connection.ID, register.DomainID),
                register.DomainName, new Base1_.QualifiedID(register.ProviderID, register.TopicID), register.RegistrationType);
        }

        #endregion
    }
}
