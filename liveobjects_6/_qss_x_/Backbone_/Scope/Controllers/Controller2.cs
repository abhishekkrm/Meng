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

namespace QS._qss_x_.Backbone_.Scope.Controllers
{
    /// <summary>
    /// A simple controller without regions, and with a completely flat hierarchy (a single local domain per topic).
    /// </summary>
    public sealed class Controller2 : Controller, IController
    {
        #region Constructor

        public Controller2(IControllerContext context) : base(context)
        {
        }

        #endregion

        #region Fields

        private IDictionary<ITopic, _Topic> topics = new Dictionary<ITopic, _Topic>();

        #endregion

        #region Class _Topic

        private sealed class _Topic
        {
            public _Topic(ITopic topic, ILocalDomain localdomain)
            {
                this.topic = topic;
                this.localdomain = localdomain;
            }

            public ITopic topic;
            public ILocalDomain localdomain;
        }

        #endregion

        #region IController Members

        #region Register(topic)

        void IController.Register(ITopic topic)
        {
            if (topics.ContainsKey(topic))
                throw new Exception("Topic already registered.");

            _Topic _topic = new _Topic(topic, context.CreateLocal("topic(" + topic.Name + ")"));
            topics.Add(topic, _topic);
            topic.Root = _topic.localdomain;
        }

        #endregion

        #region Register(domain,topic,membershiptype)

        void IController.Register(IDomain domain, ITopic topic, MembershipType registrationtype)
        {
            _Topic _topic;
            if (topics.TryGetValue(topic, out _topic))
                _topic.localdomain.Register(domain, registrationtype);
            else
                throw new Exception("Topic not yet registered.");
        }

        #endregion

        #region Unregister(domain,topic)

        void IController.Unregister(IDomain domain, ITopic topic)
        {
            _Topic _topic;
            if (topics.TryGetValue(topic, out _topic))
                _topic.localdomain.Unregister(domain);
            else
                throw new Exception("Topic not yet registered.");
        }

        #endregion

        #region Unregister(topic)

        void IController.Unregister(ITopic topic)
        {
            _Topic _topic;
            if (topics.TryGetValue(topic, out _topic))
            {
                context.DeleteLocal(_topic.localdomain);
                topics.Remove(topic);
                topic.Root = null;
            }
            else
                throw new Exception("Topic net yet registered.");
        }

        #endregion

        #endregion
    }
}
