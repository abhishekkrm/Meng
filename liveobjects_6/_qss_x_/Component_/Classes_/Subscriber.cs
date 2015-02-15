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
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Subscriber, "Subscriber", "A value object that pulls its values from a communication channel.")]
    public sealed class Subscriber<
        [QS.Fx.Reflection.Parameter("ValueClass", "The type of values the subscriber subscribes to.", QS.Fx.Reflection.ParameterClass.ValueClass)] ValueClass>
        : QS.Fx.Inspection.Inspectable,
        QS.Fx.Object.Classes.IValue<ValueClass>,
        QS.Fx.Interface.Classes.IValue<ValueClass>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>
        where ValueClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Subscriber(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", "The underlying multicast channel over which the values are received.", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>> _channelobjectref)
        {
            this._valueendpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IValueClient<ValueClass>,
                    QS.Fx.Interface.Classes.IValue<ValueClass>>(this);

            if (_channelobjectref == null)
                throw new Exception("Cannot run with a null channel object reference.");

            this._channelobject = _channelobjectref.Dereference(_mycontext);

            this._channelendpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<ValueClass, ValueClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>>(this);

            this._value = null;

            lock (this)
            {
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channelobject.Channel);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("valueendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<ValueClass>, 
            QS.Fx.Interface.Classes.IValue<ValueClass>> _valueendpoint;

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
        [QS.Fx.Base.Inspectable("incoming_lock")]
        private System.Object _incoming_lock = new System.Object();

        #endregion

        #region IValue<ValueClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<ValueClass>, 
            QS.Fx.Interface.Classes.IValue<ValueClass>> 
                QS.Fx.Object.Classes.IValue<ValueClass>.Endpoint
        {
            get { return this._valueendpoint; }
        }

        #endregion

        #region IValue<ValueClass> Members

        ValueClass QS.Fx.Interface.Classes.IValue<ValueClass>.Get()
        {
            return this._value;
        }

        void QS.Fx.Interface.Classes.IValue<ValueClass>.Set(ValueClass _value)
        {
            throw new Exception("This type of subscriber cannot publish new values on its own, this functionality has been disabled.");
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<ValueClass,ValueClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Receive(ValueClass _message)
        {
            if (_message != null)
            {
                bool _incoming_locked = false;
                try
                {
                    lock (this)
                    {
                        this._value = _message;
                        Monitor.Enter(this._incoming_lock);
                        _incoming_locked = true;
                    }

                    lock (this._valueendpoint)
                    {
                        if (this._valueendpoint.IsConnected)
                            this._valueendpoint.Interface.Set(this._value);
                    }
                }
                finally
                {
                    if (_incoming_locked)
                        Monitor.Exit(this._incoming_lock);
                }
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Initialize(ValueClass _checkpoint)
        {
            if (_checkpoint != null)
            {
                bool _incoming_locked = false;
                try
                {
                    lock (this)
                    {
                        this._value = _checkpoint;
                        Monitor.Enter(this._incoming_lock);
                        _incoming_locked = true;
                    }

                    lock (this._valueendpoint)
                    {
                        if (this._valueendpoint.IsConnected)
                            this._valueendpoint.Interface.Set(this._value);
                    }
                }
                finally
                {
                    if (_incoming_locked)
                        Monitor.Exit(this._incoming_lock);
                }
            }
        }

        ValueClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<ValueClass, ValueClass>.Checkpoint()
        {
            lock (this)
            {
                return this._value;
            }
        }

        #endregion
    }
}
