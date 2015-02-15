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

// #define DEBUG_SimpleURVS

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting5
{
    /// <summary>
    /// This region view sender does nothing but invokes the underlying region sender. No retransmissions are done. 
    /// </summary>
    public class SimpleURVS : Base3_.SenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender>
    {
        public SimpleURVS(QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders)
        {
            this.logger = logger;
            this.regionSenders = regionSenders;
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders;

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender
        {
            public Sender(SimpleURVS owner, Base3_.RVID regionViewID)
            {
                this.owner = owner;
                this.regionViewID = regionViewID;

                regionSender = owner.regionSenders[regionViewID.RegionID];
            }

            private SimpleURVS owner;
            private Base3_.RVID regionViewID;
            private Base3_.IReliableSerializableSender regionSender;

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
#if DEBUG_SimpleURVS
                owner.logger.Log(this, "__BeginSend: " + regionViewID.ToString() + ", " + destinationLOID.ToString() + ", " + data.ToString());
#endif

                return regionSender.BeginSend(destinationLOID, data, completionCallback, asynchronousState);
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
                regionSender.EndSend(asynchronousOperation);
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return regionSender.Address; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((Base3_.IReliableSerializableSender) this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        protected override QS._qss_c_.Base3_.IReliableSerializableSender CreateSender(QS._qss_c_.Base3_.RVID address)
        {
            return new Sender(this, address);
        }
    }
}
