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

namespace QS._qss_x_.Backbone_.Controller
{
    public sealed class _Test : QS.Fx.Inspection.Inspectable, Node.IController
    {
        #region Constructor

        public _Test(Node.IControllerContext context)
        {
            this.context = context;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        private Node.IControllerContext context;

        #endregion

        #region Node.IController Members

        Node.IControllerConnection Node.IController.Create(Node.IControllerConnectionContext context)
        {
            return new Connection(this, context);
        }

        #endregion

        #region Class Connection

        private sealed class Connection : Node.IControllerConnection
        {
            #region Constructor

            public Connection(_Test controller, Node.IControllerConnectionContext context)
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

            private _Test controller;
            private Node.IControllerConnectionContext context;

            #endregion

            #region Node.IControllerConnection Members

            void Node.IControllerConnection.ActivateChannel()
            {
                controller.context.Logger.Log("ActivateChannel");

                if (context.IsSuper)
                {
                    for (int ind = 0; ind < 10; ind++)
                    {
                        context.RequestChannel.Submit(
                            new QS._core_c_.Base2.StringWrapper(
                                "Hello (" + ind.ToString() + ") from \"" + controller.context.Name + "\" to \"" + context.Name + "\"."),
                                    Node.MessageOptions.Acknowledge | Node.MessageOptions.Respond,
                                        new QS.Fx.Base.ContextCallback<Node.IOutgoing>(this._MessageCallback), null);
                    }
                }
            }

            void Node.IControllerConnection.Handle(Node.IIncoming message)
            {
                controller.context.Logger.Log("Handle : \n" + QS.Fx.Printing.Printable.ToString(message.Message));

                message.ResponseChannel.Submit(
                    new QS._core_c_.Base2.StringWrapper("Response To : " + ((QS._core_c_.Base2.StringWrapper) message.Message).String),
                        Node.MessageOptions.Acknowledge,
                            new QS.Fx.Base.ContextCallback<Node.IOutgoing>(this._MessageCallback), null);
            }

            #endregion

            #region _MessageCallback

            private void _MessageCallback(Node.IOutgoing message)
            {
                controller.context.Logger.Log("_MessageCallback : \n" + QS.Fx.Printing.Printable.ToString(message));
            }

            #endregion
        }

        #endregion
    }
}
