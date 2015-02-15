/*

Copyright (c) 2004-2009 Saad Sami. All rights reserved.

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

using System.ServiceModel;
using System.ServiceModel.Description;

using QS._qss_x_.Utility.Classes_;
using QS._qss_x_.Interfaces_.Classes_;

namespace QS._qss_x_.Component_.Classes_
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [QS.Fx.Reflection.ComponentClass("53915EBDD0E84E04BAAF1AF03F779856", "WcfChannelController", "")]
    public sealed class WcfChannelController_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass> 
        : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass,CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            //QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            IWcfChannel_
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public WcfChannelController_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("enablebackend", QS.Fx.Reflection.ParameterClass.Value)] bool _enablebackend,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address,
            [QS.Fx.Reflection.Parameter("replica", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _replicaref,
            [QS.Fx.Reflection.Parameter("backend", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _backendref
            )
        {
            this._mycontext = _mycontext;
            this._address = _address;
            this._enablebackend = _enablebackend;
            
            // setup the replica (state component)
            this._replicaref = _replicaref;
            this._replicaobj = this._replicaref.Dereference(this._mycontext);

            this._replicaendp = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this._replicaconn = this._replicaendp.Connect(this._replicaobj.ChannelClient);
            this._replicaendp.OnConnected += new QS.Fx.Base.Callback(_replicaendp_OnConnected);

            if (this._enablebackend)
            {
                // setup the backing component
                this._backendref = _backendref;
                this._backendobj = this._backendref.Dereference(this._mycontext);

                this._backendendp = this._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this);

                this._backendconn = this._backendendp.Connect(this._backendobj.Channel);
            }

            // The WSDualHttpBinding offers reliable and ordered messaging with sessions
            WSDualHttpBinding dualbinding = new WSDualHttpBinding();
            dualbinding.ClientBaseAddress = new Uri(_address);

            //wcfs = new WcfService(_mycontext);
            //dualbinding.ReceiveTimeout = new TimeSpan(0, 0, 20); // determines how long to wait for the
                                                                   // service to timeout.  default 10 mins?
            //selfHost = new ServiceHost(wcfs, dualbinding.ClientBaseAddress);
            selfHost = new ServiceHost(this, dualbinding.ClientBaseAddress);          

            clientCallbacks = new List<IWcfChannelCallback_>();
            try
            {
                selfHost.AddServiceEndpoint(
                    typeof(IWcfChannel_),
                    dualbinding,
                    "WcfChannel_");


                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();

                _mycontext.Platform.Logger.Log("Service Started!");
            }
            catch (CommunicationException ce)
            {
                _mycontext.Platform.Logger.Log("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }

        }

        #endregion

        void _replicaendp_OnConnected()
        {
            this._replicaendp.Interface.Initialize(null);
        }

        ~WcfChannelController_()
        {
            selfHost.Close();
        }

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _address;
        [QS.Fx.Base.Inspectable]
        private bool _enablebackend;
        
        // replica fields
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _replicaref;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass> _replicaobj;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _replicaendp;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _replicaconn;

        // backend fields
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _backendref;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass> _backendobj;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _backendendp;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _backendconn;

        private WcfService wcfs;
        private ServiceHost selfHost;
        private List<IWcfChannelCallback_> clientCallbacks;
        IWcfChannelCallback_ cb = null;

        #endregion

        #region WCF Commands
        void IWcfChannel_.Submit_Update(ushort classid, byte[] header, byte[] data)
        {
            
            ArraySegment<byte> msgHeader = new ArraySegment<byte>(header);
            QS.Fx.Base.ConsumableBlock msgHeaderBlock = new QS.Fx.Base.ConsumableBlock(msgHeader);
            ArraySegment<byte> msgData = new ArraySegment<byte>(data);
            QS.Fx.Base.ConsumableBlock msgDataBlock = new QS.Fx.Base.ConsumableBlock(msgData);

            MessageClass cp = (MessageClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(classid);
            cp.DeserializeFrom(ref msgHeaderBlock, ref msgDataBlock);

            if (this._enablebackend)
            {
                // Send to Backing Component
                _backendendp.Interface.Send(cp);
            }

            // Send to State Component
            try
            {
                _replicaendp.Interface.Receive(cp);
            }
            catch (Exception e)
            {
                _mycontext.Platform.Logger.Log("Exception in Submit_Update, Interface.Receive: " + e.ToString());
            }

            _mycontext.Platform.Logger.Log("message rcv'd: classid: " + classid + " header length: " + header.Length + " data length: " + data.Length);
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<QS._qss_x_.Interfaces_.Classes_.IWcfChannelCallback_>();

                foreach (var c in clientCallbacks)
                {
                    //if (c != cb)
                    //{
                    try
                    {
                        c.Update(classid, header, data);
                    }
                    catch (Exception e)
                    {
                        _mycontext.Platform.Logger.Log("Exception in Submit_Update, c.Update: " + e.ToString());
                    }
                    //}
                }
            }
        }

        void IWcfChannel_.Checkpoint(ushort classid, byte[] header, byte[] data)
        {
            throw new NotSupportedException();
        }

        void IWcfChannel_.Connect()
        {
            ushort classid;
            byte[] headerArray;
            byte[] dataArray;
            
            CheckpointClass checkpoint;
            


            //QS.Fx.Base.ConsumableBlock chkptHeader = new QS.Fx.Base.ConsumableBlock((uint)checkpoint.SerializableInfo.HeaderSize);
            //IList<QS.Fx.Base.Block> chkptDataList = new List<QS.Fx.Base.Block>();

            //checkpoint.SerializeTo(ref chkptHeader, ref chkptDataList);
            //List<byte> headerBytes = new List<byte>();
            //List<byte> allChkptBytes = new List<byte>();

            //headerBytes.AddRange(chkptHeader.Array);
            //foreach (QS.Fx.Base.Block b in chkptDataList)
            //{
            //    allChkptBytes.AddRange(b.buffer);
            //}
            //Get the serialized message in the form of a byte array:
            //classid = checkpoint.SerializableInfo.ClassID;
            //byte[] headerArray = headerBytes.ToArray();
            //byte[] dataArray = allChkptBytes.ToArray();

            _mycontext.Platform.Logger.Log("Client Attempting to Connect...");
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<IWcfChannelCallback_>();
                if (!clientCallbacks.Contains(cb))
                {
                    _mycontext.Platform.Logger.Log("New Client Added.");
                    clientCallbacks.Add(cb);
                    //fib(1000);
                    checkpoint = this._replicaendp.Interface.Checkpoint();
                    if (checkpoint != null)
                    {
                        SerializeCheckpoint(checkpoint, out classid, out headerArray, out dataArray);
                        try
                        {
                            cb.Initialize(classid, headerArray, dataArray);
                        }
                        catch (Exception e)
                        {
                            _mycontext.Platform.Logger.Log("Exception in Connect, Initialize(): " + e.ToString());
                        }
                    }
                }
            }
        }

        void IWcfChannel_.Disconnect()
        {
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<QS._qss_x_.Interfaces_.Classes_.IWcfChannelCallback_>();
                _mycontext.Platform.Logger.Log("Client Disconnecting...");
                clientCallbacks.Remove(cb);
                if (clientCallbacks.Contains(cb))
                {
                    _mycontext.Platform.Logger.Log("Disconnect Failed.");
                }
            }
        }

        #endregion

        #region Helper Methods
        public MessageClass DeserializeToMessage(ushort classid, byte[] header, byte[] data)
        {
            ArraySegment<byte> msgHeader = new ArraySegment<byte>(header);
            QS.Fx.Base.ConsumableBlock msgHeaderBlock = new QS.Fx.Base.ConsumableBlock(msgHeader);
            ArraySegment<byte> msgData = new ArraySegment<byte>(data);
            QS.Fx.Base.ConsumableBlock msgDataBlock = new QS.Fx.Base.ConsumableBlock(msgData);

            MessageClass cp = (MessageClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(classid);
            cp.DeserializeFrom(ref msgHeaderBlock, ref msgDataBlock);

            return cp;
        }

        public void SerializeCheckpoint(CheckpointClass _chkpt, out ushort _classid, out byte[] _header, out byte[] _data)
        {
            _classid = _chkpt.SerializableInfo.ClassID;

            QS.Fx.Base.ConsumableBlock _chkptHeader = new QS.Fx.Base.ConsumableBlock((uint)_chkpt.SerializableInfo.HeaderSize);
            IList<QS.Fx.Base.Block> _chkptDataList = new List<QS.Fx.Base.Block>();

            _chkpt.SerializeTo(ref _chkptHeader, ref _chkptDataList);
            List<byte> headerBytes = new List<byte>();
            List<byte> allMsgBytes = new List<byte>();

            headerBytes.AddRange(_chkptHeader.Array);
            foreach (QS.Fx.Base.Block b in _chkptDataList)
            {
                allMsgBytes.AddRange(b.buffer);
            }
            //Get the serialized message in the form of a byte array:
            _header = headerBytes.ToArray();
            _data = allMsgBytes.ToArray();
        }

        #endregion

        #region Replica Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            throw new NotSupportedException("The state replica on the server is not supposed to send anything.");
        }

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return _replicaendp; }
        }

        #endregion

        #region Backend Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
        {
            this._replicaendp.Interface.Receive(_message);
            //throw new NotImplementedException();
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
        {
            this._replicaendp.Interface.Initialize(_checkpoint);
            //throw new NotImplementedException();
        }

        CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
        {
            CheckpointClass checkpoint = this._replicaendp.Interface.Checkpoint();
            return checkpoint;
        }

        #endregion

        public ulong fib(ulong n)
        {
            if (n <= 1)
            {
                return n;
            }
            else
            {
                return fib(n - 1) + fib(n - 2);
            }
        }
    }
}
