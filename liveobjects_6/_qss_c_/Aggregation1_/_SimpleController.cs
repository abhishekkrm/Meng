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

#define DEBUG_SimpleController

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation1_
{
/*
    public class SimpleController : IGossipControllerClass
    {
        public SimpleController(QS.Fx.Logging.ILogger logger, Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> senderCollection,
            Base3.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            this.senderCollection = senderCollection;

            demultiplexer.register((uint) ReservedObjectID.Gossiping_SimpleController, new QS.CMS.Base3.ReceiveCallback(this.receiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> senderCollection;

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            return null;
        }

        #endregion

        #region Class Message

        [QS.Fx.Serialization.ClassID(ClassID.Aggregation_SimpleController_Message)]
        public class Message : QS.Fx.Serialization.ISerializable
        {
            public Message()
            {
            }

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { throw new global::System.NotImplementedException(); }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                throw new NotImplementedException();
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion      

        #region Class Controller

        private class Controller : IGossipController
        {
            public Controller(_SimpleController owner, QS._core_c_.Base3.InstanceID localAddress, 
                IList<QS._core_c_.Base3.InstanceID> incomingNeighbors, IList<QS._core_c_.Base3.InstanceID> outgoingNeighbors)
            {
                if (outgoingNeighbors.Count > 1)
                    throw new ArgumentException("This controller does not support routing schemes with multiple outgoing paths.");

                this.localAddress = localAddress;
                this.outgoingAddress = (outgoingNeighbors.Count > 0) ? outgoingNeighbors[0] : null;
                this.owner = owner;
                this.incomingAddresses = incomingNeighbors;

#if DEBUG_SimpleController
                owner.logger.Log(this, "__constructor");
#endif
            }

            private QS._core_c_.Base3.InstanceID localAddress, outgoingAddress;
            private _SimpleController owner;
            private IList<QS._core_c_.Base3.InstanceID> incomingAddresses;

            #region IController Members

            public void Submit()
            {
#if DEBUG_SimpleController
                owner.logger.Log(this, "__Submit");
#endif


                foreach (QS._core_c_.Base3.InstanceID instanceID in outgoingNeighbors)
                {
                    owner.senderCollection[instanceID.Address].send((uint) ReservedObjectID.Gossiping_SimpleController, null);
                }
            }

            public void ChangePeers(Components.SetChange<QS._core_c_.Base3.InstanceID> incomingNeighborhoodChange, 
                Components.SetChange<QS._core_c_.Base3.InstanceID> outgoingNeighborhoodChange)
            {
                
                // TODO: Implementation required, but for now we don't bother...............................................................................................
                
            }

            #endregion
        }

        #endregion

        #region IGossipControllerClass Members

        public IGossipController CreateController(QS._core_c_.Base3.InstanceID localAddress, 
            IList<QS._core_c_.Base3.InstanceID> incomingNeighbors, IList<QS._core_c_.Base3.InstanceID> outgoingNeighbors)
        {
            return new Controller(this, localAddress, incomingNeighbors, outgoingNeighbors);
        }

        #endregion
    }
*/
}
