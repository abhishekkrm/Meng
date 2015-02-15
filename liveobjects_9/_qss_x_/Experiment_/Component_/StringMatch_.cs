/* Copyright (c) 2004-2009 Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
SUCH DAMAGE. */

//#define PROFILE_2
//#define PROFILE_3
//#define PROFILE_4

//#define USE_LIST

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("40ACA016B557401AB6192516AF9A4115")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_StringMatch)]
    [Serializable]
    [QS.Fx.Base.Inspectable]
    public sealed class StringMatch_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IStringMatch_,
        QS._qss_x_.Experiment_.Interface_.IStringMatch_, QS.Fx.Replication.IReplicated<StringMatch_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public StringMatch_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._clock = _mycontext.Platform.Clock;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IStringMatchClient_,
                    QS._qss_x_.Experiment_.Interface_.IStringMatch_>(this);

            this._workendpoint.OnConnected += new QS.Fx.Base.Callback(_workendpoint_OnConnected);
            //SymmetricAlgorithm _algo = SymmetricAlgorithm.Create();


            //this._key = _algo.Key;
            //this._iv = _algo.IV;
            //_encryptor = _algo.CreateEncryptor(_key, _iv);
            //_decryptor = _algo.CreateDecryptor(_key, _iv);
            this._encryption_init = true;
#if PROFILE_2
            this._master = true;
#endif
        }

        void _workendpoint_OnConnected()
        {
            //this._workendpoint.Interface._Set_KeyIV(_key, _iv);
        }

        public StringMatch_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IStringMatchClient_,
                QS._qss_x_.Experiment_.Interface_.IStringMatch_> _workendpoint;
#if USE_LIST
        [QS.Fx.Base.Inspectable]
        private IList<string> _result = new List<string>();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IDictionary<string, bool> _result_dict = new Dictionary<string, bool>();
#else
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, bool> _result = new Dictionary<string, bool>();
#endif
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private ICryptoTransform _encryptor, _decryptor;
        [QS.Fx.Base.Inspectable]
        private byte[] _key = null;
        [QS.Fx.Base.Inspectable]
        private byte[] _iv = null;
        [QS.Fx.Base.Inspectable]
        private bool _encryption_init = false;

        #region Encrypted Words + trickery

        // we want this data to be serialized in only one direction (master -> replica), and not on the return trip, as it
        // could be very large. 
        // Trickery occurs in _Work, where it sets the working value _enc_words to whatever it received through export, and then resets
        // _enc_words_ser to null, so it is not reserialized when Import is done to the master replica.
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private string[] _enc_words = null;
        [QS.Fx.Base.Inspectable]
        private string[] _enc_words_ser = null;

        #endregion

#if PROFILE_2
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _import_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _export_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private bool _master = false;
#endif

#if PROFILE_3
        [QS.Fx.Base.Inspectable]
        private double _sum_56 = 0;
        [QS.Fx.Base.Inspectable]
        private double _sum_12 = 0;
        [QS.Fx.Base.Inspectable]
        private double _sum_34 = 0;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples1D _sum_56_samples = new QS._qss_c_.Statistics_.Samples1D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples1D _sum_12_samples = new QS._qss_c_.Statistics_.Samples1D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples1D _sum_34_samples = new QS._qss_c_.Statistics_.Samples1D();
#endif
#if PROFILE_4
        [QS.Fx.Base.Inspectable]
        private double _sum_78 = 0;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples1D _sum_78_samples = new QS._qss_c_.Statistics_.Samples1D();
