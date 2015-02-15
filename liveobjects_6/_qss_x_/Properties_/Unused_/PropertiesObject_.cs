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

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Properties, 
        "PropertiesObject", 
        "An object that performs a dataflow-style distributed computation on a set of hierarchical properties.")]
    public sealed class PropertiesObject_ : IPropertiesObject_, IProperties_, IControllerClient_
    {
        #region Constructor

        public PropertiesObject_(
            [QS.Fx.Reflection.Parameter("controller", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<IControllerObject_> _controller_object_reference)
        {
            this._properties_endpoint = _mycontext.DualInterface<IProperties_, IProperties_>(this);
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<IProperties_, IProperties_> _properties_endpoint;

        #endregion

        #region IPropertiesObject_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<IProperties_, IProperties_> IPropertiesObject_.Properties
        {
            get { return this._properties_endpoint; }
        }

        #endregion

        #region IProperties_ Members

        void IProperties_.Value(uint _id, IVersion_ _version, IValue_ _value)
        {
        }

        #endregion

        #region IControllerClient_ Members

        void IControllerClient_.Message(IMessage_ _message)
        {
        }

        #endregion
    }
*/
}
