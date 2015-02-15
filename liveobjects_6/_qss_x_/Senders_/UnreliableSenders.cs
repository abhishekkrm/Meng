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
    public class UnreliableSenders : QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender>        
    {
        public UnreliableSenders(
            QS.Fx.Network.INetworkConnection networkConnection, QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenders)
        {
            this.networkConnection = networkConnection;
            this.underlyingSenders = underlyingSenders;
        }

        private QS.Fx.Network.INetworkConnection networkConnection;
        private QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenders;
        private IDictionary<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender> senders =
            new Dictionary<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender>();

        #region ISenderCollection<Address,ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender>.this[QS._qss_x_.Base1_.Address destinationAddress]
        {
            get 
            {
                lock (this)
                {
                    QS._qss_c_.Base3_.ISerializableSender sender;
                    if (!senders.TryGetValue(destinationAddress, out sender))
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

                            System.Diagnostics.Debug.Assert(!ipAddress.Equals(System.Net.IPAddress.None));

                            QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(ipAddress, portno);

                            try
                            {
                                sender = underlyingSenders[networkAddress];
                            }
                            catch (Exception exc)
                            {
                                throw new Exception("Cannot create sender for address \"" + destinationAddress.ToString() +
                                    "\", cannot create sender in the underlying sender collection.", exc);
                            }

                            senders.Add(destinationAddress, sender);
                        }
                        else
                            throw new Exception("Cannot create sender for address \"" + destinationAddress.ToString() + "\", unsupported protocol.");
                    }

                    return sender;
                }
            }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get 
            {
                throw new NotImplementedException();

/*
                lock (this)
                {
                    List<string> result = new List<string>(senders.Count);
                    foreach (QS.Fx.Base.Address address in senders.Keys)
                        result.Add(address.Uri.OriginalString);
                    return result;
                }
*/
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            {
                throw new NotImplementedException();

/*
                QS.Fx.Base.Address address = new QS.Fx.Base.Address(new Uri(attributeName));
                lock (this)
                {
                    QS.CMS.Base3.ISerializableSender sender;
                    senders.TryGetValue(address, out 

                }
*/ 
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Unreliable Senders"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
