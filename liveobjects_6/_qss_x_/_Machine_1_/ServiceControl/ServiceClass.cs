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

namespace QS._qss_x_._Machine_1_.ServiceControl
{
    public sealed class ServiceClass : QS.Fx.Serialization.ISerializable
    {
        public ServiceClass(Type persistentStateInterfaceType, Type persistentStateClassType, Type persistentStateOperationType,
            Type nonpersistentStateInterfaceType, Type nonpersistentStateClassType, Type nonpersistentStateOperationType, Type serviceType)
        {
            this.persistentStateInterfaceType = persistentStateInterfaceType;
            this.persistentStateClassType = persistentStateClassType;
            this.persistentStateOperationType = persistentStateOperationType;
            this.nonpersistentStateInterfaceType = nonpersistentStateInterfaceType;
            this.nonpersistentStateClassType = nonpersistentStateClassType;
            this.nonpersistentStateOperationType = nonpersistentStateOperationType;
        }

        public ServiceClass()
        {
        }

        private Type persistentStateInterfaceType, persistentStateClassType, persistentStateOperationType,
            nonpersistentStateInterfaceType, nonpersistentStateClassType, nonpersistentStateOperationType, serviceType;

        #region Accessors

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Machine_ServiceControl_ServiceClass);

                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            
        }

        #endregion
    }
}
