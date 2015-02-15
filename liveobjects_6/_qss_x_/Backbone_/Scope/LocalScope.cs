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

#define OPTION_EnableLogging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Scope
{
    public sealed class LocalScope : Scope, IExternalControl, IControllerContext, ILocalScope, IDisposable
    {
        #region Constructor

        public LocalScope(IExternalControlContext context) : base(context.ID, context.Name)
        {
            this.logger = context.Logger;
            this.context = context;
            
            controller = new Controllers.Controller1a(this);
            endpointdomain = new EndpointDomain(new QS._qss_x_.Base1_.QualifiedID(id, QS.Fx.Base.ID.NewID()), name, this);

            _InitializeInspection();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Scope Overrides

        internal override ScopeType Type
        {
            get { return ScopeType.Local; }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IController controller;
        [QS.Fx.Base.Inspectable]
        private EndpointDomain endpointdomain;

        private QS.Fx.Logging.ILogger logger;
        private IExternalControlContext context;

        private Dictionary<QS.Fx.Base.ID, Topic> localtopics = new Dictionary<QS.Fx.Base.ID, Topic>();
        private Dictionary<QS.Fx.Base.ID, SubScope> subscopes = new Dictionary<QS.Fx.Base.ID, SubScope>();
        private Dictionary<QS.Fx.Base.ID, LocalDomain> localdomains = new Dictionary<QS.Fx.Base.ID, LocalDomain>();
        private Dictionary<QS.Fx.Base.ID, SuperScope> superscopes = new Dictionary<QS.Fx.Base.ID, SuperScope>();
        private Dictionary<QS.Fx.Base.ID, Provider> providers = new Dictionary<QS.Fx.Base.ID, Provider>();

        [QS.Fx.Base.Inspectable]
        private ICollection<LocalDomain> modifieddomains = new System.Collections.ObjectModel.Collection<LocalDomain>();

        #endregion

        #region Inspection

        #region inspectable wrappers

        [QS.Fx.Base.Inspectable("_localtopics")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic> __inspectable_localtopics;

        [QS.Fx.Base.Inspectable("_subscopes")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SubScope> __inspectable_subscopes;

        [QS.Fx.Base.Inspectable("_localdomains")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, LocalDomain> __inspectable_localdomains;

        [QS.Fx.Base.Inspectable("_superscopes")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SuperScope> __inspectable_superscopes;

        [QS.Fx.Base.Inspectable("_providers")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Provider> __inspectable_providers;

        #endregion

        #region _current_domain_hierarchy

        [QS.Fx.Base.Inspectable("_current_domain_hierarchy")]
        private QS._qss_x_.Inspection_.Hierarchy_.IHierarchyView _current_domain_hierarchy
        {
            get
            {
                lock (this)
                {
                    QS._qss_x_.Inspection_.Hierarchy_.HierarchyView hierarchyview = new QS._qss_x_.Inspection_.Hierarchy_.HierarchyView("domains");
                    
                    hierarchyview.Add(endpointdomain.ID, "local endpoint", "endpoint domain");
                    
                    foreach (SubScope subscope in subscopes.Values)
                    {
                        foreach (SubScopeDomain domain in subscope.Domains.Values)
                            hierarchyview.Add(domain.ID, domain.Scope.Name + " : " + 
                                ((domain.Name != null && domain.Name.Length > 0) ? domain.Name : domain.ID.Object.ToString()), 
                                "subscope domain");
                    }

                    foreach (LocalDomain domain in localdomains.Values)
                        hierarchyview.Add(domain.ID, "local : " + domain.Name, "local domain");

                    foreach (LocalDomain domain in localdomains.Values)
                    {
                        if (domain.View != null)
                        {
                            foreach (Membership membership in domain.View.Memberships)
                                hierarchyview.Link(membership.Member.ID, domain.ID, 
                                    ((membership.Member.Name != null && membership.Member.Name.Length > 0) ? membership.Member.Name : membership.Member.ID.ToString()) + 
                                    " in " + domain.Name, "membership");
                        }
                    }

                    return hierarchyview;
                }
            }
        }

        #endregion

        #region _comitted_domain_hierarchy

        [QS.Fx.Base.Inspectable("_comitted_domain_hierarchy")]
        private QS._qss_x_.Inspection_.Hierarchy_.IHierarchyView _comitted_domain_hierarchy
        {
            get
            {
                lock (this)
                {
                    QS._qss_x_.Inspection_.Hierarchy_.HierarchyView hierarchyview = new QS._qss_x_.Inspection_.Hierarchy_.HierarchyView("domains");

                    hierarchyview.Add(endpointdomain.ID, "local endpoint", "endpoint domain");

                    foreach (SubScope subscope in subscopes.Values)
                    {
                        foreach (SubScopeDomain domain in subscope.Domains.Values)
                            hierarchyview.Add(domain.ID, domain.Scope.ID.ToString() + " : " + domain.ID, "subscope domain");
                    }

                    foreach (LocalDomain domain in localdomains.Values)
                        hierarchyview.Add(domain.ID, "local : " + domain.Name, "local domain");

                    foreach (LocalDomain domain in localdomains.Values)
                    {
                        if (domain.ComittedView != null)
                        {
                            foreach (Membership membership in domain.ComittedView.Memberships)
                                hierarchyview.Link(membership.Member.ID, domain.ID,
                                    ((membership.Member.Name != null) ? membership.Member.Name : membership.Member.ID.ToString()) +
                                    " in " + domain.Name, "membership");
                        }
                    }

                    return hierarchyview;
                }
            }
        }

        #endregion

        #region _InitializeInspection

        private void _InitializeInspection()
        {
            __inspectable_localtopics =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic>("_localtopics", localtopics,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Topic>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_subscopes =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SubScope>("_subscopes", subscopes,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SubScope>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_localdomains =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, LocalDomain>("_localdomains", localdomains,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, LocalDomain>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_superscopes =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SuperScope>("_superscopes", superscopes,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, SuperScope>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));

            __inspectable_providers =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Provider>("_providers", providers,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Provider>.ConversionCallback(
                        delegate(string s) { return QS.Fx.Base.ID.FromString(s); }));
        }

        #endregion

        #endregion

        #region IExternalControl Members

        #region HierarchyController

        IController IExternalControl.HierarchyController
        {
            get { return controller; }
            set 
            {
                lock (this)
                {
                    if (controller != null)
                        throw new Exception("Controller is already defined; at this point we do not support reconfiguring controllers on the fly.");
                    controller = value;
                }
            }
        }

        #endregion

        #region EndpointControl

        IEndpointControl IExternalControl.EndpointControl
        {
            get { return endpointdomain; }
        }

        #endregion

        #region RegisterProvider(providerid, name)

        void IExternalControl.RegisterProvider(QS.Fx.Base.ID providerid, string name)
        {
            lock (this)
            {
                if (providers.ContainsKey(providerid))
                    throw new Exception("Provider with id = " + providerid.ToString() + "  is already registered.");

                Provider provider = new Provider(providerid, name);
                providers.Add(providerid, provider);
            }
        }

        #endregion

        #region UnregisterProvider(providerid)

        void IExternalControl.UnregisterProvider(QS.Fx.Base.ID providerid)
        {
            lock (this)
            {
                Provider provider;

                if (providers.TryGetValue(providerid, out provider))
                {
                    // TODO: We should unregister all topics managed by this provider............
                    providers.Remove(providerid);

                    _Commit();
                }
                else
                    throw new Exception("Provider with id = " + providerid.ToString() + "  is not yet registered.");
            }
        }

        #endregion

        #region RegisterTopic(topicid,name)

        void IExternalControl.RegisterTopic(Base1_.QualifiedID topicid, string name)
        {
            lock (this)
            {
                if (topicid.Context.Equals(id))
                {
                    if (localtopics.ContainsKey(topicid.Object))
                        throw new Exception("A local topic with id = " + topicid.Object.ToString() + " is already registered.");

                    Topic topic = new Topic(topicid, name, true, this, null);
                    localtopics.Add(topicid.Object, topic);
                }
                else
                {
                    Provider provider;
                    if (!providers.TryGetValue(topicid.Context, out provider))
                        throw new Exception("Provider with id = " + topicid.Context.ToString() + " has not been registered.");

                    if (provider.Topics.ContainsKey(topicid.Object))
                        throw new Exception("A topic with id = " + topicid.Object.ToString() + " is already registered in the context of provider with id = " + topicid.Context.ToString() + ".");

                    Topic topic = new Topic(topicid, name, false, this, provider);
                    provider.Topics.Add(topicid.Object, topic);
                }                
            }
        }

        #endregion

        #region UnregisterTopic(topicid)

        void IExternalControl.UnregisterTopic(Base1_.QualifiedID topicid)
        {
            lock (this)
            {
                if (topicid.Context.Equals(id))
                {
                    Topic topic;
                    if (!localtopics.TryGetValue(topicid.Object, out topic))
                        throw new Exception("A local topic with id = " + topicid.Object.ToString() + " has not been registered.");

                    _Unregister(topic);
                    localtopics.Remove(topicid.Object);
                }
                else
                {
                    Provider provider;
                    if (!providers.TryGetValue(topicid.Context, out provider))
                        throw new Exception("Provider with id = " + topicid.Context.ToString() + " has not been registered.");

                    Topic topic;
                    if (!provider.Topics.TryGetValue(topicid.Object, out topic))
                        throw new Exception("A topic with id = " + topicid.Object.ToString() + " has not been registered in the context of provider with id = " + topicid.Context.ToString() + ".");

                    _Unregister(topic);
                    provider.Topics.Remove(topicid.Object);
                }

                _Commit();
            }
        }

        #endregion

        #region RegisterSubscope

        void IExternalControl.RegisterSubscope(QS.Fx.Base.ID subscopeid, string name)
        {
            lock (this)
            {
                if (subscopes.ContainsKey(subscopeid))
                    throw new Exception("Subscope with id = " + subscopeid.ToString() + "  is already registered.");

                SubScope subscope = new SubScope(subscopeid, name);
                subscopes.Add(subscopeid, subscope);
            }
        }

        #endregion

        #region UnregisterSubscope(scopeid)

        void IExternalControl.UnregisterSubscope(QS.Fx.Base.ID subscopeid)
        {
            lock (this)
            {
                SubScope subscope;
                if (subscopes.TryGetValue(subscopeid, out subscope))
                {
                    foreach (SubScopeDomain domain in subscope.Domains.Values)
                    {
                        controller.Unregister(domain);
                        _UnregisterCleanup(domain);

                        foreach (Registration registration in domain.Registrations.Values)
                        {
                            registration.Topic.Registrations.Remove(domain.ID);
                            if (registration.Topic.Registrations.Count == 0)
                                controller.Unregister(registration.Topic);
                        }
                    }

                    subscopes.Remove(subscopeid);

                    _Commit();
                }
                else
                {
#if OPTION_EnableLogging
                    logger.Log("Cannot unregister: subscope " + subscopeid.ToString() + " is not currently registered with this scope.");
#endif
                }
            }
        }

        #endregion

        #region RegisterSubdomain(domainid,topicid,membershiptype)

        void IExternalControl.RegisterSubdomain(
            Base1_.QualifiedID domainid, string domainname, Base1_.QualifiedID topicid, MembershipType membershiptype)
        {
            if (membershiptype != MembershipType.Active)
                throw new Exception("Currently only active membership is supported.");

            lock (this)
            {
                SubScope subscope;
                if (!subscopes.TryGetValue(domainid.Context, out subscope))
                    throw new Exception("Cannot register topic: subscope " + domainid.Context.ToString() + " has not been registered.");

                SubScopeDomain subdomain;
                if (!subscope.Domains.TryGetValue(domainid.Object, out subdomain))
                {
                    subdomain = new SubScopeDomain(domainid, domainname, subscope);
                    subscope.Domains.Add(domainid.Object, subdomain);
                    controller.Register(subdomain);
                }

                _Register(subdomain, topicid, membershiptype);

                _Commit();
            }
        }

        #endregion

        #region UnregisterSubdomain(domainid,topicid)
        
        void IExternalControl.UnregisterSubdomain(QS._qss_x_.Base1_.QualifiedID domainid, Base1_.QualifiedID topicid)
        {
            lock (this)
            {
                SubScope subscope;
                if (!subscopes.TryGetValue(domainid.Context, out subscope))
                    throw new Exception("Cannot unregister subdomain: subscope " + domainid.Context.ToString() + 
                        " is not currently registered with this scope.");

                SubScopeDomain subdomain;
                if (!subscope.Domains.TryGetValue(domainid.Object, out subdomain))
                    throw new Exception("Cannot unregister subdomain: subdomain " + domainid.ToString() + 
                        " is not currently registered with this scope.");

                _Unregister(subdomain, topicid);

                // if (subscope.Domains.Count == 0)
                //    subscopes.Remove(domainid.Context);

                _Commit();
            }
        }

        #endregion

        #endregion

        #region IControllerContext Members

        ILocalDomain IControllerContext.CreateLocal(string name)
        {
            QS.Fx.Base.ID domainid = QS.Fx.Base.ID.NewID();
            LocalDomain domain = new LocalDomain(new Base1_.QualifiedID(id, domainid), name, this);
            localdomains.Add(domainid, domain);
            return domain;
        }

        void IControllerContext.DeleteLocal(ILocalDomain domain)
        {
            localdomains.Remove(domain.ID.Object);
        }

        #endregion

        // internal

        #region Internal Interface

        #region Subscribe

        internal void Subscribe(Base1_.QualifiedID topicid, MembershipType membershiptype)
        {
            if (membershiptype != MembershipType.Active)
                throw new Exception("Currently only active membership is supported.");

            lock (this)
            {
                _Register(endpointdomain, topicid, membershiptype);
                _Commit();
            }
        }

        #endregion

        #region Unsubscribe

        internal void Unsubscribe(Base1_.QualifiedID topicid)
        {
            lock (this)
            {
                _Unregister(endpointdomain, topicid);
                _Commit();
            }
        }

        #endregion

        #region Modified

        internal void Modified(LocalDomain localdomain)
        {
            lock (this)
            {
                if (!modifieddomains.Contains(localdomain))
                    modifieddomains.Add(localdomain);
            }
        }

        #endregion

        #region ForwardSubscribe

        internal void ForwardSubscribe(Domain domain, Topic topic)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(!topic.IsLocal);

                // for now, just hardcode active membership
                context.RegisterSubdomain(domain.ID.Object, domain.Name, topic.ID, MembershipType.Active);
            }
        }

        #endregion

        #region ForwardUnsubscribe

        internal void ForwardUnsubscribe(Domain domain, Topic topic)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(!topic.IsLocal);

                context.UnregisterSubdomain(domain.ID.Object, topic.ID);
            }
        }

        #endregion

        #endregion

        // private

        #region _Register(domain, topicid, membershiptype)

        private void _Register(Domain domain, Base1_.QualifiedID topicid, MembershipType membershiptype)
        {
            Topic topic;
            if (topicid.Context.Equals(id))
            {
                if (!localtopics.TryGetValue(topicid.Object, out topic))
                    throw new Exception("Currently topic discovery is not supported.");
            }
            else
            {
                Provider provider;
                if (providers.TryGetValue(topicid.Context, out provider))
                {
                    if (!provider.Topics.TryGetValue(topicid.Object, out topic))
                        throw new Exception("Currently topic discovery is not supported.");
                }
                else
                    throw new Exception("Currently provider discovery is not supported.");
            }

            Registration registration;
            if (!domain.Registrations.TryGetValue(topicid, out registration))
            {
                registration = new Registration(domain, topic, membershiptype);
                domain.Registrations.Add(topicid, registration);

                if (topic.Registrations.Count == 0)
                    controller.Register(topic);
                topic.Registrations.Add(domain.ID, registration);

                controller.Register(domain, topic, membershiptype);
            }
            else
            {
                if (registration.Type != membershiptype)
                {
                    // TODO: (later) We should support changing the registration type.
                    throw new NotImplementedException();
                }
                else
                {
#if OPTION_EnableLogging
                    logger.Log("Domain " + domain.ID.ToString() +
                        " is already registered with topic " + topicid.ToString() + " as " + membershiptype.ToString() + " .");
#endif
                }
            }
        }

        #endregion

        #region _Unregister(domain, topicid)

        private void _Unregister(Domain domain, Base1_.QualifiedID topicid)
        {
            Topic topic;

            if (topicid.Context.Equals(id))
            {
                if (!localtopics.TryGetValue(topicid.Object, out topic))
                    throw new Exception("Cannot unsubscribe: local topic " + topicid.Object.ToString() + " does not exist.");
            }
            else
            {
                Provider provider;
                if (!providers.TryGetValue(topicid.Context, out provider))
                    throw new Exception("Cannot unsubscribe: provider " + topicid.Context.ToString() + " is not known locally.");

                if (!provider.Topics.TryGetValue(topicid.Object, out topic))
                    throw new Exception("Cannot unsubscribe: topic " + topicid.ToString() + " is not known to exist at provider " +
                        topicid.Context.ToString() + ".");
            }

            Registration registration;
            if (domain.Registrations.TryGetValue(topic.ID, out registration))
            {
                domain.Registrations.Remove(topic.ID);
                controller.Unregister(domain, topic);

                if (domain.Registrations.Count == 0)
                {
                    controller.Unregister(domain);
                    _UnregisterCleanup(domain);
                }

                topic.Registrations.Remove(domain.ID);
                if (topic.Registrations.Count == 0)
                    controller.Unregister(topic);
            }
        }

        #endregion

        #region _Unregister(topic)

        private void _Unregister(Topic topic)
        {
            // TODO: When unregistering a topic, we should cancel all activities related to it.
        }

        #endregion

        #region _Commit

        private void _Commit()
        {
            foreach (LocalDomain domain in modifieddomains)
                domain.Commit();
            modifieddomains.Clear();
        }

        #endregion

        #region _UnregisterCleanup(domain)

        private void _UnregisterCleanup(IDomain domain)
        {
            // TODO: We should iterate over all uplinks of this domain and forcifully detach them from their parents.
        }

        #endregion
    }
}
