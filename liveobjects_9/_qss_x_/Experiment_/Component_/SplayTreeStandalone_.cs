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

//#define CPUUTIL

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("7A363F204CA64795BE449A27002AB286", "StplayTreeStandalone_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class SplayTreeStandalone_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public SplayTreeStandalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("get burst size", QS.Fx.Reflection.ParameterClass.Value)]
            int _get_burst_size,
            [QS.Fx.Reflection.Parameter("ratio add to get", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ratio_add_to_get,
            [QS.Fx.Reflection.Parameter("# of threads", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_threads)
        {
            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._clock = _mycontext.Platform.Clock;
            this._count = _count;
            this._get_burst_size = _get_burst_size;
            this._ratio_add_to_get = _ratio_add_to_get;
            this._dict = new QS._qss_c_.Collections_2_.SplayOf<string,StringNode>();

            this._num_threads = _num_threads;
            _threads = new Thread[_num_threads];
            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i] = new Thread(new ParameterizedThreadStart(this._ThreadWork));
            }

            
            int size = this._count / this._num_threads;

            for (int i = 0; i < this._num_threads; i++)
            {
                //_workers[i] = new Worker_(i * _individual_dict_size, _num_cycles, _cd, _num_adds_per_cycle, _num_gets_per_cycle, new ThreadStart(_Done));
                _threads[i].Start(new object[] { i * size, size });
            }
        }

        #endregion

        private void _ThreadWork(object _params)
        {
            object[] _o = (object[])_params;
            int _base = (int)_o[0];
            int _size = (int)_o[1];

            int _curr = _base;

            if (Interlocked.Increment(ref this._ready) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if CPUUTIL

            _cpuutil.Start();
#endif

                this._begin = this._clock.Time;
                _start.Set();
            }
            else
            {
                _start.WaitOne();
            }



            while (_curr < _base + _size)
            {
                for (int i = 0; i < _ratio_add_to_get * _get_burst_size; i++)
                {
                    lock (this._dict)
                    {
                        this._dict.Add(new StringNode(_curr.ToString(), string.Empty));
                    }
                    _curr++;
                    if (_curr >= _base + _size)
                        break;
                }
                for (int i = 0; i < _get_burst_size; i++)
                {
                    lock (this._dict)
                    {
                        StringNode a = this._dict[(_curr - 1).ToString()];
                    }

                }

                
            }








            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {
                this._end = this._clock.Time;
                this._mycontext.Platform.Logger.Log("Counted: "+this._dict.Count);
                this._mycontext.Platform.Logger.Log("Duration: " + (this._end - this._begin).ToString());

            }

        }

        
        private class StringNode : QS._qss_c_.Collections_2_.BTNOf<string>
        {
            public StringNode(string key, string value)
                : base(key)
            {
                this._key = key;
                this._val = value;
            }

            public StringNode() { }

            private string _val;
            private string _key;
            public string Value
            {
                get
                {
                    return this._val;
                }
                set
                {
                    this._val = value;
                }
            }

            public string Key
            {
                get
                {
                    return this._key;
                }
            }
        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        #region Fields
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Collections_2_.SplayOf<string, StringNode> _dict;
        [QS.Fx.Base.Inspectable]
        private double _ratio_add_to_get;
        [QS.Fx.Base.Inspectable]
        private int _cycle = 0;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _cycle_wait = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private int _get_burst_size;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _start = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _done = 0;
        [QS.Fx.Base.Inspectable]
        private int _ready = 0;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _monitor = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double _begin;
        [QS.Fx.Base.Inspectable]
        private double _end;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;

        [QS.Fx.Base.Inspectable]
        private IList<double> _elapsed = new List<double>();
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        

        
    }
}
