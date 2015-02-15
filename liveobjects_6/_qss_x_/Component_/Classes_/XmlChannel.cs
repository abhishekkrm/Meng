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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.XmlChannel, 
        "Xml Channel",
        "A checkpoint communication channel carrying serializable .NET objects encoded as XML strings.")]
    public sealed class XmlChannel_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass> 
        : QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>
        where MessageClass : class
        where CheckpointClass : class
    {
        #region Constructor

        public XmlChannel_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("underlyingchannel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>
                        _underlyingchannel)
        {
            this._mycontext = _mycontext;
            this._underlyingchannel = _underlyingchannel;
            this._outerendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
            this._innerendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>(this);
            this._outerendpoint.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
            this._outerendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
            this._messageserializer = new XmlSerializer(typeof(MessageClass));
            this._checkpointserializer = new XmlSerializer(typeof(CheckpointClass));
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>>
                    _underlyingchannel;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                    _outerendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>> 
                    _innerendpoint;
        private QS.Fx.Endpoint.IConnection _connection;
        private XmlSerializer _messageserializer, _checkpointserializer;

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._outerendpoint; }
        }

        #endregion

        #region _ConnectCallback

        private void _ConnectCallback()
        {
            this._connection = this._innerendpoint.Connect(_underlyingchannel.Dereference(_mycontext).Channel);            
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback()
        {
            this._connection.Dispose();
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            QS._core_c_.Base2.BlockOfData _block;
            try
            {
                if (_message != null)
                {
                    MemoryStream _stream = new MemoryStream();
                    this._messageserializer.Serialize(_stream, _message);
                    uint _length = (uint) _stream.Length;
                    _stream.Close();
                    _block = new QS._core_c_.Base2.BlockOfData(_stream.GetBuffer(), 0U, _length);
                }
                else
                    _block = new QS._core_c_.Base2.BlockOfData(null, 0U, 0U);
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not serialize the message.", _exc);
            }
            this._innerendpoint.Interface.Send(_block);
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IText,IText> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Receive(QS.Fx.Serialization.ISerializable _message)
        {
            MessageClass _deserializedmessage;
            try
            {
                if (_message != null)
                {
                    QS._core_c_.Base2.BlockOfData _block = (QS._core_c_.Base2.BlockOfData)_message;
                    if (_block.SizeOfData > 0)
                    {
                        MemoryStream _stream = _block.AsStream;
                        _deserializedmessage = (MessageClass) this._messageserializer.Deserialize(_stream);
                    }
                    else
                        _deserializedmessage = null;
                }
                else
                    _deserializedmessage = null;
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not deserialize the message.", _exc);
            }
            this._outerendpoint.Interface.Receive(_deserializedmessage);
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Initialize(QS.Fx.Serialization.ISerializable _checkpoint)
        {
            CheckpointClass _deserializedcheckpoint;
            try
            {
                if (_checkpoint != null)
                {
                    QS._core_c_.Base2.BlockOfData _block = (QS._core_c_.Base2.BlockOfData)_checkpoint;
                    if (_block.SizeOfData > 0)
                    {
                        MemoryStream _stream = _block.AsStream;
                        _deserializedcheckpoint = (CheckpointClass) this._checkpointserializer.Deserialize(_stream);
                    }
                    else
                        _deserializedcheckpoint = null;
                }
                else
                    _deserializedcheckpoint = null;
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not deserialize the checkpoint.", _exc);
            }
            this._outerendpoint.Interface.Initialize(_deserializedcheckpoint);
        }

        QS.Fx.Serialization.ISerializable QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.ISerializable>.Checkpoint()
        {
            CheckpointClass _checkpoint = this._outerendpoint.Interface.Checkpoint();
            QS._core_c_.Base2.BlockOfData _block;
            try
            {
                if (_checkpoint != null)
                {
                    MemoryStream _stream = new MemoryStream();
                    this._checkpointserializer.Serialize(_stream, _checkpoint);
                    uint _length = (uint) _stream.Length;
                    _stream.Close();
                    _block = new QS._core_c_.Base2.BlockOfData(_stream.GetBuffer(), 0U, _length);
                }
                else
                    _block = new QS._core_c_.Base2.BlockOfData(null, 0U, 0U);
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not serialize the checkpoint.", _exc);
            }
            return _block;
        }

        #endregion
    }
}
