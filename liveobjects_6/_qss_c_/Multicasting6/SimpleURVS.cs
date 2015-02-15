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

namespace QS._qss_c_.Multicasting6
{
    public class SimpleURVS : Base4_.ISinkCollection<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
    {
        public SimpleURVS(QS.Fx.Logging.ILogger logger,
            Base4_.ISinkCollection<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionSinks)
        {
            this.logger = logger;
            this.regionSinks = regionSinks;
        }

        private QS.Fx.Logging.ILogger logger;
        private Base4_.ISinkCollection<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> regionSinks;
        private IDictionary<Base3_.RVID, Sender> senders = new Dictionary<Base3_.RVID, Sender>();

        #region GetSender

        private Sender GetSender(Base3_.RVID destinationRVID)
        {
            lock (this)
            {
                if (senders.ContainsKey(destinationRVID))
                    return senders[destinationRVID];
                else
                {
                    Sender sender = new Sender(this, destinationRVID);
                    senders[destinationRVID] = sender;
                    return sender;
                }
            }
        }

        #endregion

        #region Class Sender

        private class Sender : Base4_.IAddressedSink<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
        {
            public Sender(SimpleURVS owner, Base3_.RVID destinationRVID)
            {
                this.owner = owner;
                this.destinationRVID = destinationRVID;
                underlyingSink = owner.regionSinks[destinationRVID.RegionID];
            }

            private SimpleURVS owner;
            private Base3_.RVID destinationRVID;
            private Base4_.IAddressedSink<Base3_.RegionID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> underlyingSink;

            #region ISink<Message> Members

            QS._qss_c_.Base4_.IChannel QS._qss_c_.Base4_.ISink<Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Register(
                QS._qss_c_.Base4_.GetObjectsCallback<Base4_.Asynchronous<QS._core_c_.Base3.Message>> getObjectCallback)
            {
                return underlyingSink.Register(getObjectCallback);
            }

            uint QS._qss_c_.Base4_.ISink<Base4_.Asynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { return underlyingSink.MTU; }
            }

            #endregion

            #region IAddressedSink<RVID,Message> Members

            QS._qss_c_.Base3_.RVID 
                QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Address
            {
                get { return destinationRVID; }
            }

            #endregion
        }

        #endregion

        #region ISinkCollection<RVID,Message> Members

        QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return GetSender(destinationAddress);  }
        }

        #endregion
    }
}
