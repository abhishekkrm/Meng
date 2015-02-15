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

namespace QS._qss_x_.Backbone_.Scope.Controllers
{
    /// <summary>
    /// A regional controller with precise, QSM-style regions of overlap.
    /// </summary>
    public sealed class Controller3 : Controller, IController
    {
        #region Constructor

        public Controller3(IControllerContext context) : base(context)
        {
        }

        #endregion

        #region Fields

        private IDictionary<IDomain, _Node> nodes = new Dictionary<IDomain, _Node>();
        private IDictionary<_Signature, _Region> regions = new Dictionary<_Signature, _Region>();
        private IDictionary<ITopic, _Topic> topics = new Dictionary<ITopic, _Topic>();
        
        #endregion

        #region Class _Topic

        private sealed class _Topic
        {
            public _Topic(ITopic topic, ILocalDomain domain)
            {
                this.topic = topic;
                this.domain = domain;
            }

            public ITopic topic;
            public ILocalDomain domain;
            public ICollection<_Region> regions = new System.Collections.ObjectModel.Collection<_Region>();

            #region System.Object Overrides

            public override bool Equals(object obj)
            {
                _Topic other = obj as _Topic;
                return (other != null) && topic.Equals(other.topic);
            }

            public override int GetHashCode()
            {
                return topic.GetHashCode();
            }

            public override string ToString()
            {
                return topic.ToString();
            }

            #endregion
        }

        #endregion

        #region Class _Signature

        private sealed class _Signature : IComparable<_Signature>, IComparable, IEquatable<_Signature>
        {
            public _Signature(ITopic[] topics)
            {
                this.topics = topics;
            }

            public ITopic[] topics;

            #region IComparable<_Signature> Members

            int IComparable<_Signature>.CompareTo(_Signature other)
            {
                int result = topics.Length.CompareTo(other.topics.Length);
                if (result == 0)
                {
                    for (int ind = 0; ind < topics.Length; ind++)
                    {
                        result = topics[ind].CompareTo(other.topics[ind]);
                        if (result == 0)
                            break;
                    }                    
                }
                return result;
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                _Signature other = obj as _Signature;
                if (other != null)
                    return ((IComparable<_Signature>)this).CompareTo(other);
                else
                    throw new Exception("The object of the comparison is either null or of a wrong type.");
            }

            #endregion

            #region IEquatable<_Signature> Members

            bool IEquatable<_Signature>.Equals(_Signature other)
            {
                if (topics.Length != other.topics.Length)
                    return false;

                for (int ind = 0; ind < topics.Length; ind++)
                {
                    if (!topics[ind].Equals(other.topics[ind]))
                        return false;
                }

                return true;
            }

            #endregion

            #region System.Object Overrides

            public override bool Equals(object obj)
            {
                _Signature other = obj as _Signature;
                if (other != null)
                    return ((IEquatable<_Signature>)this).Equals(other);
                else
                    throw new Exception("The object of the comparison is either null or of a wrong type.");
            }

            public override int GetHashCode()
            {
                int hashcode = topics.Length.GetHashCode();
                for (int ind = 0; ind < topics.Length; ind++)
                    hashcode ^= topics[ind].GetHashCode();
                return hashcode;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                for (int ind = 0; ind < topics.Length; ind++)
                {
                    if (ind > 0)
                        builder.Append(",");
                    builder.Append(topics[ind].Name);
                }
                return builder.ToString();
            }

            #endregion
        }

        #endregion

        #region Class _Region

        private sealed class _Region
        {
            public _Region(_Signature signature, ILocalDomain domain)
            {
                this.signature = signature;
                this.domain = domain;
            }

            public _Signature signature;
            public ICollection<_Topic> topics = new System.Collections.ObjectModel.Collection<_Topic>();
            public ILocalDomain domain;
            public ICollection<_Node> nodes = new System.Collections.ObjectModel.Collection<_Node>();
        }

        #endregion

        #region Class _Node

        private sealed class _Node
        {
            public _Node(IDomain domain)
            {
                this.domain = domain;
            }

            public IDomain domain;
            public _Region region;
        }

        #endregion

        #region _RegionalAdd

