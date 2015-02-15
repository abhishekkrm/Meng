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


//#define PRE_INIT

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("84666FAD41E547208144A1E7B84071F7")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class HistogramClient_ : QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IHistogramClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public HistogramClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch_size,
            [QS.Fx.Reflection.Parameter("# of frames", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_of_frames,
            [QS.Fx.Reflection.Parameter("repeat",QS.Fx.Reflection.ParameterClass.Value)]
            int _repeat,
            [QS.Fx.Reflection.Parameter("cache",QS.Fx.Reflection.ParameterClass.Value)]
            bool _cache,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IHistogram_> _workreference)
        {
            this._mycontext = _mycontext;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IHistogram_,
                    QS._qss_x_.Experiment_.Interface_.IHistogramClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            Random _random = new Random();
#if PRE_INIT
            byte[] _bitmap = new byte[(int)(1024 * 1024 * _num_of_frames)];      
            _random.NextBytes(_bitmap);
#endif
            this._clock = _mycontext.Platform.Clock;
            
            string _cache_path = @"C:\Users\Public\histogramcache.dat";
            
            int _num_batches = (int)Math.Ceiling(((double)(1024*1024 *(Int64)_num_of_frames) / _batch_size));
#if PRE_INIT
            this._workendpoint.Interface._Set_Bitmap(_bitmap);
            this._workendpoint.Interface._Flush();
#else
            bool _generate_batches = false;
            if (_cache)
            {
                if (!File.Exists(_cache_path))
                {
                    _generate_batches = true;
                }
            }
            else
            {
                _generate_batches = true;
            }

            byte[][] _batches;
            
            if (_generate_batches)
            {
                 _batches= new byte[_num_batches][];
                for (int i = 0; i < _num_batches; i++)
                {
                    int _len;
                    if (i == _num_batches - 1)
                    {
                        _len = (1024 * 1024 * _num_of_frames) - (i * _batch_size);

                    }
                    else
                    {
                        _len = _batch_size;

                    }
                    _batches[i] = new byte[_len];
                    _random.NextBytes(_batches[i]);

                }
                if (_cache)
                {
                    // save batches
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    MemoryStream _ms = new MemoryStream();
                    _bf.Serialize(_ms, _batches);
                    using (FileStream _fs = new FileStream(_cache_path, FileMode.Create))
                    {
                        _fs.Write(_ms.ToArray(), 0, (int)_ms.Length);
                    }
                    
                }
            }
            else
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using(FileStream _fs = new FileStream(_cache_path,FileMode.Open)) {
                    _batches = (byte[][])_bf.Deserialize(_fs);
                }
            }
#endif
            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            _cpuutil.Start();
            _begin = this._clock.Time;

#if PRE_INIT
            
            
            for (int _i = 0; _i < _num_batches; _i++)
            {
                int _c;
                if (_i == _num_batches - 1)
                {
                    _c = _bitmap.Length - (_i * 1000) ;
                }
                else
                {
                    _c = 1000;
                }
                this._workendpoint.Interface._Work(1000 * _i, _c);
            }
#else
            if (_repeat <= 0)
                _repeat = 1;
            for (int j = 0; j < _repeat; j++)
            {
                for (int i = 0; i < _num_batches; i++)
                {
                    this._workendpoint.Interface._Work2(_batches[i]);
                }
            }
#endif
            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IHistogram_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IHistogram_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IHistogram_,
                QS._qss_x_.Experiment_.Interface_.IHistogramClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private double _begin;
        [QS.Fx.Base.Inspectable]
        private double _end;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IHistogramClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IHistogramClient_._Done(int[] _histogram)
        {
            this._end = this._clock.Time;
            _cpuutil.Stop();
            _cpuutil.PrintAvg();
            _cpuutil.CopyStats(this._end - this._begin);
            this._mycontext.Platform.Logger.Log("Duration: " + (this._end - this._begin));

            StringBuilder _s = new StringBuilder();
            for (int _i = 0; _i < _histogram.Length; _i++)
            {
                _s.Append(_histogram[_i].ToString());
                _s.Append(" ");
            }

            MessageBox.Show(_s.ToString());
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
