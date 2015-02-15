/*

Copyright (c) 2004-2009 Colin Barth. All rights reserved.

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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("6BF98124BCEA4b99ABD695858375220C", "Test Discovery")]
    public sealed class DiscoveryTest
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IDiscoveryClient<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>
    {
        #region Constructor

        public DiscoveryTest
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Discovery", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IDiscovery<
                    QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>> _discovery_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug)
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DiscoveryTest.Constructor");
#endif
            if (_discovery_reference == null)
                _mycontext.Error("Dicovery reference cannot be NULL.");

            this.id = new QS.Fx.Base.Identifier("12345");
            QS.Fx.Value.STUNAddress[] stunaddrs = { new QS.Fx.Value.STUNAddress("1.2.3.4", 1000, "4.3.2.1", 2000) };
            this.addrs = new AddressArray<QS.Fx.Value.STUNAddress>(stunaddrs);

            // create
            this._discovery_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDiscoveryOps<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>,
                    QS.Fx.Interface.Classes.IDiscoveryClient<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>>(this);

            // add callbacks
            this._discovery_endpoint.OnConnected += new QS.Fx.Base.Callback(_Discovery_Connected);
            this._discovery_endpoint.OnDisconnect += new QS.Fx.Base.Callback(_Discovery_Disconnect);

            // connect
            this._discovery_connection = this._discovery_endpoint.Connect(_discovery_reference.Dereference(_mycontext).Discovery);


        }

        #endregion

        #region Fields

        private QS.Fx.Base.Identifier id;
        private AddressArray<QS.Fx.Value.STUNAddress> addrs;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDiscoveryOps<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>,
            QS.Fx.Interface.Classes.IDiscoveryClient<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>> _discovery_endpoint;

        private QS.Fx.Endpoint.IConnection _discovery_connection;

        private System.Timers.Timer msgTimer;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Discovery_Connect

        private void _Discovery_Connected()
        {
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Discovery_Connected)));
        }

        private void _Discovery_Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            try
            {
                this._discovery_endpoint.Interface.Register(this.id, this.addrs);

                if (msgTimer == null)
                {
                    msgTimer = new System.Timers.Timer();
                    msgTimer.Elapsed += new ElapsedEventHandler(_Lookup);
                    msgTimer.Interval = 50000;
                    msgTimer.AutoReset = false;
                    msgTimer.Start();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion

        #region _Discovery_Disconnect

        private void _Discovery_Disconnect()
        {
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Discovery_Disconnect)));           
        }

        private void _Discovery_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            //if ()
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDiscoveryClient<Identifier,STUNAddress> Members

        void QS.Fx.Interface.Classes.IDiscoveryClient<QS.Fx.Base.Identifier, QS.Fx.Value.STUNAddress>.Found(QS.Fx.Base.Identifier id, QS.Fx.Value.Classes.IAddressCollection<QS.Fx.Value.STUNAddress> addrs)
        {
            throw new NotImplementedException(addrs.Addresses.ElementAt(0).ToString());
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region

        private void _Lookup(object source, ElapsedEventArgs e)
        {
            if (this._discovery_endpoint.IsConnected)
            {
                this._discovery_endpoint.Interface.Lookup(this.id);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        private class AddressArray<AddressClass>
            : QS.Fx.Value.Classes.IAddressCollection<AddressClass>
        {
            public AddressArray(AddressClass[] addresses)
            {
                this.addresses = addresses;
            }

            private AddressClass[] addresses;

            #region IAddressCollection<AddressClass> Members

            IEnumerable<AddressClass> QS.Fx.Value.Classes.IAddressCollection<AddressClass>.Addresses
            {
                get { return this.addresses; }
            }

            #endregion
        }
    }
}
