/* Copyright (c) 2009 Abhishek Jha. All rights reserved.

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
SUCH DAMAGE. */

//


using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass("8B8B90F500BA4F259F7E560F33E48DBA", 
        "UnreliableEsbChannel", "")]
    public sealed class UnreliableEsbChannel_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable, 
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public UnreliableEsbChannel_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
        {
            this._address = _address;
            this._debugging = _debugging;
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);

            this._mycontext = _mycontext;

            this._hostname = System.Net.Dns.GetHostName();

            // Get our client for the Esb Sender Server ready.
            this._tcpClient = new TcpClient();
            this._mycontext.Platform.Logger.Log(this, "Connecting from ....." + this._hostname);
            _tcpClient.Connect(this._hostname, this._ESB_SENDER_PORT);
            this._mycontext.Platform.Logger.Log(this, "Connected!!");
            _stream = _tcpClient.GetStream();

            // Get our Server for the Esb Receiver Client ready.
            //this._mainServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //this.scheduleServerForEsbReceiver(null);
            
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _address;
        
        [QS.Fx.Base.Inspectable]
        private bool _debugging;
        
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channelendpoint;

        [QS.Fx.Base.Inspectable]
        private TcpClient _tcpClient;

        [QS.Fx.Base.Inspectable]
        private System.IO.Stream _stream;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private String _hostname;

        [QS.Fx.Base.Inspectable]
        private static int _MAX_CONNECTIONS = 10;

        [QS.Fx.Base.Inspectable]
        private Socket _mainServerSocket;

        //[QS.Fx.Base.Inspectable]
        //private Socket[] _clientSocketArray = new Socket[_MAX_CONNECTIONS];

        [QS.Fx.Base.Inspectable]
        private int _ESB_SENDER_PORT = 10000;
        
        //[QS.Fx.Base.Inspectable]
        //private int _PORT_FOR_ESB_RECEIVER = 10010;
        
        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass message)
        {
            this._mycontext.Platform.Logger.Log(this, "Inside the Send method!!");
            threadEnqueueSendToEsb(message);            
        }
        
        #endregion

        private void threadEnqueueSendToEsb(object o)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering threadEnqueueSendToEsb");
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(enqueueSendToEsb), o));
        }

        private void enqueueSendToEsb(object o)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering enqueueSendToEsb");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.sendToEsb), o));
        }

        private void sendToEsb(object o)
        {
            MessageClass mc = (MessageClass)o;
            byte[] dataArray = convert2Bytes(mc);

            this._mycontext.Platform.Logger.Log(this, "Starting transmission!!");
            this._stream.BeginWrite(dataArray, 0, dataArray.Length, new AsyncCallback(this.sendToEsbCallBack), dataArray);
        }

        private void sendToEsbCallBack(IAsyncResult r)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering sendToEsbCallBack");
            if (r.IsCompleted)
            {
                this._stream.EndWrite(r);
                //this._stream.Flush();
                // Temporary .. Shouldn't have to do this for each Send!
                this._stream.Close();
                this._tcpClient.Close();
                this._mycontext.Platform.Logger.Log(this, "Done transmission!!");
                this._tcpClient = new TcpClient();
                this._tcpClient.Connect(this._hostname, 10000);
                this._mycontext.Platform.Logger.Log(this, "Connected!!");
                this._stream = this._tcpClient.GetStream();
            }
            else
            {
                //oh no
                this._mycontext.Platform.Logger.Log(this, "r.IsCompleted is False in sendToEsbCallBack!!");
            }

        }

        private byte[] convert2Bytes(QS.Fx.Serialization.ISerializable s)
        {
            MessageClass message = (MessageClass)s;
            QS.Fx.Base.ConsumableBlock msgHeader = new QS.Fx.Base.ConsumableBlock((uint)message.SerializableInfo.HeaderSize);
            IList<QS.Fx.Base.Block> msgDataList = new List<QS.Fx.Base.Block>();
            message.SerializeTo(ref msgHeader, ref msgDataList);
            List<byte> allMsgBytes = new List<byte>();
            //Getting all the message bytes into one list.
            allMsgBytes.AddRange(msgHeader.Array);
            foreach (QS.Fx.Base.Block b in msgDataList)
            {
                allMsgBytes.AddRange(b.buffer);
            }
            //Get the serialized message in the form of a byte array:
            byte[] dataArray = allMsgBytes.ToArray();
            return dataArray;
        }

        /*
        private void threadEnqueueServerForEsbReceiver(object o)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering threadEnqueueServerForEsbReceiver");
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(enqueueServerForEsbReceiver), o));

        }
         */

        /*
         private void scheduleServerForEsbReceiver(Object o)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering enqueueServerForEsbReciever");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.serverForEsbReceiver), o));
        }
         

        private void serverForEsbReceiver(Object o)
        {
            // This is where we start our TCP server that receives connections from the Esb Receiver whenever the 
            // latter has any messages to give us. For every client request, we schedule an asynchronous callback to handle it.
            this._mycontext.Platform.Logger.Log(this, "Inside serverForEsbReceiver");
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, this._PORT_FOR_ESB_RECEIVER);
            this._mainServerSocket.Bind(ipEndPoint);
            this._mainServerSocket.Listen(5);
            // Create call back for client connections.
            this._mainServerSocket.BeginAccept(new AsyncCallback(this.enqueueThreadEsbReceiverConnected), null);
            this._mycontext.Platform.Logger.Log(this, "Leaving serverForEsbReceiver");
            
        }

        private void enqueueThreadEsbReceiverConnected(IAsyncResult r){
            try
            {
                Socket esbReceiverSocket = this._mainServerSocket.EndAccept(r);
                // Enqueue this connection to be handled in order.
                this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.scheduleEsbReceiverConnected), esbReceiverSocket));
                // Now, let the mainServerSocket go back to accepting new connections.
                this._mainServerSocket.BeginAccept(new AsyncCallback(this.enqueueThreadEsbReceiverConnected), null);
            }
            catch (SocketException se)
            {
                this._mycontext.Platform.Logger.Log(this, "Exception in enqueueThreadEsbReceiverConnected" + se.Message);
            }
        }

        private void scheduleEsbReceiverConnected(Object o)
        {
            this._mycontext.Platform.Logger.Log(this, "Entering scheduleEsbReceiverConnected");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.esbReceiverConnected), o));

        }

        private void esbReceiverConnected(Object o)
        {
            Socket sock = (Socket)o;
        }
         */

        #region _ChannelConnectCallback

        private void _ChannelConnectCallback()
        {
            this._channelendpoint.Interface.Initialize(null);
        }

        #endregion

        #region _ChannelDisconnectCallback

        private void _ChannelDisconnectCallback()
        {
        }

        #endregion
    }
}
