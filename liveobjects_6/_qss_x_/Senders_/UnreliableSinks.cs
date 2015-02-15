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

namespace QS._qss_x_.Senders_
{
    public class UnreliableSinks : QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>
    {
        public UnreliableSinks(QS.Fx.Network.INetworkConnection networkConnection, 
            QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinks)
        {
            this.networkConnection = networkConnection;
            this.underlyingSinks = underlyingSinks;
        }

        private QS.Fx.Network.INetworkConnection networkConnection;
        private QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> underlyingSinks;
        private IDictionary<QS._qss_x_.Base1_.Address, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinks =
            new Dictionary<QS._qss_x_.Base1_.Address, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();

        #region ICollectionOf<Address,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._qss_x_.Base1_.Address destinationAddress]
        {
            get 
            {
                lock (this)
                {
                    QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink;
                    if (!sinks.TryGetValue(destinationAddress, out sink))
                    {
                        string hostname;
                        int portno;
                        if (destinationAddress.ToQuickSilver(out hostname, out portno))
                        {
                            System.Net.IPAddress ipAddress = System.Net.IPAddress.None;

                            try
                            {
                                ipAddress = System.Net.IPAddress.Parse(hostname);
                            }
                            catch (Exception)
                            {
                                System.Net.IPHostEntry hostentry = networkConnection.GetHostEntry(hostname);
                                bool found = false;
                                foreach (System.Net.IPAddress _address in hostentry.AddressList)
                                {
                                    if (_address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        found = true;
                                        ipAddress = _address;
                                        break;
                                    }
                                }

                                if (!found)
                                    throw new Exception("Cannot resolve address \"" + hostname + "\".");
                            }

                            QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(ipAddress, portno);

                            try
                            {
                                sink = underlyingSinks[networkAddress];
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Cannot create sender for address \"" + destinationAddress.ToString() +
                                    "\", cannot create sender in the underlying sender collection.", exc);
                            }

                            sinks.Add(destinationAddress, sink);
                        }
                        else
                            throw new Exception("Cannot create sender for address \"" + destinationAddress.ToString() + "\", unsupported protocol.");
                    }

                    return sink;
                }
            }
        }

        #endregion
    }
}
