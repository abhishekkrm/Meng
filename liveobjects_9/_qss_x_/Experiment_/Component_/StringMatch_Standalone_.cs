/*
Copyright (c) 2008-2009 Chuck Sakoda. All rights reserved.
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
//#define PROFILE
//#define PROFILE_2
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;


namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("0D6033B035124b93BA959E811D4F2A67", "StringMatch_Standalone_")]
    class StringMatch_Standalone_ : QS.Fx.Object.Classes.IObject
    {

        #region Constructor


        

        public StringMatch_Standalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("words file", QS.Fx.Reflection.ParameterClass.Value)] 
            string _words_path,
            [QS.Fx.Reflection.Parameter("ratio of enc words to unenc words", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ratio,
            [QS.Fx.Reflection.Parameter("# threads", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_threads)
        {


            #region Init Fields

            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._num_threads = _num_threads;
            this._clock = this._mycontext.Platform.Clock;
            this._result = new Dictionary<string, bool>();
            //SymmetricAlgorithm _symmetricalgorithm = SymmetricAlgorithm.Create();
            //this._encryptor = _symmetricalgorithm.CreateEncryptor();
            //this._key = _symmetricalgorithm.Key;
            //this._iv = _symmetricalgorithm.IV;

            #endregion

            #region Init Threads

            Thread[] _threads = new Thread[_num_threads];

            for (int _i = 0; _i < _num_threads; _i++)
            {
                _threads[_i] = new Thread(new ParameterizedThreadStart(this._Work));
            }

            #endregion

            // Batch Words and Generate Encrypted Words appear exactly as they do for the Replicated solution

            
            string[][] _batches;

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

            int _batch_size = (int)Math.Ceiling((double)_words.Count / _num_threads);

            _batches = new string[_num_threads][];

            for (int i = 0; i < _num_threads; i++)
            {
                int _size = ((_batch_size * (i + 1)) < _words.Count) ? _batch_size : (_words.Count - (_batch_size * i));
                _batches[i] = new string[_size];
                for (int j = 0; j < _size; j++)
                {
                    _batches[i][j] = _words[(_batch_size * i) + j];
                }
            }

            //string[] _enc_words;

            if (File.Exists(_words_path + ".enc"))
            {
                //List<string> _ew = new List<string>();
                using (FileStream _fs = new FileStream(_words_path + ".enc", FileMode.Open))
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    _enc_words = (string[])bFormatter.Deserialize(_fs);
                }
                using (FileStream _fs = new FileStream(_words_path + ".iv", FileMode.Open))
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    _iv = (byte[])bFormatter.Deserialize(_fs);
                }
                using (FileStream _fs = new FileStream(_words_path + ".key", FileMode.Open))
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    _key = (byte[])bFormatter.Deserialize(_fs);
                }
                //using (FileStream _fs = new FileStream(_words_path + ".batches", FileMode.Open))
                //{
                //    BinaryFormatter bFormatter = new BinaryFormatter();
                //    _batches = (string[][])bFormatter.Deserialize(_fs);
                //}
                //this._decryptor = _algo.CreateDecryptor(_key, _iv);
                //this._encryptor = _algo.CreateEncryptor(_key, _iv);
            }
            else
            {

                
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

            }

            #region Start Threads

            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i].Start(new object[] { _batches[i] });
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private ICryptoTransform _encryptor, _decryptor;
        [QS.Fx.Base.Inspectable]
        private SymmetricAlgorithm _algo;
        [QS.Fx.Base.Inspectable]
        private List<string> _words = new List<string>();
        [QS.Fx.Base.Inspectable]
        private byte[] _key;
        [QS.Fx.Base.Inspectable]
        private byte[] _iv;
        [QS.Fx.Base.Inspectable]
        string[] _enc_words;
        [QS.Fx.Base.Inspectable]
        private int _init = 0;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _started = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private double _begin, _end;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
        
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, bool> _result;
        [QS.Fx.Base.Inspectable]
        private int _done = 0;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Thread Work

        void _Work(object _o)
        {
            object[] o = (object[])_o;
            string[] _words = (string[])o[0]; // The list of words this thread will encrypt and compare
            byte[] _key = this._key;
            byte[] _iv = this._iv;
            int _w_len = _words.Length;
            int _e_len = _enc_words.Length;

            //string[] _thread_enc_words = this._enc_words;
            //Array.Copy(_enc_words, _thread_enc_words, _enc_words.Length);

            #region Init each thread's encryptor

            MemoryStream _ms;
            CryptoStream cryptostream;

            ICryptoTransform _thread_encryptor;

            using (SymmetricAlgorithm _s = SymmetricAlgorithm.Create())
            {
                _thread_encryptor = _s.CreateEncryptor(_key, _iv);
            }

            #endregion

            #region Wait for all threads to start before working

            if (Interlocked.Increment(ref this._init) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                _cpuutil.Start();
                this._begin = this._clock.Time;
                _started.Set();
            }
            else
            {
                _started.WaitOne();
            }

            #endregion

            #region Init Profiling

#if PROFILE
            double _sum_56 = 0;
#endif
#if PROFILE || PROFILE_2
            double _sum_12 = 0;
#endif
#if PROFILE_2
            double _sum_78 = 0;
#endif
            double _t3 = this._clock.Time;

            #endregion

            #region Main Work Loop

            for (int i = 0; i < _w_len; i++)
            {
                #region Profiling

#if PROFILE
                double _t5 = this._clock.Time;
#endif

                #endregion

                string _word = _words[i];

                #region Encrypt _word

                _ms = new MemoryStream();
                cryptostream = new CryptoStream(_ms, _thread_encryptor, CryptoStreamMode.Write);
                byte[] _b = ASCIIEncoding.ASCII.GetBytes(_word);

                cryptostream.Write(_b, 0, _b.Length);
                cryptostream.FlushFinalBlock();
                cryptostream.Close();

                string _enc = ASCIIEncoding.ASCII.GetString(_ms.ToArray());
                _ms.Close();

                #endregion

                #region Profiling

#if PROFILE
                double _t6 = this._clock.Time;
                _sum_56 += _t6 - _t5;
#endif
#if PROFILE || PROFILE_2
                double _t1 = this._clock.Time;
#endif
                #endregion

                #region Compare Encrypted String with Master List

                for (int j = 0; j < _e_len; j++)
                {

                    string _enc_word = _enc_words[j];

                    if (_enc_word.Equals(_enc))
                    {
#if PROFILE_2
                        double _t7 = this._clock.Time;
#endif
                        lock (this._result)
                        {
                            _result[_word] = true;
                        }
#if PROFILE_2
                        double _t8 = this._clock.Time;
                        _sum_78 += _t8 - _t7;
#endif
                        break;
                    }
                }

                #endregion

                #region Profiling

#if PROFILE || PROFILE_2
                double _t2 = this._clock.Time;
                _sum_12 += _t2 - _t1;
#endif

                #endregion
            }

            #endregion

            #region Profiling

            double _t4 = this._clock.Time;

            #endregion

            #region Report Per-Thread Profiling

            this._mycontext.Platform.Logger.Log("Main time: " + (_t4 - _t3).ToString());
#if PROFILE
            this._mycontext.Platform.Logger.Log("5-6: " + _sum_56 + "        ||| 1-2: " + _sum_12);
#endif
#if PROFILE || PROFILE_2
            this._mycontext.Platform.Logger.Log("1-2: " + _sum_12);
#endif
#if PROFILE_2
            this._mycontext.Platform.Logger.Log("7-8: " + _sum_78);
            this._mycontext.Platform.Logger.Log("Time Comparing: " + (_sum_12 - _sum_78));
#endif
            //this._mycontext.Platform.Logger.Log("Avg time acquiring lock: " + _sum + " / "+_sum_c+" = "+(_sum / _sum_c).ToString());

            #endregion

            #region Report Whole Experiment Profiling (last thread to complete)

            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {
                this._end = this._clock.Time;
                this._cpuutil.Stop();
                this._mycontext.Platform.Logger.Log("Elapsed Time: " + (_end - _begin) + "s");
                this._cpuutil.PrintAvg();
                this._cpuutil.CopyStats(_end - _begin);

                StringBuilder _found_words = new StringBuilder();
                foreach (KeyValuePair<string, bool> _element in _result)
                {
                    if (_element.Value)
                    {
                        _found_words.Append(_element.Key + ", ");
                    }
                }
                _found_words.Remove(_found_words.Length - 2, 2);

                this._mycontext.Platform.Logger.Log("The encrypted file contains the following words: " + _found_words.ToString());
            }

            #endregion
        }

        #endregion

    }
}

