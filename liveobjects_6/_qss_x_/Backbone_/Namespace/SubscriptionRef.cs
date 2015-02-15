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

namespace QS._qss_x_.Backbone_.Namespace
{
    public sealed class SubscriptionRef : QS._qss_x_.Namespace_.IObject
    {
        #region Constructor

        public SubscriptionRef(ControllerRef controllerref, Controller.ISubscription subscription, ulong namespaceidentifier)
        {
            this.controllerref = controllerref;
            this.namespaceidentifier = namespaceidentifier;
            this.subscription = subscription;
        }

        #endregion

        #region Fields

        private ControllerRef controllerref;
        private ulong namespaceidentifier;
        private Controller.ISubscription subscription;

        #endregion

        #region IObject Members

        ulong QS._qss_x_.Namespace_.IObject.Identifier
        {
            get { return namespaceidentifier; }
        }

        QS._qss_x_.Namespace_.Category QS._qss_x_.Namespace_.IObject.Category
        {
            get { return QS._qss_x_.Namespace_.Category.Connection; }
        }

        string QS._qss_x_.Namespace_.IObject.Name
        {
            get { return "subscription of client \"" + subscription.Client.Name + "\" to topic \"" + subscription.Topic.Name + "\""; }
        }

        bool QS._qss_x_.Namespace_.IObject.IsFolder
        {
            get { return false; }
        }

        IEnumerable<QS._qss_x_.Namespace_.IAction> QS._qss_x_.Namespace_.IObject.Actions
        {
            get { return new QS._qss_x_.Namespace_.IAction[] { new QS._qss_x_.Namespace_.Action(1, "unsubscribe", 0) }; }
        }

        bool QS._qss_x_.Namespace_.IObject.Invoke(ulong identifier, ulong context)
        {
            switch (identifier)
            {
                case 1:
                    controllerref.Unsubscribe(subscription);
                    break;

                default:
                    break;
            }

            return true;
        }

        #endregion
    }
}
