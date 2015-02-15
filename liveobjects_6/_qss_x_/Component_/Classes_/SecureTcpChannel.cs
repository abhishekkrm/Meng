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

#define DEBUG_REPORT_DISCONNECT

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SecureTcpChannel,
        "SecureTcpChannel", "This component provides a secure point-to-point communication channel to a remote server.")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous)]
    public sealed class SecureTcpChannel<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>         
        : QS._qss_x_.SecureTcp_.Client, 
            QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>, QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public SecureTcpChannel(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address,
            [QS.Fx.Reflection.Parameter("network", QS.Fx.Reflection.ParameterClass.Value)] string _network) : base(_mycontext, _address, _network)
        {
            this._mycontext = _mycontext;
            this.endpoint = _mycontext.DualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>(this);
            this.endpoint.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
            this.endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>> endpoint;            

        #endregion

        #region ICommunicationChannel<MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>, QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>
                QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>._Channel
        {
            get { return this.endpoint; }
        }

        #endregion

        #region _ConnectCallback

        private void _ConnectCallback()
        {
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._ConnectCallback_0_)));
        }

        private void _ConnectCallback_0_(object _o)
        {
            lock (this)
            {
                this._Connect();
            }
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback()
        {
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._DisconnectCallback_0_)));
        }

        private void _DisconnectCallback_0_(object _o)
        {
            lock (this)
            {
                this._Disconnect();
            }
        }

        #endregion

        #region ICommunicationChannel<MessageClass> Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous)]
        void QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>._Send(MessageClass _message)
        {
            // TODO: Need to remove this; this is a double-protection, just in case...
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Send_0_), _message));
        }

        private void _Send_0_(object _o)
        {
            lock (this)
            {
                this._Send((QS.Fx.Serialization.ISerializable) _o);
            }
        }

        #endregion

        #region _Exception

        protected override void _Exception(Exception _exception)
        {
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Exception_0_), _exception));
        }

        private void _Exception_0_(object _o)
        {
            Exception _exception = (Exception) _o;
            lock (this)
            {
#if DEBUG_REPORT_DISCONNECT
                System.Windows.Forms.MessageBox.Show(_exception.ToString(), "Exception",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                /*
                            try
                            {
                                (new QS.Fx.Base.ExceptionForm(_exception)).Show();
                            }
                            catch (Exception)
                            {
                            }
                */
#endif

                this.endpoint.Disconnect();
            }
        }

        #endregion

        #region _Receive

        protected override void _Receive(QS.Fx.Serialization.ISerializable message)
        {
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Receive_0_), message));
        }

        private void _Receive_0_(object _o)
        {
            QS.Fx.Serialization.ISerializable message = (QS.Fx.Serialization.ISerializable) _o;
            lock (this)
            {
                MessageClass _message = message as MessageClass;
                if (_message != null)
                    this.endpoint.Interface._Receive(_message);
                else
                    _Disconnect();
            }
        }

        #endregion
    }
}
