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

namespace QS._qss_c_.Senders6
{
/*
    public class ReliableInstanceSender
    {
        public ReliableInstanceSender(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, Base3.IDemultiplexer demultiplexer)
        {
        }

        private QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, Base3.IDemultiplexer demultiplexer;

        #region Class Sender

        private class Sender : Base3.IReliableSerializableSender
        {
            public Sender()
            {
            }

            #region IReliableSerializableSender Members

            QS.CMS.Base3.IAsynchronousOperation QS.CMS.Base3.IReliableSerializableSender.BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS.CMS.Base3.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void QS.CMS.Base3.IReliableSerializableSender.EndSend(QS.CMS.Base3.IAsynchronousOperation asynchronousOperation)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS.CMS.Base3.ISerializableSender.Address
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            void QS.CMS.Base3.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            int QS.CMS.Base3.ISerializableSender.MTU
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
*/ 
}
