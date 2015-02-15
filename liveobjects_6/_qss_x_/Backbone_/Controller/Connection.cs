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

#define DEBUG_EnableLogging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Controller
{
    public sealed class Connection : QS.Fx.Inspection.Inspectable, Node.IControllerConnection, IConnectionControl
    {
        #region Constructor

        public Connection(IControllerControl controller, Node.IControllerConnectionContext context)
        {
            this.controller = controller;
            this.context = context;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        private IControllerControl controller;
        [QS.Fx.Base.Inspectable]
        private Node.IControllerConnectionContext context;
        private bool activated;

        #endregion

        #region Node.IControllerConnection Members

        void Node.IControllerConnection.ActivateChannel()
        {
            controller.ActivateChannel(this);
        }

        void Node.IControllerConnection.Handle(Node.IIncoming message)
        {
            controller.Handle(this, message);
        }

        #endregion

        #region IConnection Members

        string IConnection.Name
        {
            get { return context.Name; }
        }

        QS.Fx.Base.ID IConnection.ID
        {
            get { return context.ID; }
        }

        #endregion

        #region IConnectionControl Members

        Node.IControllerConnectionContext IConnectionControl.Context
        {
            get { return context; }
        }

        bool IConnectionControl.IsActivated
        {
            get { return activated; }
            set { activated = value; }
        }

        #endregion
    }
}
