/*

Copyright (c) 2010 Bo Peng. All rights reserved.

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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quilt.PubsubApp
{
    [QS.Fx.Reflection.ComponentClass("AF49E2EB3CC1420c918FE9648BA260CB", "PubSubOpsSample", "a simple PubSubOps")]
    public sealed class PubSubOpsSample : 
        QS._qss_x_.Properties_.Component_.Base_, 
        QS.Fx.Interface.Classes.IPubSubOps,
        QS.Fx.Object.Classes.IPubSub
    {
        #region Constructor

        public PubSubOpsSample(QS.Fx.Object.IContext _context
            )
            : base(_context, true)
        {
            this.mycontext = _context;
            this.pubsubEndpoint = this.mycontext.DualInterface<QS.Fx.Interface.Classes.IPubSubClient, QS.Fx.Interface.Classes.IPubSubOps>(this);
            this.pubsubEndpoint.OnConnected += new QS.Fx.Base.Callback(delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.Connect))); });
            this.pubsubEndpoint.OnDisconnect += new QS.Fx.Base.Callback(delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.Disconnect))); });
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext mycontext;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IPubSubClient, QS.Fx.Interface.Classes.IPubSubOps> pubsubEndpoint;

        #endregion

        #region Connect & Disconnect

        private void Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            //start to subscribe when connected
        }

        private void Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            //start to unsubscribe when disconnected
        }

        #endregion

        #region IPubSubOps Members

        void QS.Fx.Interface.Classes.IPubSubOps.Subscribe(string group_id, double quality, bool is_publisher)
        {
            this._logger.Log("Subscribe to: " + group_id + "with quality " + Convert.ToString(quality) + "is publisher: " + Convert.ToString(is_publisher));
            this.pubsubEndpoint.Interface.Subscribed(group_id, is_publisher);
        }

        void QS.Fx.Interface.Classes.IPubSubOps.UnSubscribe(string group_id)
        {
            this._logger.Log("UnSubscribe to: " + group_id);
        }

        void QS.Fx.Interface.Classes.IPubSubOps.Publish(string group_id, QS.Fx.Serialization.ISerializable data)
        {
            this._logger.Log("Publish: " + group_id + "Data: " + data.ToString());
        }

        #endregion

        #region IPubSub Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IPubSubClient, QS.Fx.Interface.Classes.IPubSubOps> QS.Fx.Object.Classes.IPubSub.PubSub
        {
            get { return this.pubsubEndpoint; }
        }

        #endregion
    }
}
