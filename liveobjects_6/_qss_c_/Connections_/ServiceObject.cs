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

// #define DEBUG_ServiceObject

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Connections_
{
    public class ServiceObject : IAsynchronousObject
    {
        public ServiceObject(QS.Fx.Logging.ILogger logger, object targetObject)
        {
            this.logger = logger;
            this.targetObject = targetObject;
        }        

        private QS.Fx.Logging.ILogger logger;
        private object targetObject;

        #region Internal Processing

        private object Dispatch(QS.Fx.Serialization.ISerializable argumentObject)
        {
            MethodCall methodCall = argumentObject as MethodCall;
            if (methodCall != null)
            {
#if DEBUG_ServiceObject
                logger.Log(this, "Call : " + methodCall.MethodName + "(" +
                    Helpers.CollectionHelper.ToStringSeparated<object>(methodCall.Arguments, ", ") + ")");
#endif

                string methodName = methodCall.MethodName;
                object[] arguments = methodCall.Arguments;

                System.Reflection.MethodInfo methodInfo = targetObject.GetType().GetMethod(methodName);
                if (methodInfo == null)
                    throw new Exception("Method \"" + methodName + "\" not found in object " + QS._core_c_.Helpers.ToString.Object(targetObject));

                try
                {
                    return methodInfo.Invoke(targetObject, arguments);
                }
                catch (Exception exc)
                {
                    throw new Exception("An exception was thrown while executing method \"" + methodName + "\".", exc);
                }
            }
            else
                throw new Exception("Received message of a wrong type.");
        }

        #endregion

        #region IAsynchronousObject Members

        void IAsynchronousObject.AsynchronousCall(QS.Fx.Serialization.ISerializable argumentObject)
        {
            try
            {
                this.Dispatch(argumentObject);
            }
            catch (Exception exc)
            {
                logger.Log(this, "Cannot dispatch the call.\n" + exc.ToString());
            }
        }

        void IAsynchronousObject.AsynchronousCall(QS.Fx.Serialization.ISerializable argumentObject, ResponseCallback responseCallback)
        {
            try
            {
                object responseObject = this.Dispatch(argumentObject);

#if DEBUG_ServiceObject
                logger.Log(this, "Response : " + Helpers.ToString.Object(responseObject));
#endif

                responseCallback(true, MethodCall.EncodeObject(responseObject), null);
            }
            catch (Exception exc)
            {
                responseCallback(false, null, exc);
            }
        }

        #endregion
    }
}
