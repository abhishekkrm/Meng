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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public struct Submission
    {
        public Submission(ServiceControl.IServiceControllerOperation operation,
            QS.Fx.Base.ContextCallback<ServiceControl.IServiceControllerOperation, object> operationCompletionCallback, object callbackContext)
        {
            this.operation = operation;
            this.operationCompletionCallback = operationCompletionCallback;
            this.callbackContext = callbackContext;
        }

        [QS.Fx.Printing.Printable]
        private ServiceControl.IServiceControllerOperation operation;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Base.ContextCallback<ServiceControl.IServiceControllerOperation, object> operationCompletionCallback;
        [QS.Fx.Printing.Printable]
        private object callbackContext;

        #region Accessors

        public ServiceControl.IServiceControllerOperation Operation
        {
            get { return operation; }
        }

        public QS.Fx.Base.ContextCallback<ServiceControl.IServiceControllerOperation, object> OperationCompletionCallback
        {
            get { return operationCompletionCallback; }
        }

        public object CallbackContext
        {
            get { return callbackContext; }
        }

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }
    }
}
