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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Coordinates_2, "Coordinates_2", "Coordinates of a moving object.")]
    public sealed class Coordinates_2
        : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>, 
        QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>, IDisposable
    {
        #region Constructor

        public Coordinates_2(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("PX", QS.Fx.Reflection.ParameterClass.Value)] double _px,
            [QS.Fx.Reflection.Parameter("PY", QS.Fx.Reflection.ParameterClass.Value)] double _py,
            [QS.Fx.Reflection.Parameter("PZ", QS.Fx.Reflection.ParameterClass.Value)] double _pz,
            [QS.Fx.Reflection.Parameter("RX", QS.Fx.Reflection.ParameterClass.Value)] double _rx,
            [QS.Fx.Reflection.Parameter("RY", QS.Fx.Reflection.ParameterClass.Value)] double _ry,
            [QS.Fx.Reflection.Parameter("RZ", QS.Fx.Reflection.ParameterClass.Value)] double _rz,
            [QS.Fx.Reflection.Parameter("R", QS.Fx.Reflection.ParameterClass.Value)] double _r,
            [QS.Fx.Reflection.Parameter("T", QS.Fx.Reflection.ParameterClass.Value)] double _t,
            [QS.Fx.Reflection.Parameter("S", QS.Fx.Reflection.ParameterClass.Value)] double _s)
        {
            this._px = _px;
            this._py = _py;
            this._pz = _pz;
            this._r = _r;
            this._t = _t;
            this._s = _s;
            this._rx = _rx;
            this._ry = _ry;
            this._rz = _rz;

            this._coordinatesendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>, 
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>>(this);

            this._coordinatesendpoint.OnConnect += new QS.Fx.Base.Callback(this._CoordinatesConnectCallback);
            this._coordinatesendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._CoordinatesDisconnectCallback);

            this._RecalculateCoordinates();
        }

        #endregion

        #region Finalizer

        ~Coordinates_2()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                System.Threading.Thread _thread;
                lock (this)
                {
                    this._exiting = true;
                    if (this._exitingevent != null)
                        this._exitingevent.Set();
                    _thread = this._thread;
                }
                if (_thread != null)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this._ThreadJoinCallback), _thread);
                else
                    _ThreadJoinCallback(null);
            }
        }

        #endregion

        #region _ThreadJoinCallback

        private void _ThreadJoinCallback(object _o)
        {
            if (_o != null)
            {
                System.Threading.Thread _thread = (System.Threading.Thread)_o;
                _thread.Join();
            }
            lock (this)
            {
                this._thread = null;
                this._exitingevent = null;
                this._exiting = false;
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinatesendpoint;

        private System.Threading.ManualResetEvent _exitingevent;
        private bool _exiting;
        private System.Threading.Thread _thread;
        private QS._qss_x_.Channel_.Message_.Coordinates _coordinates;
        private double _px, _py, _pz, _r, _t, _s, _rx, _ry, _rz;

        #endregion

        #region _CoordinatesConnectCallback

        private void _CoordinatesConnectCallback()
        {
            lock (this)
            {
                this._RecalculateCoordinates();
                if (this._s > 0)
                {
                    this._exitingevent = new System.Threading.ManualResetEvent(false);
                    this._exiting = false;
                    this._thread = new System.Threading.Thread(new System.Threading.ThreadStart(this._ThreadCallback));
                    this._thread.Start();
                }
            }
        }

        #endregion

        #region _CoordinatesDisconnectCallback

        private void _CoordinatesDisconnectCallback()
        {
            Dispose(true);
        }

        #endregion

        #region _ThreadCallback

        private void _ThreadCallback()
        {
            lock (this)
            {
                while (!this._exiting)
                {
                    Monitor.Exit(this);
                    try
                    {
                        this._RecalculateCoordinates();
                    }
                    finally
                    {
                        Monitor.Enter(this);
                    }
                    WaitHandle _e = this._exitingevent;
                    Monitor.Exit(this);
                    try
                    {
                        _e.WaitOne(30, false);
                    }
                    finally
                    {
                        Monitor.Enter(this);
                    }
                }
            }
        }

        #endregion

        #region _RecalculateCoordinates

        private readonly DateTime _referencetime = DateTime.Today;
        private void _RecalculateCoordinates()
        {
            double _time = (DateTime.Now - _referencetime).TotalSeconds;
            double _a = (_time + this._t) * this._s * 2 * Math.PI;

            QS._qss_x_.Channel_.Message_.Coordinates _coordinates = null;
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates> _callbackinterface = null;
            lock (this)
            {
                this._coordinates = 
                    new QS._qss_x_.Channel_.Message_.Coordinates
                    (
                        (float) _time,
                        (float)(this._px + this._r * Math.Cos(_a)), (float)(this._py + this._r * Math.Sin(_a)), (float) this._pz,
                        0f, 0f, 0f,
                        (float) this._rx, (float) this._ry, ((float) this._rz) + ((float) _a),
                        0f, 0f, 0f
                    );

                if (this._coordinatesendpoint.IsConnected)
                {
                    _coordinates = this._coordinates;
                    _callbackinterface = this._coordinatesendpoint.Interface;
                }
            }

            if (_callbackinterface != null)
                _callbackinterface.Set(_coordinates);
        }

        #endregion

        #region IValue<ICoordinates> Members

        QS._qss_x_.Channel_.Message_.ICoordinates QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Get()
        {
            return this._coordinates;
        }

        void QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Set(QS._qss_x_.Channel_.Message_.ICoordinates _value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IValue<ICoordinates> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>, 
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> 
                QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Endpoint
        {
            get { return this._coordinatesendpoint; }
        }

        #endregion
    }
}
