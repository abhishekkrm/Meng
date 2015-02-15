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
    [QS.Fx.Reflection.ComponentClass("58E9DD08A8B04ad681308ECA920B6542", "ReplicatedDictionaryClient_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class ReplicatedDictionaryClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IDictionaryClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ReplicatedDictionaryClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("number of cycles", QS.Fx.Reflection.ParameterClass.Value)] 
            int _cycles,
            [QS.Fx.Reflection.Parameter("ratio", QS.Fx.Reflection.ParameterClass.Value)]
            double _ratio,
            [QS.Fx.Reflection.Parameter("max string length", QS.Fx.Reflection.ParameterClass.Value)]
            int _str_len,
            [QS.Fx.Reflection.Parameter("Get", QS.Fx.Reflection.ParameterClass.Value)]
            bool _get,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch_size,
            [QS.Fx.Reflection.Parameter("use files",QS.Fx.Reflection.ParameterClass.Value)]
            bool _files,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference)
        {
            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._workreference = _workreference;
            this._clock = this._mycontext.Platform.Clock;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionary_,
                    QS._qss_x_.Experiment_.Interface_.IDictionaryClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._count = _count;
            Random _r = new Random();
            this._cycles_count = _cycles;
            this._cycles = _cycles;
            _kvps = new List<string[]>(_count);

            //for (int i = 0; i < _count; i++)
            //{
            //    //int _r1 = _r.Next(1, _str_len+1);
            //    //int _r2 = _r.Next(1, _str_len+1);
            //    //byte[] _key_bytes = new byte[_r1];
            //    //byte[] _val_bytes = new byte[_r2];
            //    //_r.NextBytes(_key_bytes);
            //    //_r.NextBytes(_val_bytes);
            //    //_kvps.Add(new string[] { ASCIIEncoding.ASCII.GetString(_key_bytes), ASCIIEncoding.ASCII.GetString(_val_bytes) });
            //    _kvps.Add(new string[] { i.ToString(), i.ToString() });
            //}
            double _t1 = this._mycontext.Platform.Clock.Time;
            string _file = @"C:\Users\Public\repdicinput";
            IList<string> _batch_paths = new List<string>();
            if (_files)
            {
                //_dir = @"C:\Users\Public\repdict_tmp";
                //if (!Directory.Exists(_dir))
                //{
                //    Directory.CreateDirectory(_dir);
                //}
                //else
                //{
                //    if (Directory.GetFiles(_dir).Length != (_count / _batch_size))
                //    {
                //        foreach (string _file in Directory.GetFiles(_dir))
                //        {
                //            File.Delete(_file);
                //        }
                //    }
                //    else
                //    {
                //        foreach (string _file in Directory.GetFiles(_dir))
                //        {
                //            _batch_paths.Add(_file);
                //        }
                //        goto NoNewFiles;
                //    }
                //}
                //for (int i = 0; i < _count;)
                //{
                //    string _path = Path.Combine(_dir, i.ToString());
                //    _batch_paths.Add(_path);
                //    using (TextWriter _tw = new StreamWriter(new FileStream(_path, FileMode.CreateNew)))
                //    {
                //        for (int j = 0; j < _batch_size; j++)
                //        {
                //            _tw.WriteLine(i + ":" + i++);
                //        }
                //    }
                //}
                
                if (!File.Exists(_file))
                {
                    using(TextWriter _tw = new StreamWriter(new FileStream(_file, FileMode.CreateNew))) {
                        for(int i =0;i<_count;i++) {
                            _tw.WriteLine(i+":"+i);
                        }
                    }
                }
                
            }
            
            

            this._workendpoint.Interface._Set_Ratio(_ratio);
            this._batch_size = _batch_size;

            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
#if CPUUTIL

            _cpuutil.Start();
#endif
            
            this._begin = this._clock.Time;



            this._mycontext.Platform.Logger.Log("Submitting cycle 0 of " + _count.ToString() + " elements");

            if (_batch_size > 1 || (_batch_size == 1&& _files))
            {
                if (_files)
                {
                    for(int i =0;i<_count;) {
                        this._workendpoint.Interface._AddFromFile(_file,i, _batch_size);
                        i+=_batch_size;
                    }
                }
                else
                {
                    for (int i = 0; i < _count; )
                    {
                        IList<string> _l = new List<string>();
                        for (int j = 0; j < _batch_size; j++)
                        {
                            _l.Add(i++.ToString());
                        }
                        this._workendpoint.Interface._AddMultiple(_l.ToArray(), _l.ToArray());
                    }
                }
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {


                    this._workendpoint.Interface._Add(i.ToString(), i.ToString());

                }

            }
            if (!_get)
            {
                this._workendpoint.Interface._Clear();
            }
            else
            {
                this._workendpoint.Interface._Get("0");
            }
            double _t2 = this._clock.Time - this._begin;

        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        [QS.Fx.Base.Inspectable]
        private int _batch_size;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _monitor = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IDictionary_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IDictionary_,
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private int _cycles;
        [QS.Fx.Base.Inspectable]
        private int _cycles_count;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double _begin;
        [QS.Fx.Base.Inspectable]
        private IList<string[]> _kvps;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;

        [QS.Fx.Base.Inspectable]
        private IList<double> _elapsed = new List<double>();
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicatedDictionaryClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Got(string key, string _vals)
        {
            this._elapsed.Add(this._mycontext.Platform.Clock.Time -
                this._begin);
            //this._mycontext.Platform.Logger.Log("Completed cycle " + _completed_cycles++ + ".  Duration: " + this._elapsed[_elapsed.Count - 1]);
            _completed_cycles++;


            if (_completed_cycles == _cycles_count)
            {
#if CPUUTIL
                this._cpuutil.Stop();
                this._cpuutil.PrintAvg();
#endif
                double _sum = 0;
                foreach (double _d in _elapsed)
                {
                    _sum += _d;
                }
                _mycontext.Platform.Logger.Log("Total duration: " + _sum.ToString());
                _sum -= _elapsed[0];
                _sum /= (_elapsed.Count - 1);
                _mycontext.Platform.Logger.Log("Average duration: " + _sum.ToString());

            }
            else
            {
                this._begin = this._mycontext.Platform.Clock.Time;
                //this._mycontext.Platform.Logger.Log("Submitting cycle " + _completed_cycles.ToString() + " of " + _count.ToString() + " elements");
                if (_batch_size > 1)
                {
                    for (int i = 0; i < _count; )
                    {
                        IList<string> _l = new List<string>();
                        for (int j = 0; j < _batch_size; j++)
                        {
                            _l.Add(i++.ToString());
                        }
                        this._workendpoint.Interface._AddMultiple(_l.ToArray(), _l.ToArray());
                    }
                }
                else
                {
                    for (int i = _completed_cycles * _count; i < (_completed_cycles+1)*_count; )
                    {
                        for (int j = 0; j < _batch_size; j++)
                        {
                            this._workendpoint.Interface._Add(i.ToString(), i++.ToString());
                        }
                    }
                }

                this._workendpoint.Interface._Get("1");

            }

            //this._mycontext.Platform.Logger.Log("Got - key: " + key + ", value: " + _vals);


        }



        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionaryClient_ Members

        private int _completed_cycles = 0;

        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Cleared()
        {
            this._elapsed.Add(this._mycontext.Platform.Clock.Time -
                this._begin);
            this._mycontext.Platform.Logger.Log("Completed cycle " + _completed_cycles++ + ".  Duration: " + this._elapsed[_elapsed.Count - 1]);
            //_completed_cycles++;


            if (_completed_cycles == _cycles_count)
            {
#if CPUUTIL
                this._cpuutil.Stop();
                this._cpuutil.PrintAvg();
#endif
                double _sum = 0;
                foreach (double _d in _elapsed)
                {
                    _sum += _d;
                }
                _mycontext.Platform.Logger.Log("Total duration: " + _sum.ToString());
                _sum -= _elapsed[0];
                _sum /= (_elapsed.Count-1);
                _mycontext.Platform.Logger.Log("Average duration: " + _sum.ToString());
               
            }
            else
            {
                this._begin = this._mycontext.Platform.Clock.Time;
                //this._mycontext.Platform.Logger.Log("Submitting cycle " + _completed_cycles.ToString() + " of " + _count.ToString() + " elements");
                if (_batch_size > 1)
                {
                    for (int i = 0; i < _count; )
                    {
                        IList<string> _l = new List<string>();
                        for (int j = 0; j < _batch_size; j++)
                        {
                            _l.Add(i++.ToString());
                        }
                        this._workendpoint.Interface._AddMultiple(_l.ToArray(), _l.ToArray());
                    }
                }
                else
                {
                    for (int i = _completed_cycles*_count; i < _count*(_completed_cycles+1); )
                    {
                        for (int j = 0; j < _batch_size; j++)
                        {
                            this._workendpoint.Interface._Add(i.ToString(), i++.ToString());
                        }
                    }
                }

                this._workendpoint.Interface._Clear();

            }
        }

        #endregion

        #region IDictionaryClient_ Members


        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Counted(int _count)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
