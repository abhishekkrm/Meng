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
using System.Security.Cryptography;
using System.Timers;

namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass("5103D89F166449b8A86FF9457C53B96E")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_C : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Decrypter")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_C> Decrypter
        {
            get;
        }
    }
}

namespace QS.Fx.Interface.Classes
{

    [QS.Fx.Reflection.InterfaceClass("16E55A96F1CF41d1B1CE6B1E7940898F")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_C : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Decrypt")]
        void Decrypt(QS.Fx.Value.Tuple_<int,byte[]> block);

        [QS.Fx.Reflection.Operation("Share Keys")]
        void ShareKeys(byte[] key, byte[] iv);

        
    }

}

namespace QS._qss_x_.Properties_.Component_
{
    #if SYNC
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
    [QS.Fx.Reflection.ComponentClass("195BAF21F0084f7cB9419A21608F2160", "MCexp9_C")]
    [QS._qss_x_.Reflection_.Internal]
    public class MCexp9_C
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IMCexp9_C, QS.Fx.Object.Classes.IMCexp9_C, QS.Fx.Interface.Classes.IMCexp9_A
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MCexp9_C
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("writer or enc", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IMCexp9_N> _writer_or_next_enc,
            [QS.Fx.Reflection.Parameter("Enc Type", QS.Fx.Reflection.ParameterClass.Value)] int _enc_type,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + ".Constructor");
#endif
            this._write_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_N,QS.Fx.Interface.Classes.IMCexp9_A>(this);
            this._dec_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_C>(this);

            switch (_enc_type)
            {

                case 1:
                    this._rij_alg = new AesCryptoServiceProvider();
                    break;
                case 2:
                    this._rij_alg = new DESCryptoServiceProvider();
                    break;
                case 3:
                    this._rij_alg = new RC2CryptoServiceProvider();
                    break;
                case 4:
                    this._rij_alg = new TripleDESCryptoServiceProvider();
                    break;
                default:
                    this._rij_alg = new RijndaelManaged();
                    break;

            }
            
            this._plaintext_stream = new MemoryStream();
            



            this._write_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Connect);
            _write_obj = _writer_or_next_enc.Dereference(_mycontext);
            if (_writer_or_next_enc != null)
                this._write_connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._write_endpoint).Connect(_write_obj.WriterOrNextEnc);

         
        }

        #endregion

        #region Fields
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IMCexp9_N _write_obj;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IMCexp9_N, QS.Fx.Interface.Classes.IMCexp9_A> _write_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IMCexp9_A,QS.Fx.Interface.Classes.IMCexp9_C> _dec_endpoint;

        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Value.Tuple_<int, byte[]>> _incoming_ciphertext = new Queue<QS.Fx.Value.Tuple_<int, byte[]>>();
        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _write_connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _dec_connection;


        [QS.Fx.Base.Inspectable]
        private const string _component_name = "MCexp9_C";


        [QS.Fx.Base.Inspectable]
        long _last_pos = 0;
        [QS.Fx.Base.Inspectable]
        SymmetricAlgorithm _rij_alg;
        [QS.Fx.Base.Inspectable]
        ICryptoTransform _enc_transform;
        [QS.Fx.Base.Inspectable]
        MemoryStream _plaintext_stream;
        [QS.Fx.Base.Inspectable]
        CryptoStream _decrypter;
        
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;

        [QS.Fx.Base.Inspectable]
        private double _startTime;

        [QS.Fx.Base.Inspectable]
        private byte[] key;
        [QS.Fx.Base.Inspectable]
        private byte[] iv;

        [QS.Fx.Base.Inspectable]
        private bool _writer_connected = false;

#if VERBOSE
        [QS.Fx.Base.Inspectable]
        private System.Text.ASCIIEncoding str_enc = new System.Text.ASCIIEncoding();
#endif


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
                if (this._write_endpoint.IsConnected)
                    this._write_endpoint.Disconnect();


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

            _writer_connected = true;
            while (this._incoming_ciphertext.Count > 0)
            {
                this._Decrypt_2();
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

        #region _Decrypt_2
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void _Decrypt_2()
        {
            if(_incoming_ciphertext.Count>0 && _writer_connected) {
                lock (this)
                {
                    QS.Fx.Value.Tuple_<int, byte[]> block = _incoming_ciphertext.Dequeue();

                    if (block.y == null)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Decrypt_2 : Received EOF");
#endif
                        // EOF
                        if (block.x == 0)
                        {
                            this._write_endpoint.Interface.Next(block);
                        }
                        else
                        {
                            block.x--;
                            this._dec_endpoint.Interface.Cycle(block);
                        }
                        return;
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Decrypt_2 : Received ciphertext \"" + str_enc.GetString(block.y) + "\"");
#endif
                    }
                    foreach (byte b in block.y)
                    {
                        this._decrypter.WriteByte(b);
                    }

                    long _num_cbytes = this._plaintext_stream.Length - this._last_pos;
                    byte[] arr = this._plaintext_stream.ToArray();
                    byte[] arr2 = new byte[_num_cbytes];
                    Array.Copy(arr, this._last_pos, arr2, 0, _num_cbytes);
                    block.y = arr2;
                    this._last_pos = this._plaintext_stream.Length;
                    if (this._plaintext_stream.Length >= 1024 * 1024 * 25)
                    {
                        this._last_pos = 0;
                        this._plaintext_stream.Close();
                        this._plaintext_stream = new MemoryStream();
                        this._decrypter = new CryptoStream(this._plaintext_stream, this._enc_transform, CryptoStreamMode.Write);
                    }
                    if (block.x == 0)
                    {
                        this._write_endpoint.Interface.Next(block);
                    }
                    else
                    {
                        block.x--;
                        this._dec_endpoint.Interface.Cycle(block);
                    }
                    
                    

                }
            }
        }

        #endregion


        #region IMCexp9_C Members
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void QS.Fx.Interface.Classes.IMCexp9_C.Decrypt(QS.Fx.Value.Tuple_<int, byte[]> block)
        {
            lock (this._incoming_ciphertext)
            {
                _incoming_ciphertext.Enqueue(block);
            }
            this._Decrypt_2();
        }

        void QS.Fx.Interface.Classes.IMCexp9_C.ShareKeys(byte[] key, byte[] iv)
        {
            this._enc_transform = _rij_alg.CreateDecryptor(key, iv);
            this._decrypter = new CryptoStream(this._plaintext_stream, this._enc_transform, CryptoStreamMode.Write);   
        }

       

        #endregion

       

        #region IMCexp9_A Members

        void QS.Fx.Interface.Classes.IMCexp9_A.ReportEndTime(double _end_time)
        {
            this._dec_endpoint.Interface.ReportEndTime(_end_time);
        }

        #endregion

        #region IMCexp9_C Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_C> QS.Fx.Object.Classes.IMCexp9_C.Decrypter
        {
            get { return this._dec_endpoint; }
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
