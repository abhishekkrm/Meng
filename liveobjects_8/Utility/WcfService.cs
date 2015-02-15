/*

Copyright (c) 2004-2009 Saad Sami. All rights reserved.

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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

using QS._qss_x_.Interfaces_.Classes_;

namespace QS._qss_x_.Utility.Classes_
{
    
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public sealed class WcfService
        : IWcfChannel_
    {
        IWcfChannelCallback_ cb = null;
        QS.Fx.Object.IContext mycontext = null;

        private List<IWcfChannelCallback_> clientCallbacks = new List<IWcfChannelCallback_>();

        /*
        private Dictionary<int, IWcfChannelCallback_> clientCallbacks =
            new Dictionary<int, IWcfChannelCallback_>();
        */

        public WcfService(QS.Fx.Object.IContext _ctxt)
        {
            this.mycontext = _ctxt;
        }

        void IWcfChannel_.Submit_Update(ushort classid, byte[] header, byte[] data)
        {
            mycontext.Platform.Logger.Log("message rcv'd: classid: " + classid + " header length: " + header.Length + " data length: " + data.Length);
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<QS._qss_x_.Interfaces_.Classes_.IWcfChannelCallback_>();

                foreach (var c in clientCallbacks)
                {
                    if (c != cb)
                    {
                        c.Update(classid, header, data);
                    }
                }
            }
        }

        void IWcfChannel_.Checkpoint(ushort classid, byte[] header, byte[] data)
        {
            //TODO
        }

        void IWcfChannel_.Connect()
        {
            mycontext.Platform.Logger.Log("Client Attempting to Connect...");
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<QS._qss_x_.Interfaces_.Classes_.IWcfChannelCallback_>();
                if (!clientCallbacks.Contains(cb))
                {
                    mycontext.Platform.Logger.Log("New Client Added.");
                    clientCallbacks.Add(cb);
                }
            }
        }

        void IWcfChannel_.Disconnect()
        {
            OperationContext context = OperationContext.Current;
            if (context.InstanceContext.State == CommunicationState.Opened)
            {
                cb = context.GetCallbackChannel<QS._qss_x_.Interfaces_.Classes_.IWcfChannelCallback_>();
                mycontext.Platform.Logger.Log("Client Disconnecting...");
                clientCallbacks.Remove(cb);
                if (clientCallbacks.Contains(cb)){
                    mycontext.Platform.Logger.Log("Disconnect Failed.");
                }
            }
        }
         
    }
         
}
