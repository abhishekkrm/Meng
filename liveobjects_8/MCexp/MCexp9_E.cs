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
#define DES
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace QS.Fx.Interface.Classes
{

    [QS.Fx.Reflection.InterfaceClass("EC4B24D35DF74ed6A78DA4F58B263D0D")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_E : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Next")]
        void Next(QS.Fx.Value.Tuple_<int, byte[]> block);



    }

}
namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass("4179542753DE41c786CDF2EA830632F7")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_E : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Writer or Next Encrypter")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_E> WriterOrNextEnc
        {
            get;
        }
    }
}

namespace QS._qss_x_.Properties_.Component_
{
#if SYNC
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
    [QS.Fx.Reflection.ComponentClass("AC681B8F717348c78062ACFB181D7AE9", "MCexp9_E")]
    [QS._qss_x_.Reflection_.Internal]
    public class MCexp9_E
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IMCexp9_E, QS.Fx.Interface.Classes.IMCexp9_E, QS.Fx.Interface.Classes.IMCexp9_A
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MCexp9_E
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("decrypter", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IMCexp9_C> _dec,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + ".Constructor");
#endif
            this._enc_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_E>(this);
            this._dec_endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IMCexp9_C, QS.Fx.Interface.Classes.IMCexp9_A>(this);
#if DES
            this._rij_alg = new DESCryptoServiceProvider();
            
#else
            this._rij_alg = Rijndael.Create();
#endif
            this._enc_transform = _rij_alg.CreateEncryptor(_rij_alg.Key, _rij_alg.IV);

            this._ciphertext_stream = new MemoryStream();
            this._encrypter = new CryptoStream(this._ciphertext_stream, this._enc_transform, CryptoStreamMode.Write);

            this._dec_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Connect);
            this._dec_obj = _dec.Dereference(_mycontext);

            if (_dec != null)
                this._dec_connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._dec_endpoint).Connect(_dec_obj.Decrypter);



            this._block_size = _block_size;
        }

        #endregion

        #region Fields
        [QS.Fx.Base.Inspectable]
        private int _num_chunks_handled;
        [QS.Fx.Base.Inspectable]
        QS.Fx.Object.Classes.IMCexp9_C _dec_obj;
        [QS.Fx.Base.Inspectable]
        long _last_pos = 0;

        [QS.Fx.Base.Inspectable]
#if DES
        DESCryptoServiceProvider _rij_alg;
#else
        Rijndael _rij_alg;
#endif
        [QS.Fx.Base.Inspectable]
        ICryptoTransform _enc_transform;
        [QS.Fx.Base.Inspectable]
        MemoryStream _ciphertext_stream;
        [QS.Fx.Base.Inspectable]
        CryptoStream _encrypter;

        [QS.Fx.Base.Inspectable]
        int _block_size;
        [QS.Fx.Base.Inspectable]
        private const string _component_name = "MCexp9_e";



        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_E> _enc_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IMCexp9_C, QS.Fx.Interface.Classes.IMCexp9_A> _dec_endpoint;

        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Value.Tuple_<int, byte[]>> _incoming_plaintext = new Queue<QS.Fx.Value.Tuple_<int, byte[]>>();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _dec_connection;
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;
        [QS.Fx.Base.Inspectable]
        private bool _dec_connected = false;
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
                if (this._dec_endpoint.IsConnected)
                    this._dec_endpoint.Disconnect();


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
            _dec_connected = true;
            this._dec_endpoint.Interface.ShareKeys(_rij_alg.Key, _rij_alg.IV);
            while (_incoming_plaintext.Count > 0)
            {
                this._Encrypt_2();
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


        #region _Encrypt_2
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void _Encrypt_2()
        {
            if (_incoming_plaintext.Count > 0 && _dec_connected)
            {
                lock (this)
                {
                    QS.Fx.Value.Tuple_<int, byte[]> block = _incoming_plaintext.Dequeue();
                    _num_chunks_handled++;
                    if (block.y == null)
                    {
                        // EOF
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Encrypt_2 : Received EOF");
#endif
                        this._dec_endpoint.Interface.Decrypt(block);
                        return;
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Component_." + _component_name + "._Encrypt_2 : Received plaintext \"" + str_enc.GetString(block.y) + "\"");
#endif
                    }
                    foreach (byte b in block.y)
                    {
                        this._encrypter.WriteByte(b);
                    }

                    long _num_cbytes = this._ciphertext_stream.Length - this._last_pos;
                    byte[] arr = this._ciphertext_stream.ToArray();
                    byte[] arr2 = new byte[_num_cbytes];
                    Array.Copy(arr, this._last_pos, arr2, 0, _num_cbytes);
                    block.y = arr2;
                    this._last_pos = this._ciphertext_stream.Length;
                    if (this._ciphertext_stream.Length >= 1024 * 1024 * 25)
                    {
                        this._last_pos = 0;
                        this._ciphertext_stream.Close();
                        this._ciphertext_stream = new MemoryStream();
                        this._encrypter = new CryptoStream(this._ciphertext_stream, this._enc_transform, CryptoStreamMode.Write);
                    }
                    this._dec_endpoint.Interface.Decrypt(block);



                }


            }


        }

        #endregion




        #region IMCexp9_A Members

        void QS.Fx.Interface.Classes.IMCexp9_A.ReportEndTime(double _end_time)
        {
            this._enc_endpoint.Interface.ReportEndTime(_end_time);
        }

        #endregion


        #region IMCexp9_A Members

#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void QS.Fx.Interface.Classes.IMCexp9_A.Cycle(QS.Fx.Value.Tuple_<int, byte[]> block)
        {
            lock (this._incoming_plaintext)
            {
                _incoming_plaintext.Enqueue(block);
            }

            this._Encrypt_2();
        }

        #endregion

        #region IMCexp9_N Members
#if SYNC
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
#endif
        void QS.Fx.Interface.Classes.IMCexp9_E.Next(QS.Fx.Value.Tuple_<int, byte[]> block)
        {

            lock (this._incoming_plaintext)
            {
                _incoming_plaintext.Enqueue(block);
            }

            this._Encrypt_2();
        }

        #endregion

        #region IMCexp9_N Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_E> QS.Fx.Object.Classes.IMCexp9_E.WriterOrNextEnc
        {
            get { return this._enc_endpoint; }
        }

        #endregion
    }
}
