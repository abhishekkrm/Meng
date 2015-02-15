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
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;

namespace QS._qss_x_.Uplink_
{
/*
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public abstract class Controller_ : IController, IController_
    {
        #region Constructor

        protected Controller_(int _port)
        {
            this._servicehost = new ServiceHost(this);
            WSHttpBinding _binding = new WSHttpBinding();
            _binding.Security.Mode = SecurityMode.Message;
            _binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            this._servicehost.AddServiceEndpoint(typeof(IController_), _binding, "http://localhost:" + _port);
            this._servicehost.Open();
        }

        #endregion

        #region Fields

        private ServiceHost _servicehost;
        private IDictionary<QS.Fx.Base.ID, Connection_> _connections = new Dictionary<QS.Fx.Base.ID, Connection_>();

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._servicehost.Close();
        }

        #endregion

        #region IController_ Members

        void IController_.Connect(string _name, int _capacity)
        {
            lock (this)
            {
                _connections.
            }
        }

        #endregion

        #region Class Connection_

        protected abstract class Connection_
        {
            #region Constructor

            protected Connection_(QS.Fx.Base.ID _id, string _name, int _capacity)
            {
                this._id = _id;
                this._name = _name;
                this._capacity = _capacity;
            }

            #endregion

            #region Fields

            protected QS.Fx.Base.ID _id;
            protected string _name;
            protected int _capacity;

            #endregion
        }

        #endregion
    }
*/ 
}