#endif

        [NonSerialized]
        private SymmetricAlgorithm _symmetricalgorithm;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IStringMatch_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IStringMatchClient_,
                QS._qss_x_.Experiment_.Interface_.IStringMatch_>
                    QS._qss_x_.Experiment_.Object_.IStringMatch_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IStringMatch_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IStringMatch_._Work(string[] _words)
        {
            MemoryStream _ms;
            CryptoStream cryptostream;

            if (!_encryption_init)
            {
                _InitEncryption(this._key, this._iv);
            }
            if (this._enc_words == null)
            {
                this._enc_words = _enc_words_ser;
                _enc_words_ser = null;
            }



            int _w_len = _words.Length;
            int _e_len = _enc_words.Length;
#if PROFILE_3
            double _t3 = this._clock.Time;
#endif
            for (int i = 0; i < _w_len; i++)
            {
#if PROFILE_3
                double _t5 = this._clock.Time;
#endif
                string _word = _words[i];

                _ms = new MemoryStream();
                cryptostream = new CryptoStream(_ms, this._encryptor, CryptoStreamMode.Write);
                byte[] _b = ASCIIEncoding.ASCII.GetBytes(_word);

                cryptostream.Write(_b, 0, _b.Length);
                cryptostream.FlushFinalBlock();
                cryptostream.Close();

                string _enc = ASCIIEncoding.ASCII.GetString(_ms.ToArray());
                _ms.Close();


#if PROFILE_3
                double _t6 = this._clock.Time;
                _sum_56 += _t6 - _t5;

                double _t1 = this._clock.Time;
#endif
                for (int j = 0; j < _e_len; j++)
                {
                    string _enc_word = _enc_words[j];
                    if (_enc_word.Equals(_enc))
                    {
#if PROFILE_4
                        double _t7 = this._clock.Time;
#endif

                        _result[_word] = true;

#if PROFILE_4
                        double _t8 = this._clock.Time;
                        _sum_78 += _t8 - _t7;
#endif
                        break;
                    }
                }
#if PROFILE_3
                double _t2 = this._clock.Time;
                _sum_12 += _t2 - _t1;
#endif


            }
#if PROFILE_3
            double _t4 = this._clock.Time;
            _sum_34 += _t4 - _t3;
#endif
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IStringMatch_._Done()
        {
#if USE_LIST
            this._workendpoint.Interface._Done(_result_dict);
#else
            this._workendpoint.Interface._Done(_result);
#endif
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IStringMatch_._Set_Enc_Words(string[] _enc_words)
        {


            this._enc_words = _enc_words;
            this._enc_words_ser = _enc_words;
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IStringMatch_._Set_Key_IV(byte[] _key, byte[] _iv)
        {
            this._key = _key;
            this._iv = _iv;

           // _InitEncryption(_key, _iv);
        }

        #endregion

        private void _InitEncryption(byte[] _key, byte[] _iv)
        {

            _encryption_init = true;
            this._symmetricalgorithm = SymmetricAlgorithm.Create();
            
                _symmetricalgorithm.Key = _key;
                _symmetricalgorithm.IV = _iv;
                _encryptor = _symmetricalgorithm.CreateEncryptor(_key,_iv);
                _decryptor = _symmetricalgorithm.CreateDecryptor(_key,_iv);
            
        }
        




        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<StringMatch_> Members

        void QS.Fx.Replication.IReplicated<StringMatch_>.Export(StringMatch_ _other)
        {
#if PROFILE_2
            double _t1 = 0;
            if (this._master)
            {
                _t1 = this._clock.Time;
            }
#endif
            _other._key = _key;
            _other._iv = _iv;
            _other._encryption_init = false;

            _other._enc_words_ser = this._enc_words_ser;
            _other._clock = this._clock;
            _other._result.Clear();
#if PROFILE_2
            if (this._master)
            {
                double _t2 = this._clock.Time;
                this._export_times.Add(_t1, _t2 - _t1);
            }
#endif
        }

        void QS.Fx.Replication.IReplicated<StringMatch_>.Import(StringMatch_ _other)
        {
#if PROFILE_2
            double _t1 = 0;
            if (this._master)
            {
                _t1 = this._clock.Time;
            }
#endif
#if USE_LIST
            foreach(string _element in _other._result)^
#else
            foreach (KeyValuePair<string, bool> _element in _other._result)
#endif
            {
                bool _found;

#if USE_LIST
                if(!_result_dict.TryGetValue(_element,out _found))
#else

                if (!_result.TryGetValue(_element.Key, out _found))
#endif
                {
                    _found = true;
#if USE_LIST
                    _result_dict[_element] = _found;
#else


                    _result[_element.Key] = _found;
#endif
                }
            }
            _other._result.Clear();
#if PROFILE_2
            if (this._master)
            {
                double _t2 = this._clock.Time;
                this._import_times.Add(_t1, _t2 - _t1);
            }
#endif

#if PROFILE_3
            if(this._master) {
                _sum_12_samples.Add(_other._sum_12);
                _sum_34_samples.Add(_other._sum_34);
                _sum_56_samples.Add(_other._sum_56);
            }
#endif

#if PROFILE_4
            if(this._master) {
                _sum_78_samples.Add(_other._sum_78);
            }
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _count = this._result.Count;
                int _headersize = sizeof(int) * (_count + 1);

                int _totalsize = _headersize;
                foreach (KeyValuePair<string, bool> _element in this._result)
                {
                    _totalsize += _element.Key.Length;
                }

                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Experiment_Component_StringMatch, _headersize, _totalsize, _count);
            }
        }




        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _count = (this._result != null) ? this._result.Count : 0;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);

                if (_count > 0)
                {
                    foreach (KeyValuePair<string, bool> _element in this._result)
                    {
                        *((int*)_pbuffer) = _element.Key.Length;
                        _pbuffer += sizeof(int);

                        _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_element.Key)));

                    }
                }
            }
            _header.consume(sizeof(int) * (_count + 1));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _count;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                _count = *((int*)_pbuffer);
                _pbuffer += sizeof(int);
                this._result = new Dictionary<string, bool>(_count);
                for (int i = 0; i < _count; i++)
                {
                    int _key_length = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    try
                    {
                        string _key = Encoding.ASCII.GetString(_data.Array, _data.Offset, _key_length);
                        _data.consume(_key_length);
                        this._result.Add(_key, true);
                    }
                    catch (Exception e)
                    {
                        int a;
                    }



                }
            }
            _header.consume(sizeof(int) * (_count + 1));
        }

        #endregion
    }
}
