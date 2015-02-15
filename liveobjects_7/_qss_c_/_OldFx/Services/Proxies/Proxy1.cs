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

#define DEBUG_Proxy1

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace QS._qss_c_._OldFx.Services.Proxies
{
    public class Proxy1
    {
        public Proxy1(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, 
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senders)
        {
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.senders = senders;

            demultiplexer.register(
                (uint)ReservedObjectID.Fx_Services_Hosting_ResponseChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.ResponseCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senders;
        private int lastused_seqno = 0;
        private IDictionary<int, IMethodCallMessage> pendingCalls = new Dictionary<int, IMethodCallMessage>();

        #region ConnectTo

        public Base.IServiceRef<C> ConnectTo<C>(QS._core_c_.Base3.InstanceID instanceAddress, string path)
        {
            Proxy proxy = new Proxy(this, typeof(C), instanceAddress, senders[instanceAddress], path);
            C service = (C) proxy.GetTransparentProxy();
            if (service == null)
                throw new Exception("Service is null.");
            return new Base.ServiceRef<C>(service, proxy);
        }

        #endregion

        #region ResponseCallback

        private QS.Fx.Serialization.ISerializable ResponseCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Base.Response response = receivedObject as Base.Response;
            if (response == null)
                throw new Exception("The received object is not of type " + typeof(Base.Response).ToString());

            IMethodReturnMessage methodReturn = (IMethodReturnMessage) response.Object;

#if DEBUG_Proxy1
            logger.Log(this, "Response is " + methodReturn.ToString());
#endif





            // ........................................................................................................................................................................


            return null;
        }

        #endregion

        #region Class Proxy

        private class Proxy : RealProxy, IDisposable
        {
            public Proxy(Proxy1 owner, System.Type interfaceType, QS._core_c_.Base3.InstanceID instanceAddress, 
                QS._qss_c_.Base3_.ISerializableSender asynchronousSender, string path) : base(interfaceType)
            {
                this.owner = owner;
                this.instanceAddress = instanceAddress;
                this.interfaceType = interfaceType;
                this.asynchronousSender = asynchronousSender;
                this.path = path;
            }

            private Proxy1 owner;
            private QS._core_c_.Base3.InstanceID instanceAddress;
            private System.Type interfaceType;
            private QS._qss_c_.Base3_.ISerializableSender asynchronousSender;
            private string path;

            #region Method Invoke

            public override IMessage Invoke(IMessage msg)
            {
                IMethodCallMessage methodCall = (IMethodCallMessage)msg;

#if DEBUG_Proxy1
                StringBuilder s = new StringBuilder();
                s.AppendLine("Proxy(" + instanceAddress.ToString() + ", " + path + ", " + interfaceType.Name + ").Invoke\n{");
                foreach (System.Collections.DictionaryEntry entry in msg.Properties)
                    s.AppendLine("\t" + entry.Key + " = " + entry.Value);
                s.AppendLine("}");
                Console.WriteLine("__________Invoke___\n" + s.ToString());
#endif

                int seqno;
                lock (owner)
                {
                    seqno = ++owner.lastused_seqno;
                    owner.pendingCalls.Add(seqno, methodCall);
                }
                    
                Base.Message message = new Base.Message(msg, path, seqno);

                asynchronousSender.send((uint)ReservedObjectID.Fx_Services_Hosting_MessageChannel, message);
   
                return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion
    }
}
