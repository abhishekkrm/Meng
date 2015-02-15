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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("4919F986891046B8B088C953A9FABD78")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class StringMatchClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IStringMatchClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public StringMatchClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("words file", QS.Fx.Reflection.ParameterClass.Value)] 
            string _words_path,
            [QS.Fx.Reflection.Parameter("ratio of enc words to unenc words", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ratio,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)] 
            int _batch_size,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IStringMatch_> _workreference)
        {
            this._mycontext = _mycontext;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IStringMatch_,
                    QS._qss_x_.Experiment_.Interface_.IStringMatchClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);




            //this._algo = SymmetricAlgorithm.Create();
            //this._iv = _algo.IV;
            //this._key = _algo.Key;
            //this._decryptor = _algo.CreateDecryptor(_key, _iv);
            //this._encryptor = _algo.CreateEncryptor(_key, _iv);

           


            //_monitor.WaitOne();

            string[][] _batches;
            string[] _enc_words;
            using (TextReader _tr = new StreamReader(new FileStream(_words_path, FileMode.Open)))
            {
                this._words = new List<string>();
                string _line = _tr.ReadLine();
                while (_line != null)
                {
                    this._words.Add(_line);
                    _line = _tr.ReadLine();
                }
            }


            string[] _t = _words.ToArray();

            int _num_batches = (int)Math.Ceiling((double)_words.Count / _batch_size);

            _batches = new string[_num_batches][];

            for (int i = 0; i < _num_batches; i++)
            {
                int _size = ((_batch_size * (i + 1)) < _words.Count) ? _batch_size : (_words.Count - (_batch_size * i));
                _batches[i] = new string[_size];
                for (int j = 0; j < _size; j++)
                {
                    _batches[i][j] = _words[(_batch_size * i) + j];
                }
            }
            //if (File.Exists(_words_path + ".enc"))
            //{
            //    //List<string> _ew = new List<string>();
            //    using (FileStream _fs = new FileStream(_words_path + ".enc", FileMode.Open))
            //    {
            //        BinaryFormatter bFormatter = new BinaryFormatter();
            //        _enc_words = (string[])bFormatter.Deserialize(_fs);
            //    }
            //    using (FileStream _fs = new FileStream(_words_path + ".iv", FileMode.Open))
            //    {
            //        BinaryFormatter bFormatter = new BinaryFormatter();
            //        _iv = (byte[])bFormatter.Deserialize(_fs);
            //    }
            //    using (FileStream _fs = new FileStream(_words_path + ".key", FileMode.Open))
            //    {
            //        BinaryFormatter bFormatter = new BinaryFormatter();
            //        _key = (byte[])bFormatter.Deserialize(_fs);
            //    }
            //    //using (FileStream _fs = new FileStream(_words_path + ".batches", FileMode.Open))
            //    //{
            //    //    BinaryFormatter bFormatter = new BinaryFormatter();
            //    //    _batches = (string[][])bFormatter.Deserialize(_fs);
            //    //}
            //    //this._decryptor = _algo.CreateDecryptor(_key, _iv);
            //    //this._encryptor = _algo.CreateEncryptor(_key, _iv);
            //}
            //else
            //{

                


                int _num_enc_words = (int)(_ratio * _words.Count);
                this._mycontext.Platform.Logger.Log("Num Enc Words: " + _num_enc_words);
                this._algo = SymmetricAlgorithm.Create();
                this._iv = _algo.IV;
                this._key = _algo.Key;
                this._decryptor = _algo.CreateDecryptor(_key, _iv);
                this._encryptor = _algo.CreateEncryptor(_key, _iv);
                _enc_words = new string[_num_enc_words];
                MemoryStream _ms;
                CryptoStream _cs;
                Random _random = new Random();
                List<int> _indicies = new List<int>();
                for (int i = 0; i < _enc_words.Length; i++)
                {
                    _ms = new MemoryStream();
                    _cs = new CryptoStream(_ms, this._encryptor, CryptoStreamMode.Write);
                    int _r = _random.Next(_words.Count);
                    while (_indicies.Contains(_r))
                        _r = _random.Next(_words.Count);
                    _indicies.Add(_r);
                    byte[] _b = ASCIIEncoding.ASCII.GetBytes(_words[_r]);
                    _cs.Write(_b, 0, _b.Length);
                    _cs.FlushFinalBlock();
                    _cs.Close();

                    string _enc = ASCIIEncoding.ASCII.GetString(_ms.ToArray());
                    _ms.Close();

                    _enc_words[i] = _enc;
                }

                //using (FileStream _fs = new FileStream(_words_path + ".enc", FileMode.CreateNew))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();
                //    bFormatter.Serialize(_fs, _enc_words);
                //}
                //using (FileStream _fs = new FileStream(_words_path + ".key", FileMode.CreateNew))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();
                //    bFormatter.Serialize(_fs, this._key);
                //}
                //using (FileStream _fs = new FileStream(_words_path + ".iv", FileMode.CreateNew))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();
                //    bFormatter.Serialize(_fs, this._iv);
                //}
                //using (FileStream _fs = new FileStream(_words_path + ".batches", FileMode.CreateNew))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();
                //    bFormatter.Serialize(_fs, _batches);
                //}

            //}

            this._workendpoint.Interface._Set_Key_IV(_key, _iv);
            this._workendpoint.Interface._Set_Enc_Words(_enc_words);
            GC.Collect();

            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);


            

            this._clock = this._mycontext.Platform.Clock;
            _cpuutil.Start();
            this._begin = this._clock.Time;

            

            for (int _i = 0; _i < _batches.Length; _i++)
            {
                this._workendpoint.Interface._Work(_batches[_i]);
            }

            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        [QS.Fx.Base.Inspectable]
        private byte[] _key;
        [QS.Fx.Base.Inspectable]
        private byte[] _iv;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _monitor = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IStringMatch_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IStringMatch_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IStringMatch_,
                QS._qss_x_.Experiment_.Interface_.IStringMatchClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;

        [QS.Fx.Base.Inspectable]
        private double _begin, _end;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private SymmetricAlgorithm _algo;
        [QS.Fx.Base.Inspectable]
        private List<string> _words;
        [QS.Fx.Base.Inspectable]
        private List<string> _keys;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
        [QS.Fx.Base.Inspectable]
        private ICryptoTransform _encryptor, _decryptor;
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IStringMatchClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IStringMatchClient_._Done(IDictionary<string, bool> _found)
        {
            this._end = this._clock.Time;
            this._cpuutil.Stop();
            this._mycontext.Platform.Logger.Log("Elapsed Time: " + (_end - _begin) + "s");
            this._cpuutil.PrintAvg();
            this._cpuutil.CopyStats(_end - _begin);
            
            StringBuilder _found_words = new StringBuilder();
            foreach (KeyValuePair<string, bool> _element in _found)
            {
                if (_element.Value)
                {
                    _found_words.Append(_element.Key + ", ");
                }
            }
            _found_words.Remove(_found_words.Length - 2, 2);
            
            this._mycontext.Platform.Logger.Log("The encrypted file contains the following words: " + _found_words.ToString());
        }

        void QS._qss_x_.Experiment_.Interface_.IStringMatchClient_._Set_KeyIV(byte[] _key, byte[] _iv)
        {
            this._algo = SymmetricAlgorithm.Create();
            this._iv = _iv;
            this._key = _key;
            this._decryptor = _algo.CreateDecryptor(_key, _iv);
            this._encryptor = _algo.CreateEncryptor(_key, _iv);
            this._monitor.Set();
            
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
