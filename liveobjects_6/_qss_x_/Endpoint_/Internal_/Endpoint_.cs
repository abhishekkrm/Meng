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
using System.Threading;

namespace QS._qss_x_.Endpoint_.Internal_
{
    internal abstract class Endpoint_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Endpoint_.Internal_.IEndpoint_
    {
        #region Constructor

        protected Endpoint_(QS._qss_x_.Endpoint_.Internal_.Endpoint_ _internalendpoint)
        {
            this._id = Interlocked.Increment(ref QS._qss_x_.Endpoint_.Internal_.Endpoint_._lastid);
            this._internalendpoint = (_internalendpoint != null) ? _internalendpoint : this;
        }

        #endregion

        #region Fields

        private static long _lastid;

        [QS.Fx.Base.Inspectable]
        private long _id;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Endpoint_.Internal_.Endpoint_ _internalendpoint;
        [QS.Fx.Base.Inspectable]
        private bool _isconnected;
        [QS.Fx.Base.Inspectable]
        private Connection_ _connection;
        
        private event QS.Fx.Base.Callback _onconnect, _onconnected, _ondisconnect;

        #endregion

        #region _Start and _Stop

        protected virtual void _Start(Endpoint_ _other)
        {
        }

        protected virtual void _Stop(Endpoint_ _other)
        {
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return InterfaceClass_.Unknown_; }
        }

        #endregion

        #region QS.Fx.Endpoint.Classes.IEndpoint Members

        QS.Fx.Endpoint.IConnection QS.Fx.Endpoint.Classes.IEndpoint.Connect(QS.Fx.Endpoint.Classes.IEndpoint _other)
        {
            if (_other == null)
                throw new Exception("Cannot connect this endpoint because the reference to the other endpoint is null.");

            QS._qss_x_.Endpoint_.Internal_.Endpoint_ _otherendpoint = _other as QS._qss_x_.Endpoint_.Internal_.Endpoint_;
            if (_otherendpoint == null)
                throw new Exception(
                    "Cannot connect this endpoint because the implementation of the other endpoint is unrecognized by the runtime.");

            Endpoint_ _e1, _e2;
            if (this._id < _otherendpoint._id)
            {
                _e1 = this;
                _e2 = _otherendpoint;
            }
            else
            {
                _e1 = _otherendpoint;
                _e2 = this;
            }

            Endpoint_ _o1, _o2;
            _o1 = _e1._internalendpoint;
            _o2 = _e2._internalendpoint;

            lock (_o1)
            {
                lock (_o2)
                {
                    if (_o1._isconnected || _o2._isconnected)
                        throw new Exception("Cannot connect endpoints because one of them is already connected to some other endpoint.");

                    _o1._isconnected = true;
                    _o2._isconnected = true;

                    Connection_ _connection = new Connection_(_e1, _e2);
                    
                    _o1._connection = _connection;
                    _o2._connection = _connection;

                    bool _done_1 = false;
                    bool _done_2 = false;
                    bool _done_3 = false;
                    bool _done_4 = false;

                    try
                    {
                        _e1._Start(_e2);
                        _done_1 = true;

                        _e2._Start(_e1);
                        _done_2 = true;

                        QS.Fx.Base.Callback _callback;
                        
                        _callback = _o1._onconnect;
                        if (_callback != null)
                        {
                            _callback();
                            _done_3 = true;
                        }

                        _callback = _o2._onconnect;
                        if (_callback != null)
                        {
                            _callback();
                            _done_4 = true;
                        }

                        _callback = _o1._onconnected;
                        if (_callback != null)
                            _callback();

                        _callback = _o2._onconnected;
                        if (_callback != null)
                            _callback();                        
                    }
                    catch (Exception exc)
                    {
                        QS.Fx.Base.Callback _callback;

                        if (_done_4)
                        {
                            _callback = _o2._ondisconnect;
                            if (_callback != null)
                            {
                                try
                                {
                                    _callback();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                        if (_done_3)
                        {
                            _callback = _o1._ondisconnect;
                            if (_callback != null)
                            {
                                try
                                {
                                    _callback();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                        if (_done_2)
                        {
                            try
                            {
                                _e2._Stop(_e1);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (_done_1)
                        {
                            try
                            {
                                _e1._Stop(_e2);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        _o2._connection = null;
                        _o1._connection = null;

                        _o2._isconnected = false;
                        _o1._isconnected = false;

                        throw new Exception("Could not connect endpoints.", exc);
                    }

                    return _connection;
                }
            }
        }

        #endregion

        #region QS.Fx.Endpoint.Internal.IEndpoint Members

        void QS.Fx.Endpoint.Internal.IEndpoint.Disconnect()
        {
            Connection_ _connection = this._internalendpoint._connection;
            if (_connection != null)
                ((IDisposable) _connection).Dispose();
        }

        bool QS.Fx.Endpoint.Internal.IEndpoint.IsConnected
        {
            get { return this._internalendpoint._isconnected; }
        }

        event QS.Fx.Base.Callback QS.Fx.Endpoint.Internal.IEndpoint.OnConnect
        {
            add
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._onconnect += value;
                }
            }

            remove
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._onconnect -= value;
                }
            }
        }

        event QS.Fx.Base.Callback QS.Fx.Endpoint.Internal.IEndpoint.OnConnected
        {
            add
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._onconnected += value;
                }
            }

            remove
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._onconnected -= value;
                }
            }
        }

        event QS.Fx.Base.Callback QS.Fx.Endpoint.Internal.IEndpoint.OnDisconnect
        {
            add
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._ondisconnect += value;
                }
            }

            remove
            {
                lock (this._internalendpoint)
                {
                    this._internalendpoint._ondisconnect -= value;
                }
            }
        }

        #endregion

        #region Class Connection_
        
        private sealed class Connection_ : QS._qss_x_.Endpoint_.Internal_.IConnection_
        {
            #region Constructor

            internal Connection_(Endpoint_ _e1, Endpoint_ _e2)
            {
                this._e1 = _e1;
                this._e2 = _e2;
            }

            #endregion

            #region Fields

            private Endpoint_ _e1, _e2;
            private bool _isdisconnected;

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                lock (this)
                {
                    if (!this._isdisconnected)
                    {
                        _isdisconnected = true;

                        Endpoint_ _o1 = _e1._internalendpoint;
                        Endpoint_ _o2 = _e2._internalendpoint;

                        lock (_o1)
                        {
                            lock (_o2)
                            {
                                QS.Fx.Base.Callback _callback;

                                _callback = _o2._ondisconnect;
                                if (_callback != null)
                                {
                                    try
                                    {
                                        _callback();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                _callback = _o1._ondisconnect;
                                if (_callback != null)
                                {
                                    try
                                    {
                                        _callback();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                try
                                {
                                    _e2._Stop(_e1);
                                }
                                catch (Exception)
                                {
                                }

                                try
                                {
                                    _e1._Stop(_e2);
                                }
                                catch (Exception)
                                {
                                }

                                _o2._connection = null;
                                _o1._connection = null;

                                _o2._isconnected = false;
                                _o1._isconnected = false;
                            }
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