        private void _RegionalAdd(_Signature _signature, _Node _node)
        {
            _Region _region;
            if (!regions.TryGetValue(_signature, out _region))
            {
                _region = new _Region(_signature, context.CreateLocal("region(" + _signature.ToString() + ")"));
                regions.Add(_signature, _region);

                foreach (ITopic topic in _signature.topics)
                {
                    _Topic _topic;
                    if (!topics.TryGetValue(topic, out _topic))
                        throw new Exception("Topic not yet registered.");

                    _region.topics.Add(_topic);
                    _topic.regions.Add(_region);

                    _topic.domain.Register(_region.domain, MembershipType.Active);
                }
            }

            _node.region = _region;
            _region.nodes.Add(_node);

            _region.domain.Register(_node.domain, MembershipType.Active);
        }

        #endregion

        #region _RegionalRemove

        private void _RegionalRemove(_Node _node)
        {
            _node.region.nodes.Remove(_node);
            _node.region.domain.Unregister(_node.domain);

            if (_node.region.nodes.Count == 0)
            {
                foreach (_Topic _topic in _node.region.topics)
                {
                    _topic.regions.Remove(_node.region);
                    _topic.domain.Unregister(_node.region.domain);
                }

                context.DeleteLocal(_node.region.domain);
                regions.Remove(_node.region.signature);
            }

            _node.region = null;
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
            topic.Root = _topic.domain;            
        }

        #endregion

        #region Register(domain)

        void IController.Register(IDomain domain)
        {
            if (nodes.ContainsKey(domain))
                throw new Exception("Subdomain already registered.");

            nodes.Add(domain, new _Node(domain));
        }

        #endregion

        #region Register(domain,topic,membershiptype)

        void IController.Register(IDomain domain, ITopic topic, MembershipType membershiptype)
        {
            if (membershiptype != MembershipType.Active)
                throw new Exception("Currently, only active membership is supported.");

            _Topic _topic;
            if (!topics.TryGetValue(topic, out _topic))
                throw new Exception("Topic not yet registered.");

            _Node _node;
            if (!nodes.TryGetValue(domain, out _node))
                throw new Exception("Subdomain not yet registered.");

            _Signature _signature;
            if (_node.region != null)
            {
                ITopic[] _topics = new ITopic[_node.region.signature.topics.Length + 1];
                _topics[0] = topic;
                _node.region.signature.topics.CopyTo(_topics, 1);
                Array.Sort<ITopic>(_topics);
                _signature = new _Signature(_topics);

                _RegionalRemove(_node);
            }
            else
            {
                _signature = new _Signature(new ITopic[] { topic });
            }

            _RegionalAdd(_signature, _node);
        }

        #endregion

        #region Unregister(domain,topic)

        void IController.Unregister(IDomain domain, ITopic topic)
        {
            _Topic _topic;
            if (!topics.TryGetValue(topic, out _topic))
                throw new Exception("Topic not yet registered.");

            _Node _node;
            if (!nodes.TryGetValue(domain, out _node))
                throw new Exception("Subdomain not yet registered.");

            if (_node.region == null)
                throw new Exception("Subdomain not registered for any topics.");
            
            ITopic[] _topics = new ITopic[_node.region.signature.topics.Length - 1];
            int index = 0;
            foreach (ITopic signaturetopic in _node.region.signature.topics)
            {
                if (!signaturetopic.Equals(topic))
                {
                    if (index < _topics.Length)
                        _topics[index++] = signaturetopic;
                    else
                        throw new Exception("Subdomain not registered for this topic.");
                }
            }
            _Signature _signature = new _Signature(_topics);

            _RegionalRemove(_node);

            if (_signature.topics.Length > 0)
                _RegionalAdd(_signature, _node);
        }

        #endregion

        #region Unregister(domain)

        void IController.Unregister(IDomain domain)
        {
            _Node _node;
            if (nodes.TryGetValue(domain, out _node))
            {
                if (_node.region != null)
                    _RegionalRemove(_node);

                nodes.Remove(domain);
            }
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
                if (_topic.regions.Count != 0)
                    throw new Exception("Not all regions have been unregistered.");

                context.DeleteLocal(_topic.domain);
                topics.Remove(topic);
                topic.Root = null;
            }
            else
                throw new Exception("Topic not yet registered.");
        }

        #endregion

        #endregion
    }
}
