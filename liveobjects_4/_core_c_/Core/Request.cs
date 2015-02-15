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

namespace QS._core_c_.Core
{
/*
    public sealed class Request<C> : Core.IRequest
    {
        #region Constructor

        public Request(QS.Fx.Base.ContextCallback<C> callback, C context)
        {
            this.callback = callback;
            this.context = context;
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ContextCallback<C> callback;
        private C context;
        private Core.IRequest next;

        #endregion

        #region IRequest Members

        void QS._core_c_.Core.IRequest.Process()
        {
            callback(context);
        }

        #endregion

        #region IItem<IRequest> Members

        QS._core_c_.Core.IRequest QS._core_c_.Synchronization.IItem<QS._core_c_.Core.IRequest>.Next
        {
            get { return next; }
            set { next = value; }
        }

        #endregion
    }

    public sealed class Request<C1, C2> : Core.IRequest
    {
        #region Constructor

        public Request(QS.Fx.Base.ContextCallback<C1, C2> callback, C1 context1, C2 context2)
        {
            this.callback = callback;
            this.context1 = context1;
            this.context2 = context2;
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ContextCallback<C1,C2> callback;
        private C1 context1;
        private C2 context2;
        private Core.IRequest next;

        #endregion

        #region IRequest Members

        void QS._core_c_.Core.IRequest.Process()
        {
            callback(context1, context2);
        }

        #endregion

        #region IItem<IRequest> Members

        QS._core_c_.Core.IRequest QS._core_c_.Synchronization.IItem<QS._core_c_.Core.IRequest>.Next
        {
            get { return next; }
            set { next = value; }
        }

        #endregion
    }
*/
}
