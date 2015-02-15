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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Publisher, "Publisher", "A component that can pump values from a value object into a channel.")]
    public sealed class Publisher<
        [QS.Fx.Reflection.Parameter("ValueClass", QS.Fx.Reflection.ParameterClass.ValueClass)] ValueClass> 
        : QS.Fx.Inspection.Inspectable, 
        QS.Fx.Object.Classes.IObject,
        QS.Fx.Interface.Classes.IValueClient<ValueClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>,
        IDisposable
        where ValueClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Publisher(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("value", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<ValueClass>> _valueobjectref,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>> _channelobjectref)
        {
            if (_valueobjectref == null)
                throw new Exception("Cannot run with a null value object reference.");

            if (_channelobjectref == null)
                throw new Exception("Cannot run with a null channel object reference.");

            this._valueobject = _valueobjectref.Dereference(_mycontext);

            this._channelobject = _channelobjectref.Dereference(_mycontext);

            this._valueendpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IValue<ValueClass>,
                    QS.Fx.Interface.Classes.IValueClient<ValueClass>>(this);

            this._channelendpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>>(this);

            this._value = null;

            lock (this)
            {
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channelobject.Channel);
                this._valueconnection = ((QS.Fx.Endpoint.Classes.IEndpoint) this._valueendpoint).Connect(_valueobject.Endpoint);
            }
        }

        #endregion

        #region Finalizer

        ~Publisher()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    if (this._valueconnection != null)
                    {
                        this._valueconnection.Dispose();
                        this._valueconnection = null;
                    }

                    if (this._channelconnection != null)
                    {
                        this._channelconnection.Dispose();
                        this._channelconnection = null;
                    }
                }
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("valueobject")]
        private QS.Fx.Object.Classes.IValue<ValueClass> _valueobject;
        [QS.Fx.Base.Inspectable("valueendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IValue<ValueClass>, 
            QS.Fx.Interface.Classes.IValueClient<ValueClass>> _valueendpoint;
        [QS.Fx.Base.Inspectable("valueconnection")]
        private QS.Fx.Endpoint.IConnection _valueconnection;

        [QS.Fx.Base.Inspectable("channelobject")]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass> _channelobject;
        [QS.Fx.Base.Inspectable("chananelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>> _channelendpoint;
        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        [QS.Fx.Base.Inspectable("value")]
        private ValueClass _value;

        #endregion

        #region IValueClient<ValueClass> Members

        void QS.Fx.Interface.Classes.IValueClient<ValueClass>.Set(ValueClass _value)
        {
            lock (this)
            {
                this._value = _value;

                if (this._channelendpoint.IsConnected)
                    this._channelendpoint.Interface.Send(this._value);
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<ValueClass,ValueClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Receive(ValueClass _message)
        {
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Initialize(ValueClass _checkpoint)
        {
        }

        ValueClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Checkpoint()
        {
            if (this._valueendpoint.IsConnected)
                this._value = this._valueendpoint.Interface.Get();

            return this._value;
        }

        #endregion
    }
}
