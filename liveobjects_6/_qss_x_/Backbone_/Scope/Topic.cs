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

namespace QS._qss_x_.Backbone_.Scope
{
    public sealed class Topic : QS.Fx.Inspection.Inspectable, ITopic
    {
        #region Constructor

        public Topic(Base1_.QualifiedID qualifiedid, string name, bool islocal, LocalScope localscope, Provider provider)
        {
            if (islocal)
            {
                if (provider != null)
                    throw new Exception("Topic is marked as local, but provider is not null.");
            }
            else
            {
                if (provider == null)
                    throw new Exception("Topic is marked as not local, but the provider is null.");
                if (!qualifiedid.Context.Equals(provider.ID))
                    throw new Exception("The topic doesn't come from the specified provider.");
            }

            this.qualifiedid = qualifiedid;
            this.name = name;
            this.islocal = islocal;
            this.localscope = localscope;
            this.provider = provider;

            _InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private Base1_.QualifiedID qualifiedid;
        [QS.Fx.Base.Inspectable]
        private string name;
        [QS.Fx.Base.Inspectable]
        private Domain root;
        [QS.Fx.Base.Inspectable]
        private bool islocal;
        [QS.Fx.Base.Inspectable]
        private Provider provider;

        private LocalScope localscope;
        private Dictionary<Base1_.QualifiedID, Registration> registrations = new Dictionary<Base1_.QualifiedID, Registration>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_registrations")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration> __inspectable_registrations;

        private void _InitializeInspection()
        {
            __inspectable_registrations =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration>("_registrations", registrations,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<Base1_.QualifiedID, Registration>.ConversionCallback(
                        delegate(string s) { return Base1_.QualifiedID.FromString(s); }));
        }

        #endregion

        #region IComparable<ITopic> Members

        int IComparable<ITopic>.CompareTo(ITopic other)
        {
            return ((IComparable<Base1_.QualifiedID>) qualifiedid).CompareTo(other.ID);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            ITopic other = obj as ITopic;
            if (other != null)
                return ((IComparable<ITopic>) this).CompareTo(other);
            else
                throw new Exception("The argument of the comparison is not a topic.");
        }

        #endregion

        #region IEquatable<ITopic> Members

        bool IEquatable<ITopic>.Equals(ITopic other)
        {
            return (other != null) && ((IEquatable<Base1_.QualifiedID>) qualifiedid).Equals(other.ID);
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            ITopic other = obj as ITopic;
            return (other != null) && ((IEquatable<ITopic>) this).Equals(other);
        }

        public override int GetHashCode()
        {
            return qualifiedid.GetHashCode();
        }

        public override string ToString()
        {
            return qualifiedid.ToString();
        }

        #endregion

        #region ITopic Members

        QS._qss_x_.Base1_.QualifiedID ITopic.ID
        {
            get { return qualifiedid; }
        }

        string ITopic.Name
        {
            get { return name; }
        }

        IDomain ITopic.Root
        {
            get { return root; }
            
            set 
            {
                if (root != null)
                {
                    if (value == null)
                    {
                        _StopRoot();
                        root = null;
                    }
                    else
                        throw new Exception("This topic already has one root domain assigned to it.");
                }
                else
                {
                    if (value != null)
                    {
                        Domain domain = value as Domain;
                        if (domain == null)
                            throw new Exception("The domain passed as an argument is of a wrong type.");

                        root = domain;
                        _StartRoot();
                    }
                    else
                        throw new Exception("This topic does not have a root domain assigned to it.");
                }
            }
        }

        #endregion

        // internal 

        #region Internal Interface

        internal Base1_.QualifiedID ID
        {
            get { return qualifiedid; }
        }

        internal string Name
        {
            get { return name; }
        }

        internal bool IsLocal
        {
            get { return islocal; }
        }

        internal Provider Provider
        {
            get { return provider; }
        }

        internal IDictionary<Base1_.QualifiedID, Registration> Registrations
        {
            get { return registrations; }
        }

        internal Domain Root
        {
            get { return root; }
        }

        #endregion

        // private

        #region _StartRoot

        private void _StartRoot()
        {
            if (islocal)
                root.StartRoot(this);
            else
                localscope.ForwardSubscribe(root, this);
        }

        #endregion

        #region _StopRoot

        private void _StopRoot()
        {
            if (islocal)
                root.StopRoot(this);
            else
                localscope.ForwardUnsubscribe(root, this);
        }

        #endregion
    }
}
