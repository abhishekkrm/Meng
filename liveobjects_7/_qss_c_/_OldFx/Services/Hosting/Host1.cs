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

#define DEBUG_Host1

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Runtime.Remoting;

namespace QS._qss_c_._OldFx.Services.Hosting
{
    // This host uses quicksilver for transmission.
    public class Host1 : Base.IHost
    {
        public Host1(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID instanceID, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senders)
        {
            this.logger = logger;
            this.instanceID = instanceID;
            this.senders = senders;
            this.demultiplexer = demultiplexer;
            demultiplexer.register(
                (uint)ReservedObjectID.Fx_Services_Hosting_MessageChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.MessageCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Base3.InstanceID instanceID;
        private Base3_.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senders;
        private IDictionary<string, Registration> registrations = new Dictionary<string, Registration>();

        #region MessageCallback

        private QS.Fx.Serialization.ISerializable MessageCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Base.Message message = receivedObject as Base.Message;
            if (message == null)
                throw new Exception("The received object is not of type " + typeof(Base.Message).ToString());

            Registration registration;
            lock (this)
            {
                if (!registrations.TryGetValue(message.Path, out registration))
                    throw new Exception("No service has been installed at \"" + message.Path + "\".");
            }

#if DEBUG_Host1
            logger.Log(this, "Processing request from " + sourceAddress.ToString() + " to " + message.Path.ToString() + ".\n" + message.Object.ToString());
#endif

            IMethodCallMessage methodCall = (IMethodCallMessage) message.Object;
            if (methodCall == null)
                throw new Exception("The received message is not a method call.");

            if (!methodCall.MethodBase.ReflectedType.IsAssignableFrom(registration.InterfaceType))
                throw new Exception("Method call is for type " + methodCall.MethodBase.ReflectedType.ToString() + ", but the object registered at " +
                    registration.Path + " is of type " + registration.InterfaceType.ToString());

            IMethodReturnMessage methodReturn = RemotingServices.ExecuteMessage(registration.ServiceObject, methodCall);

#if DEBUG_Host1
            logger.Log(this, "Response is " + methodReturn.ToString());
#endif

            Base.Response response = new Base.Response(methodReturn, message.Cookie);
            ((QS._qss_c_.Base3_.ISerializableSender)senders[sourceAddress]).send((uint)ReservedObjectID.Fx_Services_Hosting_ResponseChannel, response);

            return null;
        }

        #endregion

        #region Class Registration

        private class Registration : Base.IRegistration
        {
            public Registration(Host1 owner, string path, Type interfaceType, MarshalByRefObject serviceObject, Uri uri)
            {
                this.owner = owner;
                this.path = path;
                this.interfaceType = interfaceType;
                this.serviceObject = serviceObject;
                this.uri = uri;
            }

            private Host1 owner;
            private string path;
            private Type interfaceType;
            private MarshalByRefObject serviceObject;
            private Uri uri;

            #region Accessors

            public string Path
            {
                get { return path; }
            }

            public Type InterfaceType
            {
                get { return interfaceType; }
            }

            public MarshalByRefObject ServiceObject
            {
                get { return serviceObject; }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                owner.unregister(this);
            }

            #endregion

            #region IRegistration Members

            Uri QS._qss_c_._OldFx.Services.Base.IRegistration.Uri
            {
                get { return uri; }
            }

            #endregion
        }

        #endregion

        #region IHost Members

        Base.IRegistration QS._qss_c_._OldFx.Services.Base.IHost.Register<InterfaceClass>(string path, MarshalByRefObject service)
        {
            if (!typeof(InterfaceClass).IsInterface || !typeof(InterfaceClass).IsAssignableFrom(service.GetType()))
                throw new Exception("The registered class must be hosted behind an interface that it implements.");

            lock (this)
            {
                Registration registration;
                if (registrations.TryGetValue(path, out registration))
                    throw new Exception("Another service, of type \"" + registration.InterfaceType.ToString() +
                        "\" has already been registered at \"" + path + "\".");

                registration = new Registration(this, path, typeof(InterfaceClass), service, Base.Addressing.EncodeUri(instanceID, path));
                registrations.Add(path, registration);
                return registration;
            }
        }

        #endregion

        private void unregister(Registration registration)
        {
            lock (this)
            {
                // for now, just do nothing..........
            }
        }
    }
}
