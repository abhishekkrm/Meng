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
using System.Timers;
using System.IO;

namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass("165B18DC24BA475d84F99B67EAFD27A3")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_D : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Writer")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_N> Writer
        {
            get;
        }
    }
}

namespace QS.Fx.Interface.Classes
{
    
    [QS.Fx.Reflection.InterfaceClass("C1D29D7ED2534e4fA1D76F75F73165EB")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_D : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Write")]
        void Write(byte[] block);

        
    }
}

namespace QS._qss_x_.Properties_.Component_
{
    #if SYNC
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
    [QS.Fx.Reflection.ComponentClass("1B29C84F43044a6e8EA4B45FA079F1E4", "MCexp9_D")]
    [QS._qss_x_.Reflection_.Internal]
    public class MCexp9_D
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IMCexp9_N, QS.Fx.Object.Classes.IMCexp9_N
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MCexp9_D
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("out file", QS.Fx.Reflection.ParameterClass.Value)] string _path,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] bool _debug,
            [QS.Fx.Reflection.Parameter("block size", QS.Fx.Reflection.ParameterClass.Value)] int _block_size
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + ".Constructor");
#endif
            this._writer_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_N>(this);
            try
            {
                this._out_file = new FileStream(System.IO.Path.GetFullPath(_path), FileMode.Create, FileAccess.Write, FileShare.None, _block_size , false);
            }
            catch (Exception e)
            {
                throw new Exception("Could not open " + _path + " for writing.", e);
            }


            this._writer_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Connect);



        }

        #endregion

        #region Fields



        [QS.Fx.Base.Inspectable]
        private const string _component_name = "MCexp9_D";

        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Value.Tuple_<int,byte[]>> _incoming_plaintext = new Queue<QS.Fx.Value.Tuple_<int,byte[]>>();

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_N> _writer_endpoint;

        [QS.Fx.Base.Inspectable]
        private System.Text.ASCIIEncoding str_enc = new System.Text.ASCIIEncoding();
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;
        [QS.Fx.Base.Inspectable]
        private FileStream _out_file;
        [QS.Fx.Base.Inspectable]
        private double _end_time;
        [QS.Fx.Base.Inspectable]
        private long _buffered_bytes = 0;

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

        private void _Connect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Connect ");
#endif

            
            // start sending
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

        #region _Write_2
        #if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
 
#endif
        void _Write_2()
        {
            if (this._incoming_plaintext.Count > 0)
            {
                lock (this._incoming_plaintext)
                {
                    byte[] block = _incoming_plaintext.Dequeue().y;

                    if (block == null)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Write_2 : Received EOF");
#endif
                        _end_time = _mycontext.Platform.Clock.Time;
                        this._writer_endpoint.Interface.ReportEndTime(_end_time);
                        _out_file.Close();
                        return;
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Write_2 : Received plaintext \"" + str_enc.GetString(block) + "\"");
#endif
                    }
                    _buffered_bytes += block.Length;
                    //_out_file.BeginWrite(block, 0, block.Length, new AsyncCallback(this._WriteCallback), null);
                    //_out_file.Write(str_enc.GetString(block));
                    //if (_buffered_bytes >= 1024 * 1024 * 2)
                    //{
                    //    _buffered_bytes = 0;
                    //    _out_file.Flush();
                    //}
                }
            }
        }

        #endregion


        #region _WriteCallback

        private void _WriteCallback(IAsyncResult result)
        {
            _out_file.EndWrite(result);



        }

        #endregion




       

      

        
        #region IMCexp9_N Members
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void QS.Fx.Interface.Classes.IMCexp9_N.Next(QS.Fx.Value.Tuple_<int, byte[]> block)
        {

            lock (this._incoming_plaintext)
            {
                _incoming_plaintext.Enqueue(block);
            }
            this._Write_2();
        }

        #endregion

        #region IMCexp9_N Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_N> QS.Fx.Object.Classes.IMCexp9_N.WriterOrNextEnc
        {
            get { return this._writer_endpoint; }
        }

        #endregion
    }
}
