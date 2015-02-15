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

#define VERBOSE
//#define PROFILE
#define SYNC

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Timers;

namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass("D2C1F906D9B94cae88B508882115B203")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_A : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Reader")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_N,QS.Fx.Interface.Classes.IMCexp9_A> Reader
        {
            get;
        }
    }
}

namespace QS.Fx.Interface.Classes
{

    [QS.Fx.Reflection.InterfaceClass("9C31CAC9638F41a2A1A4558459797FA8")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_A : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Report End Time")]
        void ReportEndTime(double _end_time);

        [QS.Fx.Reflection.Operation("Cycle")]
        void Cycle(QS.Fx.Value.Tuple_<int, byte[]> block);
    }
}

namespace QS._qss_x_.Properties_.Component_
{
#if SYNC
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
    [QS.Fx.Reflection.ComponentClass("71A1F3D40EF24075B5524430D1D8A0C7", "MCexp9_A")]
    [QS._qss_x_.Reflection_.Internal]
    public class MCexp9_A
        : QS._qss_x_.Properties_.Component_.Base_, QS.Fx.Interface.Classes.IMCexp9_A,
        QS.Fx.Object.Classes.IMCexp9_A
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MCexp9_A
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("encrypter", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IMCexp9_N>
                _enc,
            [QS.Fx.Reflection.Parameter("file", QS.Fx.Reflection.ParameterClass.Value)]
            string _in_path,
            [QS.Fx.Reflection.Parameter("block size", QS.Fx.Reflection.ParameterClass.Value)]
            int _block_size,
            [QS.Fx.Reflection.Parameter("num cycles", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_cycles,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + ".Constructor");
#endif
            buf = new byte[_block_size];
            this._num_cycles = _num_cycles;
            this._block_size = _block_size;
            this._enc_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_N, QS.Fx.Interface.Classes.IMCexp9_A>(this);

            try
            {
                this._in_file = new FileStream(System.IO.Path.GetFullPath(_in_path),FileMode.Open,FileAccess.Read,FileShare.None,_block_size,false);
            }
            catch
            {
                throw new Exception("Could not open " + _in_file + ", try using the full path");
            }


            this._enc_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Connect);
            this._enc_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Disconnect);
            this._enc_obj = _enc.Dereference(_mycontext);
            if (_enc != null)
                this._enc_connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._enc_endpoint).Connect(_enc_obj.WriterOrNextEnc);

           


        }

        #endregion

        #region Fields



        [QS.Fx.Base.Inspectable]
        private const string _component_name = "MCexp9_A";


        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_N, QS.Fx.Interface.Classes.IMCexp9_A> _enc_endpoint;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _enc_connection;
        [QS.Fx.Base.Inspectable]
        private int _num_cycles;
        [QS.Fx.Base.Inspectable]
        private System.Text.ASCIIEncoding str_enc = new System.Text.ASCIIEncoding();

        [QS.Fx.Base.Inspectable]
        private FileStream _in_file;

        [QS.Fx.Base.Inspectable]
        private int _block_size;
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;
        [QS.Fx.Base.Inspectable]
        QS.Fx.Object.Classes.IMCexp9_N _enc_obj;
        [QS.Fx.Base.Inspectable]
        private double _start_time;
        [QS.Fx.Base.Inspectable]
        private double _end_time;
        [QS.Fx.Base.Inspectable]
        private byte[] buf;
        [QS.Fx.Base.Inspectable]
        private double _elapsed;
        [QS.Fx.Base.Inspectable]
        private int count = -1;
#if PROFILE
        [QS.Fx.Base.Inspectable]

        private QS._qss_c_.Statistics_.Samples2D _statistics_recvtimes = new QS._qss_c_.Statistics_.Samples2D(
            "msg recvd times", "times messsages are received on the cc level", "msg", "", "msg #", "time", "s", "time in seconds");
        //[QS.Fx.Base.Inspectable]
        //private QS._qss_c_.Statistics_.Samples2D _statistics_dequeue = new QS._qss_c_.Statistics_.Samples2D(
        //    "enqueue times", "times it takes to enqueue", "time", "s", "time in seconds", "time", "s", "time in seconds");
#endif


        #endregion


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Dispose");
#endif

            lock (this)
            {
                if (this._enc_endpoint.IsConnected)
                    this._enc_endpoint.Disconnect();


                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Start");
#endif

            base._Start();
        }

        protected void _Start(QS.Fx.Base.Address _address)
        {
            lock (this)
            {
                //this._address = _address;
                //this._initialized = true;
                //if (this._transport_endpoint.IsConnected)
                //{
                //    this._notified = true;
                //    this._transport_endpoint.Interface.Address(_address);
                //}
                //this._ConnectChannel();


                //setup timer
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Stop");
#endif

            lock (this)
            {
                this._initialized = false;

                base._Stop();
            }
        }

        #endregion



        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        private void _Connect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Connect ");
#endif
            _start_time = _mycontext.Platform.Clock.Time;
            this._in_file.BeginRead(buf, 0, _block_size, new AsyncCallback(this._Read_callback), null);
            
            


        }

        #endregion



        #region _Read_callback
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        private void _Read_callback(IAsyncResult result)
        {
            count++;
            int _nread = _in_file.EndRead(result);
            this._enc_endpoint.Interface.Next(new QS.Fx.Value.Tuple_<int, byte[]>(_num_cycles, buf, count));
            if (_nread < _block_size)
            {
                this._enc_endpoint.Interface.Next(new QS.Fx.Value.Tuple_<int,byte[]>(_num_cycles,null,count));
            }
            else
            {
                buf = new byte[_block_size];
                this._in_file.BeginRead(buf, 0, _block_size, new AsyncCallback(this._Read_callback), null);
            }
        }

        #endregion


        #region _Disconnect

        private void _Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Disconnect");
#endif

            lock (this)
            {
                this._notified = false;
            }
        }

        #endregion














        #region IMCexp9_A Members

        void QS.Fx.Interface.Classes.IMCexp9_A.ReportEndTime(double _end_time)
        {
            this._end_time = _end_time;
            this._elapsed = _end_time - _start_time;
        }

        #endregion


        #region IMCexp9_A Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_N, QS.Fx.Interface.Classes.IMCexp9_A> QS.Fx.Object.Classes.IMCexp9_A.Reader
        {
            get { return this._enc_endpoint; }
        }

        #endregion

        #region IMCexp9_A Members


        void QS.Fx.Interface.Classes.IMCexp9_A.Cycle(QS.Fx.Value.Tuple_<int, byte[]> block)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
