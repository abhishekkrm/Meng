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
using System.Threading;

namespace QS._qss_c_._OldFx.Endpoints.Base
{     
    public abstract class Endpoint
    {
/*
        public Endpoint(EndpointType type)
        {
            this.type = type;
/-*
            // sanity check
            foreach (Type type in GetType().GetInterfaces())
            {
                if (!(type.IsGenericType && 
                    (type.GetGenericTypeDefinition().Equals(providerGenericType) || type.GetGenericTypeDefinition().Equals(consumerGenericType)))) 
                    throw new Exception("Cannot construct type \"" + GetType().ToString() + 
                        "\" because it inherits from Endpoint, but implements interfaces other than IProvider<C> or IConsumer<C>, which is not permitted."); 
            }
 *-/
        }

        private EndpointType type;
        private bool isConnected;
        private Endpoint connectedEndpoint;

        protected abstract void BindRequiredInterfaces(object[] objects);
        protected abstract void UnbindRequiredInterfaces();

        protected abstract void BindProvidedInterfaces(object[] objects);

        #region Main Functionality

        public void ConnectTo(Endpoint endpoint)
        {
            lock (this)
            {
                if (!type.Matches(endpoint.Type))
                    throw new Exception("Cannot connect, the types of endpoints did not match.");

                if (isConnected)
                    throw new Exception("This endpoint is already connected, disconnect it first.");

                // TODO: Perhaps fix, but for now we don't care!
                if (!Monitor.TryEnter(endpoint, 100))
                    throw new Exception("Could not lock to another endpoint, possible deadlock.");
                try
                {

                    // ...........................

                    connectedEndpoint = endpoint;
                    isConnected = true;

                }
                finally
                {
                    Monitor.Exit(endpoint);
                }
            }
        }

        public void Disconnect()
        {
            this.Disconnect(true);
        }

        private void Disconnect(bool both)
        {
            Endpoint other = null;

            lock (this)
            {
                if (!isConnected)
                    throw new Exception("This endpoint is not connected.");

                UnbindRequiredInterfaces();

                other = connectedEndpoint;
                connectedEndpoint = null;
                isConnected = false;
            }

            if (other != null)
                other.Disconnect(false);
        }

        public bool Connected
        {
            get { return isConnected; }
        }

        public EndpointType Type
        {
            get { return this.type; }
        }

        #endregion

        public static C Cast<C>(Endpoint endpoint) where C : class
        {
            if (endpoint is IInterface<C>)
                return ((IInterface<C>)endpoint).Interface;
            else
                throw new Exception("This endpoint does not provide interface \"" + typeof(C).ToString() + "\".");
        }

        public C Cast<C>() where C : class
        {
            return Endpoint.Cast<C>(this);
        }

/-*
        private static readonly Type providerGenericType = typeof(IProvider<object>).GetGenericTypeDefinition();
        private static readonly Type consumerGenericType = typeof(IConsumer<object>).GetGenericTypeDefinition();

        public static void Consume<C>(Endpoint endpoint, C service) where C : class
        {
            if (endpoint is IConsumer<C>)
                ((IConsumer<C>)endpoint).Interface = service;
            else
                throw new Exception("This endpoint does not consume interface \"" + typeof(C).ToString() + "\".");
        }

        public void Consume<C>(C service) where C : class
        {
            Endpoint.Consume<C>(this, service);
        }

        public bool Provides(Type type)
        {
            if (type.IsInterface)
                return providerGenericType.MakeGenericType(type).IsAssignableFrom(GetType());
            else
                throw new Exception("Type \"" + type.ToString() + "\" is not an interface.");
        }

        public bool Requires(Type type)
        {
            if (type.IsInterface)
                return consumerGenericType.MakeGenericType(type).IsAssignableFrom(GetType());
            else
                throw new Exception("Type \"" + type.ToString() + "\" is not an interface.");
        }
        
//        public bool Consumes<C>() where C : class
//        {
//            return this is IConsumer<C>;
//        }

//        public bool Implements<C>() where C : class
//        {
//            return this is IProvider<C>;
//        }

        public IEnumerable<Type> Provided
        {
            get 
            {
                List<Type> provided = new List<Type>();
                foreach (Type type in GetType().GetInterfaces())
                {
                    if (type.GetGenericTypeDefinition().Equals(providerGenericType))
                        provided.Add(type);
                }
                return provided;
            }
        }

        public IEnumerable<Type> Required
        {
            get
            {
                List<Type> required = new List<Type>();
                foreach (Type type in GetType().GetInterfaces())
                {
                    if (type.GetGenericTypeDefinition().Equals(consumerGenericType))
                        required.Add(type);
                }
                return required;
            }
        }
*-/
*/ 
    }
}
