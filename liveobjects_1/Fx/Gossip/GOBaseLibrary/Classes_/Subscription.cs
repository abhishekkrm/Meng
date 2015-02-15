/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Collections;
using GOBaseLibrary.Interfaces;

namespace GOBaseLibrary.Common
{
    // TODO: Move this to a config file
    [Serializable]
    public enum SubscriptionPropertyKey
    {
        AVERAGE_LOAD,
        MAX_LATENCY,
        GOSSIP_INTERVAL,
        MAX_MESSAGE_RATE
    };

    // TODO: Move this to a config file
    [Serializable]
    public class SubscriptionPropertyValue
    {
        Object value;

        public override String ToString()
        {
            return value.ToString();
        }

        public SubscriptionPropertyValue(Object val)
        {
            value = val;
        }
    }
    
    /// <summary>
    /// A SubscriptionProperties contains a list of properties that are.  Each term is a key/value pair indicating
    /// the name of the term and its value.  The names of terms are restricted to being members
    /// of ItemKey.  The values of terms can be any object as defined by ItemValue.
    /// </summary>
    
    [Serializable]
    public class Subscription : ISubscription
    {
        // Place holder for all key/value pairs of term's key/value pairs
        private Hashtable ItemKeyValuePairs;
        
        public Subscription()
        {
            ItemKeyValuePairs = new Hashtable();
        }

        // Return an enumerator for going over all terms
        public IDictionaryEnumerator GetEnumerator()
        {
            return ItemKeyValuePairs.GetEnumerator();
        }

        #region ISubscriptionProperties Members

        // Return the property value for a given key
        public SubscriptionPropertyValue Get(SubscriptionPropertyKey key)
        {
            return ItemKeyValuePairs.Contains(key)
                                    ? ItemKeyValuePairs[key] as SubscriptionPropertyValue
                                    : null as SubscriptionPropertyValue;
        }

        // Set the property value of a given key
        public void Set(SubscriptionPropertyKey key, SubscriptionPropertyValue value)
        {
            ItemKeyValuePairs[key] = value;
        }

        // Remove a property
        public void Remove(SubscriptionPropertyKey key)
        {
            ItemKeyValuePairs.Remove(key);
        }

        // Remove all properties
        public void Clear()
        {
            ItemKeyValuePairs.Clear();
        }

        // Return the number of properties
        public int Count()
        {
            return ItemKeyValuePairs.Count;
        }

        #endregion        
    }
}
