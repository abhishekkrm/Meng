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

namespace QS._core_x_.Base
{
    public sealed class ParameterCallbacks<C> : QS.Fx.Base.IParameter<C>
    {
        public ParameterCallbacks(string name, SetCallback<C> setCallback, GetCallback<C> getCallback)
        {
            if ((setCallback == null) && (getCallback == null))
                throw new Exception("Both callbacks cannot be null, parameter must be readable or writable.");

            this.name = name;
            this.setCallback = setCallback;
            this.getCallback = getCallback;
        }

        public ParameterCallbacks(string name, SetCallback<C> setCallback) : this(name, setCallback, null)
        {
        }

        public ParameterCallbacks(string name, GetCallback<C> getCallback) : this(name, null, getCallback)
        {
        }

        private string name;
        private SetCallback<C> setCallback;
        private GetCallback<C> getCallback;

        #region IParameter<C> Members

        C QS.Fx.Base.IParameter<C>.Value
        {
            get 
            {
                if (getCallback != null)
                    return getCallback();
                else
                    throw new Exception("This parameter is not readable. ");
            }

            set 
            {
                if (setCallback != null)
                    setCallback(value);
                else
                    throw new Exception("This parameter is not writable.");
            }
        }

        #endregion

        #region IParameterInfo Members

        string QS.Fx.Base.IParameterInfo.Name
        {
            get { return name; }
        }

        Type QS.Fx.Base.IParameterInfo.Type
        {
            get { return typeof(C); }
        }

        bool QS.Fx.Base.IParameterInfo.Readable
        {
            get { return getCallback != null; }
        }

        bool QS.Fx.Base.IParameterInfo.Writable
        {
            get { return setCallback != null; }
        }

        #endregion

        #region IParameter Members

        object QS.Fx.Base.IParameter.Value
        {
            get
            {
                if (getCallback != null)
                    return getCallback();
                else
                    throw new Exception("This parameter is not readable. ");
            }

            set
            {
                if (setCallback != null)
                {
                    if (value is C)
                        setCallback((C)value);
                    else
                        throw new Exception("Type mismatch.");
                }
                else
                    throw new Exception("This parameter is not writable.");
            }
        }

        #endregion
    }
}
