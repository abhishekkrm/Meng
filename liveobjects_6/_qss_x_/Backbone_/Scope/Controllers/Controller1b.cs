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
#define OPTION_SanityChecking

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Scope.Controllers
{
/*
    /// <summary>
    /// A simple controller that creates a single recovery domain for the entire scope, for all topics simultaneously.
    /// </summary>
    public sealed class Controller1b : Controller, IController
    {
        #region Constructor

        public Controller1b(IControllerContext context) : base(context)
        {
        }

        #endregion

        #region Fields

        private ILocalDomain localdomain;
        private IDictionary<ITopic, _Topic> topics = new Dictionary<ITopic, _Topic>();
        private IDictionary<ISubScopeDomain, _Node> nodes = new Dictionary<ISubScopeDomain, _Node>();

        #endregion

        #region Class _Topic

        private sealed class _Topic
        {
            public _Topic(ITopic topic)
            {
                this.topic = topic;
            }

            public ITopic topic;
            public ICollection<ISubScopeDomain> nodes = new System.Collections.ObjectModel.Collection<ISubScopeDomain>();
        }

        #endregion

        #region Class _Node

        private sealed class _Node
        {
            public _Node(ISubScopeDomain domain)
            {
                this.domain = domain;
            }

            public ISubScopeDomain domain;
            public ICollection<ITopic> topics = new System.Collections.ObjectModel.Collection<ITopic>();
        }

        #endregion

        #region IController Members

        #region Register(topic)

        void IController.Register(ITopic topic)
        {
            if (topics.ContainsKey(topic))
                throw new Exception("Topic already registered.");                            
            topics.Add(topic, new _Topic(topic));

            if (localdomain == null)
                localdomain = context.CreateLocal("local");

            topic.Root = localdomain;
        }

        #endregion

        #region Register(domain)

        void IController.Register(ISubScopeDomain domain)
        {
            if (nodes.ContainsKey(domain))
                throw new Exception("Subdomain already registered.");                            
            nodes.Add(domain, new _Node(domain));
        }

        #endregion

        #region Register(domain,topic,membershiptype)

        void IController.Register(ISubScopeDomain domain, ITopic topic, MembershipType membershiptype)
        {
            _Topic _topic;
            if (topics.TryGetValue(topic, out _topic))
            {
                _Node _node;
                if (nodes.TryGetValue(domain, out _node))
                {
                    if (_node.topics.Contains(topic))
                        throw new Exception("This domain has already been registered for this topic.");

                    if (_topic.nodes.Contains(domain))
                        throw new Exception("This domain has already been registered for this topic.");

                    _topic.nodes.Add(domain);
                    _node.topics.Add(topic);
                                
                    // localdomain.Register(domain, membershiptype);
                }
                else
                    throw new Exception("Node not registered.");
            }
            else
                throw new Exception("Topic not registered.");
        }

        #endregion

        #region Unregister(domain,topic)

        void IController.Unregister(ISubScopeDomain domain, ITopic topic)
        {
            _Node _node;
            if (nodes.TryGetValue(domain, out _node))
            {
                if (_node.topics.Contains(topic))
                    throw new Exception("This domain has already been registered for this topic.");


                // nodes.Add(domain, _node);
            }
            else
                throw new Exception("This subdomain is not registered.");

            
            

                if (localdomain == null)
                    localdomain = context.CreateLocal("local");

                localdomain.Register(domain, membershiptype);
            }

            _Topic _topic;
            if (topics.TryGetValue(topic, out _topic))
            {
                if (_topic.nodes.Contains(domain))
                    throw new Exception("This domain has already been registered for this topic.");
            }
            else
            {
                _topic = new _Topic(topic);
                topics.Add(topic, _topic);
                topic.Root = localdomain;
            }

            _topic.nodes.Add(domain);
        }

        #endregion

        #region Unregister(domain)

        void IController.Unregister(ISubScopeDomain domain)
        {
            throw new NotImplementedException();
            // TODO: Implement this
        }

        #endregion

        #endregion
    }
*/ 
}
