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
using QS._qss_x_.Interfaces_.Classes_;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass("CA5BB719D2904BCFB84ADDE1557B6FDD", "WcfChannel", "")]
    public sealed class WcfChannel_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable, IWcfChannelCallback_,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public WcfChannel_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address)
        {
            this.myctxt = _mycontext;
            this._address = _address;
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
            this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
            this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);

            
            epAddress = new EndpointAddress(_address + "/WcfChannel_");
            WSDualHttpBinding _binding = new WSDualHttpBinding();
            
            // http://msdn.microsoft.com/en-us/library/ms734681.aspx
            // http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/57140f2b-d990-49e4-9622-a4a8a808b12c
            factory = new DuplexChannelFactory<IWcfChannel_>(new InstanceContext(this), _binding, epAddress);

            factory.Closed += new EventHandler(factory_Closed);
            factory.Opened += new EventHandler(factory_Opened);
            factory.Faulted += new EventHandler(factory_Faulted);

            try
            {
                cclient = factory.CreateChannel();
                cclient.Connect();
            }
            catch (Exception e)
            {
                _mycontext.Platform.Logger.Log(e.ToString());
            }
        }

        void factory_Faulted(object sender, EventArgs e)
        {
            this.myctxt.Platform.Logger.Log("factory faulted: " + e.ToString());
        }

        void factory_Opened(object sender, EventArgs e)
        {
            this.myctxt.Platform.Logger.Log("factory opened: " + e.ToString());
        }

        void factory_Closed(object sender, EventArgs e)
        {
            this.myctxt.Platform.Logger.Log("factory closed: " + e.ToString());
        }

        ~WcfChannel_()
        {
            cclient.Disconnect();
            factory.Close();
        }


        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _address;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channelendpoint;
        
        EndpointAddress epAddress;
        DuplexChannelFactory<IWcfChannel_> factory;
        IWcfChannel_ cclient;
        QS.Fx.Object.IContext myctxt;

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

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            QS.Fx.Base.ConsumableBlock msgHeader = new QS.Fx.Base.ConsumableBlock((uint)_message.SerializableInfo.HeaderSize);
            IList<QS.Fx.Base.Block> msgDataList = new List<QS.Fx.Base.Block>();
            
            _message.SerializeTo(ref msgHeader, ref msgDataList);
            List<byte> headerBytes = new List<byte>();
            List<byte> allMsgBytes = new List<byte>();
            
            headerBytes.AddRange(msgHeader.Array);
            foreach (QS.Fx.Base.Block b in msgDataList)
            {
                allMsgBytes.AddRange(b.buffer);
            }
            //Get the serialized message in the form of a byte array:
            ushort classid = _message.SerializableInfo.ClassID;
            byte[] headerArray = headerBytes.ToArray();
            byte[] dataArray = allMsgBytes.ToArray();

            try
            {
                myctxt.Platform.Logger.Log("sending message: classid: " + classid + " header length: " + headerArray.Length + " data length: " + dataArray.Length);
                cclient.Submit_Update(classid, headerArray, dataArray);
            }
            catch (FaultException ex)
            {
                myctxt.Platform.Logger.Log("Fault Exception in Send: " + ex.ToString());
                myctxt.Platform.Logger.Log("State after exception: " + factory.State.ToString());
            }
            catch (CommunicationException ex)
            {
                myctxt.Platform.Logger.Log("Communication Exception in Send: " + ex.ToString());
                myctxt.Platform.Logger.Log("State after exception: " + factory.State.ToString());
            }
            catch (Exception e)
            {
                myctxt.Platform.Logger.Log("Exception in Send: " + e.ToString());
                myctxt.Platform.Logger.Log("State after exception: " + factory.State.ToString());
            }
        }

        #endregion

        #region WCF CallBacks

        public void Initialize(ushort classid, byte[] header, byte[] data)
        {
            ArraySegment<byte> chkptHeader = new ArraySegment<byte>(header);
            QS.Fx.Base.ConsumableBlock chkptHeaderBlock = new QS.Fx.Base.ConsumableBlock(chkptHeader);
            ArraySegment<byte> chkptData = new ArraySegment<byte>(data);
            QS.Fx.Base.ConsumableBlock chkptDataBlock = new QS.Fx.Base.ConsumableBlock(chkptData);

            CheckpointClass cpc = (CheckpointClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(classid);
            cpc.DeserializeFrom(ref chkptHeaderBlock, ref chkptDataBlock);
            this._channelendpoint.Interface.Initialize(cpc);
        }

        public void Request_Checkpoint()
        {
            /*
            CheckpointClass _cp = this._channelendpoint.Interface.Checkpoint();

            QS.Fx.Base.ConsumableBlock msgHeader = new QS.Fx.Base.ConsumableBlock((uint)_cp.SerializableInfo.HeaderSize);
            IList<QS.Fx.Base.Block> msgDataList = new List<QS.Fx.Base.Block>();

            _cp.SerializeTo(ref msgHeader, ref msgDataList);
            List<byte> headerBytes = new List<byte>();
            List<byte> allMsgBytes = new List<byte>();

            headerBytes.AddRange(msgHeader.Array);
            foreach (QS.Fx.Base.Block b in msgDataList)
            {
                allMsgBytes.AddRange(b.buffer);
            }
            //Get the serialized message in the form of a byte array:
            ushort classid = _message.SerializableInfo.ClassID;
            byte[] headerArray = headerBytes.ToArray();
            byte[] dataArray = allMsgBytes.ToArray();

            try
            {
                myctxt.Platform.Logger.Log("sending message: classid: " + classid + " header length: " + headerArray.Length + " data length: " + dataArray.Length);
                cclient.Submit_Update(classid, headerArray, dataArray);
            }
            catch (Exception e)
            {
                myctxt.Platform.Logger.Log("Exception in Send: " + e.ToString());
            }
            */

        }

        public void Update(ushort classid, byte[] header, byte[] data)
        {
            ArraySegment<byte> msgHeader = new ArraySegment<byte>(header);
            QS.Fx.Base.ConsumableBlock msgHeaderBlock = new QS.Fx.Base.ConsumableBlock(msgHeader);
            ArraySegment<byte> msgData = new ArraySegment<byte>(data);
            QS.Fx.Base.ConsumableBlock msgDataBlock = new QS.Fx.Base.ConsumableBlock(msgData);

            MessageClass cp = (MessageClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(classid);
            cp.DeserializeFrom(ref msgHeaderBlock, ref msgDataBlock); 

            try
            {
                this._channelendpoint.Interface.Receive(cp);
            }
            catch (Exception e) {
                myctxt.Platform.Logger.Log("Exception in Update(): " + e.ToString());
            }
        }

        #endregion

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
