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
    [QS.Fx.Reflection.ComponentClass("960BE4F4B1D14e6bBD46A511FA9EFEFB", "ReverseIndex_Standalone_")]
    class ReverseIndex_Standalone_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {

        #region Constructor




        public ReverseIndex_Standalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
            string _folder_path,
            [QS.Fx.Reflection.Parameter("number of files to analyze (~300MB / 10,000 files)", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_files,
            [QS.Fx.Reflection.Parameter("repeat", QS.Fx.Reflection.ParameterClass.Value)]
            int _repeat,
            [QS.Fx.Reflection.Parameter("ratio of links to return", QS.Fx.Reflection.ParameterClass.Value)]
            double _num_links,
            [QS.Fx.Reflection.Parameter("# threads", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_threads)
        {


            #region Init Fields

            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._num_threads = _num_threads;
            this._clock = this._mycontext.Platform.Clock;
            
            
            #endregion

            #region Init Threads

            Thread[] _threads = new Thread[_num_threads];

            for (int _i = 0; _i < _num_threads; _i++)
            {
                _threads[_i] = new Thread(new ParameterizedThreadStart(this._Work));
            }

            #endregion

            string[] _html_files = Directory.GetFiles(_folder_path);
            this._mycontext.Platform.Logger.Log("RPT: " + _repeat);
            List<string[]> _batches = new List<string[]>();
            List<int> _starts = new List<int>();
            int _batch_size = (int)Math.Ceiling((double)_num_files / _num_threads);
            string[] _batch = new string[_batch_size];
            int _batch_index = 0;
            int _count = 0;

            foreach (string _html_file in _html_files)
            {
                if (_count++ >= _num_files)
                {
                    break;
                }
                if (_batch_index == _batch_size)
                {
                    _batches.Add(_batch);
                    _starts.Add(_count - _batch_size - 1);
                    _batch_index = 0;
                    _batch = new string[_batch_size];
                }
                _batch[_batch_index++] = _html_file;
            }
            if (_batch_index > 0)
            {
                string[] _tmp = new string[_batch_index];
                Array.Copy(_batch, _tmp, _batch_index);
                _batches.Add(_tmp);
                _starts.Add(_count - _tmp.Length - 1);

            }


            if (_repeat <= 0)
                _repeat = 1;
            this._mycontext.Platform.Logger.Log("Rpeat:" + _repeat);


            #region Start Threads

            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i].Start(new object[] { _batches[i], _repeat });
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IDictionary<string, IList<string>> _links = new Dictionary<string, IList<string>>();
        [QS.Fx.Base.Inspectable]
        private int _init = 0;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _started = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private double _duration, _stoptime,_starttime;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;

        [QS.Fx.Base.Inspectable]
        private int _done = 0;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Thread Work

        void _Work(object _o)
        {
            object[] o = (object[])_o;
            string[] _paths = (string[])o[0]; // The list of words this thread will encrypt and compare
            int _repeat = (int)o[1];

            #region Wait for all threads to start before working

            if (Interlocked.Increment(ref this._init) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                _cpuutil.Start();
                this._starttime = this._clock.Time;
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

            for (int _rp = 0; _rp < _repeat; _rp++)
            {
                for (int i = 0; i < _paths.Length; i++)
                {
                    string _raw_html;
                    using (TextReader _tr = new StreamReader(new FileStream(_paths[i], FileMode.Open)))
                    {

                        _raw_html = _tr.ReadToEnd();
                    }
                    int _current_index = 0;
                    int _a_index, _link_index, _href_index;
                    int _start, _end;
                    _a_index = _link_index = -2;
                    int _len = _raw_html.Length;
                    while (_current_index < _len)
                    {
                        if (_a_index != -1 && _a_index < _current_index)
                        {
                            _a_index = _raw_html.IndexOf("<a ", _current_index);
                        }
                        if (_link_index != -1 && _link_index < _current_index)
                        {
                            _link_index = _raw_html.IndexOf("<link ", _current_index);
                        }


                        if (_a_index < _link_index)
                        {
                            if (_a_index != -1)
                            {
                                _current_index = _a_index;
                            }
                            else
                            {
                                _current_index = _link_index;
                            }
                        }
                        else
                        {
                            if (_link_index != -1)
                            {
                                _current_index = _link_index;
                            }
                            else
                            {
                                _current_index = _a_index;
                            }
                        }

                        if (_current_index > -1)
                        {
                            _href_index = _raw_html.IndexOf("href=", _current_index);
                            _start = _end = 0;
                            char _c = _raw_html[_href_index + 5];
                            switch (_c.ToString())
                            {
                                case "\'":
                                    _start = _href_index + 6;
                                    _end = _raw_html.IndexOf("\'", _start);
                                    break;
                                case "\"":
                                    _start = _href_index + 6;
                                    _end = _raw_html.IndexOf("\"", _start);
                                    break;
                                default:
                                    _start = _href_index + 5;
                                    int a, b;
                                    a = _raw_html.IndexOf(' ', _start);
                                    b = _raw_html.IndexOf('>', _start);
                                    _end = (a < b) ? a : b;
                                    break;
                            }

                            string _link = _raw_html.Substring(_start, _end - _start);
#if !USE_LINK_ID
                            IList<string> _list;
#else
                            IList<int> _list;
#endif
                            lock (this._links)
                            {
                                if (_links.TryGetValue(_link, out _list))
                                {

#if !USE_LINK_ID
                                    _list.Add(_paths[i]);
#else
                                _list.Add(_start_id + i);
#endif
                                }
                                else
                                {
#if !USE_LINK_ID
                                    _list = new List<string>();
                                    _list.Add(_paths[i]);
                                    _links.Add(_link, _list);

#else
                                _list = new List<int>();
                                _list.Add(_start_id + i);
                                _links.Add(_link, _list);
#endif
                                }
                            }


                            _current_index = _end;

                        }
                        else
                        {
                            break;
                        }
                    }//done with this html file, onto the next
                }
            }

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

                this._stoptime = _mycontext.Platform.Clock.Time;
                
                this._duration = this._stoptime - this._starttime;
                this._mycontext.Platform.Logger.Log("Duration : " + this._duration.ToString());



            this._cpuutil.Stop();
            this._cpuutil.PrintAvg();

                StringBuilder _s = new StringBuilder();
                _s.AppendLine("Results:\n");

                KeyValuePair<string, IList<string>>[] _link_lists = new KeyValuePair<string, IList<string>>[_links.Count];
                _links.CopyTo(_link_lists, 0);

                Array.Sort<KeyValuePair<string, IList<string>>>(
                    _link_lists,
                    new Comparison<KeyValuePair<string, IList<string>>>(
                        delegate(KeyValuePair<string, IList<string>> _x, KeyValuePair<string, IList<string>> _y)
                        {
                            return _x.Value.Count - _y.Value.Count;
                        }));

                Array.Reverse(_link_lists);

                int _count = 0;

                foreach (KeyValuePair<string, IList<
#if !USE_LINK_ID
string
#else
                int
#endif
>> _lists in _link_lists)
                {
                    if (_count++ > 15)
                    {
                        break;
                    }
                    _s.Append(_lists.Key);
                    _s.Append(" ");
                    _s.AppendLine(_lists.Value.Count.ToString());
                }
                this._mycontext.Platform.Logger.Log(_s.ToString());


            }

            #endregion
        }

        #endregion

    }
}

