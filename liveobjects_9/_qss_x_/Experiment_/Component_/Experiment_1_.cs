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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Experiment_1)]
    public sealed class Experiment_1_ : QS._qss_x_.Properties_.Component_.Base_, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Experiment_1_
        (
            QS.Fx.Object.IContext _mycontext,
            //[QS.Fx.Reflection.Parameter("file", QS.Fx.Reflection.ParameterClass.Value)] 
            //string _file,
            [QS.Fx.Reflection.Parameter("root", QS.Fx.Reflection.ParameterClass.Value)] 
            string _root,
            [QS.Fx.Reflection.Parameter("length", QS.Fx.Reflection.ParameterClass.Value)] 
            long _length,
            [QS.Fx.Reflection.Parameter("warmup", QS.Fx.Reflection.ParameterClass.Value)] 
            double _warmup,
            [QS.Fx.Reflection.Parameter("duration", QS.Fx.Reflection.ParameterClass.Value)] 
            double _duration,
            [QS.Fx.Reflection.Parameter("block", QS.Fx.Reflection.ParameterClass.Value)] 
            int _block,
            [QS.Fx.Reflection.Parameter("stages", QS.Fx.Reflection.ParameterClass.Value)] 
            int _stages,
            [QS.Fx.Reflection.Parameter("async", QS.Fx.Reflection.ParameterClass.Value)] 
            string _async,
            [QS.Fx.Reflection.Parameter("options", QS.Fx.Reflection.ParameterClass.Value)] 
            string _options,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
        : base(_mycontext, _debug)
        {
            //_file = Path.GetFullPath(_file);
            //this._file = _file;
            _root = Path.GetFullPath(_root);
            this._root = _root;
            this._length = _length;
            this._warmup = _warmup;
            this._duration = _duration;
            this._block = _block;
            this._stages = _stages;
            this._async = _async;
            this._options = _options;
            //this._id = "Experiment_1_(" + (((double)(new FileInfo(this._file)).Length) / 1048576d).ToString(".00") + 
            //    " MB file, " + (((double) this._block) / 1024d).ToString(".00") + " KB block, " + 
            //    this._length.ToString() + " stages)";
            this._id = "Experiment_1_(" + (((double) this._length) / 1048576d).ToString(".00") +
                " MB file, " + (((double) this._block) / 1048576d).ToString(".00") + " MB block, " +
                this._stages.ToString() + " stages)";

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this._id + "._Constructor");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private int _terminated;
        [QS.Fx.Base.Inspectable]
        private string _options;
        [QS.Fx.Base.Inspectable]
        private string _id;
        //[QS.Fx.Base.Inspectable]
        //private string _file;
        [QS.Fx.Base.Inspectable]
        private string _root;
        [QS.Fx.Base.Inspectable]
        private long _length;
        [QS.Fx.Base.Inspectable]
        private int _block;
        [QS.Fx.Base.Inspectable]
        private int _stages;
        [QS.Fx.Base.Inspectable]
        private string _async;
        [QS.Fx.Base.Inspectable]
        private string _objectfile;
        [QS.Fx.Base.Inspectable]
        private Process _process;
        [QS.Fx.Base.Inspectable]
        private int _nprocessors;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _perfcounter_totalcpu;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter[] _perfcounter_cpu;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_totalcpu = 
            new QS._qss_c_.Statistics_.Samples2D(
                "cpu utilization", "average cpu utilization across all cores", "time", "s", "time in seconds", "utilization", "%", "% cpu utilization");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D[] _statistics_cpu_unsorted;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D[] _statistics_cpu_sorted;
        [QS.Fx.Base.Inspectable]
        private double _totalcpu;
        [QS.Fx.Base.Inspectable]
        private double[] _cpu;
        [QS.Fx.Base.Inspectable]
        private double _warmup;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private double _time0;

        private QS.Fx.Clock.IAlarm _performancealarm;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
            base._Initialize();

#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this._id + "._Initialize");
#endif

            lock (this)
            {
                if (!Directory.Exists(this._root))
                    Directory.CreateDirectory(this._root);

                StringBuilder _s = new StringBuilder();
                _s.AppendLine("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
                _s.AppendLine("<Root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                //_s.AppendLine("<Object xsi:type=\"ReferenceObject\" id=\"0`1:1B79EE13E5604950AE3541037FA9B15A`0\">");
                _s.AppendLine("<Object xsi:type=\"ReferenceObject\" id=\"0`1:E3B07F4F4DFB496E8F7DC10660000B38`0\">");
                _s.AppendLine("<Attribute id=\"9F4C608A9A6D44FFAD8A2FDC662E185B\" value=\"reader\" />");
                _s.AppendLine("<Parameter id=\"id\">");
                _s.AppendLine("<Value xsi:type=\"xsd:string\">reader</Value>");
                _s.AppendLine("</Parameter>");
                //_s.AppendLine("<Parameter id=\"file\">");
                //_s.AppendLine("<Value xsi:type=\"xsd:string\">" + this._file + "</Value>");
                //_s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"length\">");
                _s.AppendLine("<Value xsi:type=\"xsd:long\">" + this._length.ToString() + "</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"block\">");
                _s.AppendLine("<Value xsi:type=\"xsd:int\">" + this._block.ToString() + "</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"next\">");
                for (int _n = 1; _n <= this._stages; _n++)
                {
                    _s.AppendLine("<Value xsi:type=\"ReferenceObject\" id=\"0`1:2EC5D45C0399408096DC61925B3066F3`0\">");
                    _s.AppendLine("<Attribute id=\"9F4C608A9A6D44FFAD8A2FDC662E185B\" value=\"stage_" + _n.ToString("00") + "\" />");
                    _s.AppendLine("<Parameter id=\"id\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:string\">stage_" + _n.ToString("00") + "</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"mode\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:string\">" + (((_n % 2) == 0) ? "Decrypt" : "Encrypt") + "</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"type\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:string\">Rijndael</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"iv\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:base64Binary\">fQmPIq/dv84m6ehDzC3OLg==</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"key\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:base64Binary\">TYKtCDSqK73mp5ey0UIcnWTknwB5JPwZa1M8hZgpncw=</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"next\">");
                }
                //_s.AppendLine("<Value xsi:type=\"ReferenceObject\" id=\"0`1:E82931960FB340BA841F077CAB480132`0\">");
                _s.AppendLine("<Value xsi:type=\"ReferenceObject\" id=\"0`1:D4377FFF88FE46438D6CFB5A77C9CAAA`0\">");
                _s.AppendLine("<Attribute id=\"9F4C608A9A6D44FFAD8A2FDC662E185B\" value=\"writer\" />");
                _s.AppendLine("<Parameter id=\"id\">");
                _s.AppendLine("<Value xsi:type=\"xsd:string\">writer</Value>");
                _s.AppendLine("</Parameter>");
                //_s.AppendLine("<Parameter id=\"file\">");
                //_s.AppendLine("<Value xsi:type=\"xsd:string\">" + Path.Combine(this._root, "output" + Path.GetExtension(this._file)) + "</Value>");
                //_s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"temp\">");
                _s.AppendLine("<Value xsi:type=\"xsd:string\">" + Path.Combine(this._root, "temp.writer") + "</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"debug\">");
                _s.AppendLine("<Value xsi:type=\"xsd:boolean\">false</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("</Value>");
                for (int _n = this._stages; _n >= 1; _n--)
                {
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"async\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:string\">" + this._async + "</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"temp\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:string\">" + Path.Combine(this._root, "temp.stage_") + _n.ToString("00") + "</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("<Parameter id=\"debug\">");
                    _s.AppendLine("<Value xsi:type=\"xsd:boolean\">false</Value>");
                    _s.AppendLine("</Parameter>");
                    _s.AppendLine("</Value>");
                }
                _s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"temp\">");
                _s.AppendLine("<Value xsi:type=\"xsd:string\">" + Path.Combine(this._root, "temp.reader")  + "</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("<Parameter id=\"debug\">");
                _s.AppendLine("<Value xsi:type=\"xsd:boolean\">false</Value>");
                _s.AppendLine("</Parameter>");
                _s.AppendLine("</Object>");
                _s.AppendLine("</Root>");

                this._objectfile = Path.Combine(this._root, "experiment.liveobject");
                using (StreamWriter _w = new StreamWriter(this._objectfile, false))
                {
                    _w.Write(_s.ToString());
                }

                this._nprocessors = Environment.ProcessorCount;
                this._perfcounter_totalcpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                this._perfcounter_totalcpu.NextValue();
                this._perfcounter_cpu = new PerformanceCounter[this._nprocessors];
                this._statistics_cpu_unsorted = new QS._qss_c_.Statistics_.Samples2D[this._nprocessors];
                this._statistics_cpu_sorted = new QS._qss_c_.Statistics_.Samples2D[this._nprocessors];
                for (int _n = 0; _n < this._nprocessors; _n++)
                {
                    this._perfcounter_cpu[_n] = new PerformanceCounter("Processor", "% Processor Time", _n.ToString());
                    this._perfcounter_cpu[_n].NextValue();
                    this._statistics_cpu_unsorted[_n] = 
                        new QS._qss_c_.Statistics_.Samples2D(
                            "cpu utilization (unsorted)", "cpu utilization on core " + _n.ToString(), "time", "s", "time in seconds", "utilization", "%", "% cpu utilization");
                    this._statistics_cpu_sorted[_n] =
                        new QS._qss_c_.Statistics_.Samples2D(
                            "cpu utilization (sorted)", "cpu utilization on " + _n.ToString() + "-th busiest core", "time", "s", "time in seconds", "utilization", "%", "% cpu utilization");
                }
                this._cpu = new double[this._nprocessors];

                this._performancealarm = this._platform.AlarmClock.Schedule(0.1, new QS.Fx.Clock.AlarmCallback(this._PerformanceCallback), null);

                this._process = new Process();
                this._process.StartInfo.FileName = @"C:\liveobjects\bin\liveobjects.exe";
                this._process.StartInfo.WorkingDirectory = this._root;
                this._process.StartInfo.Arguments = this._options + " " + this._objectfile;
                this._process.Start();

                this._time0 = this._platform.Clock.Time;
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log(this._id + "._Dispose");
#endif

            if (Interlocked.CompareExchange(ref this._terminated, 1, 0) == 0)
            {
                lock (this)
                {
                    this._process.CloseMainWindow();
                    this._process.WaitForExit();
                    this._process.Close();
                    this._performancealarm = null;
                    using (StreamWriter _w = new StreamWriter(Path.Combine(this._root, "totalcpu.txt"), false))
                    {
                        foreach (QS._core_e_.Data.XY _sample in this._statistics_totalcpu.Samples)
                        {
                            _w.Write(_sample.x.ToString());
                            _w.Write("\t");
                            _w.WriteLine(_sample.y.ToString());
                        }
                    }
                    using (StreamWriter _ww = new StreamWriter(Path.Combine(this._root, "unsorted_balance.txt"), false))
                    {
                        for (int _n = 0; _n < this._nprocessors; _n++)
                        {
                            double _a = 0;
                            double _b = 0;
                            int _c = 0;
                            using (StreamWriter _w = new StreamWriter(Path.Combine(this._root, "unsorted_core_" + _n.ToString() + ".txt"), false))
                            {
                                foreach (QS._core_e_.Data.XY _sample in this._statistics_cpu_unsorted[_n].Samples)
                                {
                                    _w.Write(_sample.x.ToString());
                                    _w.Write("\t");
                                    _w.WriteLine(_sample.y.ToString());
                                    _a += _sample.y;
                                    _b += _sample.y * _sample.y;
                                    _c++;
                                }
                            }
                            double _x = _a / ((double)_c);
                            double _y = 1.96 * Math.Sqrt((_b - _a * _x) / (((double)_c) * ((double)(_c - 1))));
                            _ww.WriteLine(_n.ToString() + "\t" + _x.ToString() + "\t" + _y.ToString());
                        }
                    }
                    using (StreamWriter _ww = new StreamWriter(Path.Combine(this._root, "sorted_balance.txt"), false))
                    {
                        for (int _n = 0; _n < this._nprocessors; _n++)
                        {
                            double _a = 0;
                            double _b = 0;
                            int _c = 0;
                            using (StreamWriter _w = new StreamWriter(Path.Combine(this._root, "sorted_core_" + _n.ToString() + ".txt"), false))
                            {
                                foreach (QS._core_e_.Data.XY _sample in this._statistics_cpu_sorted[_n].Samples)
                                {
                                    _w.Write(_sample.x.ToString());
                                    _w.Write("\t");
                                    _w.WriteLine(_sample.y.ToString());
                                    _a += _sample.y;
                                    _b += _sample.y * _sample.y;
                                    _c++;
                                }
                            }
                            double _x = _a / ((double)_c);
                            double _y = 1.96 * Math.Sqrt((_b - _a * _x) / (((double)_c) * ((double)(_c - 1))));
                            _ww.WriteLine(_n.ToString() + "\t" + _x.ToString() + "\t" + _y.ToString());
                        }
                    }
                    StreamReader[] _readers = new StreamReader[this._stages + 3];
                    StreamWriter[] _writers = new StreamWriter[this._stages + 2];
                    _readers[0] = new StreamReader(Path.Combine(this._root, "temp.reader\\read.txt"));
                    _writers[0] = new StreamWriter(Path.Combine(this._root, "temp.reader\\in.txt"));
                    for (int _n = 0; _n < this._stages; _n++)
                    {
                        _readers[_n + 1] = new StreamReader(Path.Combine(this._root, "temp.stage_" + (_n + 1).ToString("00") + "\\read.txt"));
                        _writers[_n + 1] = new StreamWriter(Path.Combine(this._root, "temp.stage_" + (_n + 1).ToString("00") + "\\in.txt"));
                    }
                    _readers[this._stages + 1] = new StreamReader(Path.Combine(this._root, "temp.writer\\read.txt"));
                    _readers[this._stages + 2] = new StreamReader(Path.Combine(this._root, "temp.writer\\written.txt"));
                    _writers[this._stages + 1] = new StreamWriter(Path.Combine(this._root, "temp.writer\\in.txt"));
                    bool[] _ready = new bool[this._stages + 3];
                    bool[] _finished = new bool[this._stages + 3];
                    QS._core_e_.Data.XY[] _data = new QS._core_e_.Data.XY[this._stages + 3];
                    for (int _n = 0; _n < this._stages + 3; _n++)
                    {
                        _ready[_n] = false;
                        _finished[_n] = false;
                        _data[_n] = default(QS._core_e_.Data.XY);
                    }
                    char[] _separators = new char[] { ' ', '\t' };
                    while (true)
                    {
                        bool _found = false;
                        int _smallest = -1;
                        for (int _n = 0; _n < this._stages + 3; _n++)
                        {
                            if (!_finished[_n])
                            {
                                if (!_ready[_n])
                                {
                                    string _line = _readers[_n].ReadLine();
                                    if ((_line != null) && ((_line = _line.Trim()).Length > 0))
                                    {
                                        int _separator = _line.IndexOfAny(_separators);
                                        if ((_separator < 0) || (_separator >= _line.Length))
                                            throw new Exception("Cannot parse input.");
                                        _data[_n] = new QS._core_e_.Data.XY(Convert.ToDouble(_line.Substring(0, _separator)), Convert.ToDouble(_line.Substring(_separator)));
                                        _ready[_n] = true;
                                    }
                                    else
                                        _finished[_n] = true;
                                }

                                if (_ready[_n])
                                {
                                    if (!_found)
                                    {
                                        _smallest = _n;
                                        _found = true;
                                    }
                                    else
                                    {
                                        if (_data[_n].x < _data[_smallest].x)
                                            _smallest = _n;
                                    }
                                }
                            }
                        }
                        if (_found)
                        {
                            if (_smallest > 0)
                            {
                                _writers[_smallest - 1].Write(_data[_smallest].x.ToString());
                                _writers[_smallest - 1].Write("\t");
                                _writers[_smallest - 1].Write((_data[_smallest - 1].y - _data[_smallest].y).ToString());
                                _writers[_smallest - 1].WriteLine();
                            }
                            if (_smallest < this._stages + 2)
                            {
                                _writers[_smallest].Write(_data[_smallest].x.ToString());
                                _writers[_smallest].Write("\t");
                                _writers[_smallest].Write((_data[_smallest].y - _data[_smallest + 1].y).ToString());
                                _writers[_smallest].WriteLine();
                            }
                            _ready[_smallest] = false;
                        }
                        else
                            break;
                    }
                    foreach (StreamReader _reader in _readers)
                        _reader.Dispose();
                    foreach (StreamWriter _writer in _writers)
                    {
                        _writer.Flush();
                        _writer.Dispose();
                    }
                    for (int _n = 0; _n < this._stages; _n++)
                    {
                        StreamReader _r = new StreamReader(Path.Combine(this._root, "temp.stage_" + (_n + 1).ToString("00") + "\\written.txt"));
                        StreamWriter _w = new StreamWriter(Path.Combine(this._root, "temp.stage_" + (_n + 1).ToString("00") + "\\throughput.txt"));
                        bool _haslast = false;
                        QS._core_e_.Data.XY _lastone = new QS._core_e_.Data.XY();
                        while (true)
                        {
                            string _line = _r.ReadLine();
                            if ((_line != null) && ((_line = _line.Trim()).Length > 0))
                            {
                                int _separator = _line.IndexOfAny(_separators);
                                if ((_separator < 0) || (_separator >= _line.Length))
                                    throw new Exception("Cannot parse input.");
                                QS._core_e_.Data.XY _thisone =
                                    new QS._core_e_.Data.XY(Convert.ToDouble(_line.Substring(0, _separator)), Convert.ToDouble(_line.Substring(_separator)));
                                if (_haslast)
                                {
                                    if (_thisone.x > _lastone.x + 0.1)
                                    {
                                        double _midtime = (_lastone.x + _thisone.x) / 2;
                                        double _throughput = (_thisone.y - _lastone.y) / (_thisone.x - _lastone.x);
                                        _w.Write(_midtime.ToString());
                                        _w.Write("\t");
                                        _w.Write(_throughput.ToString());
                                        _w.WriteLine();
                                        _lastone = _thisone;
                                    }
                                }
                                else
                                {
                                    _haslast = true;
                                    _lastone = _thisone;
                                }
                            }
                            else
                                break;
                        }
                        _r.Dispose();
                        _w.Flush();
                        _w.Dispose();
                    }
                    base._Dispose();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      

        #region _PerformanceCallback

        private void _PerformanceCallback(QS.Fx.Clock.IAlarm _alarm)
        {
            lock (this)
            {
                double _time = this._platform.Clock.Time;
                if (ReferenceEquals(_alarm, this._performancealarm))
                {
                    this._totalcpu = this._perfcounter_totalcpu.NextValue();
                    for (int _n = 0; _n < this._nprocessors; _n++)
                        this._cpu[_n] = this._perfcounter_cpu[_n].NextValue();
                    if (_time > this._time0 + this._warmup)
                    {
                        this._statistics_totalcpu.Add(_time, _totalcpu);
                        for (int _n = 0; _n < this._nprocessors; _n++)
                            this._statistics_cpu_unsorted[_n].Add(_time, this._cpu[_n]);
                        Array.Sort<double>(this._cpu);
                        Array.Reverse(this._cpu);
                        for (int _n = 0; _n < this._nprocessors; _n++)
                            this._statistics_cpu_sorted[_n].Add(_time, this._cpu[_n]);
                    }
                    if (!_alarm.Cancelled)
                        _alarm.Reschedule();
                }
                if (((this._process != null) && (this._process.HasExited)) || (_time > this._time0 + this._warmup + this._duration))
                {
                    Thread.Sleep(0);
                    this._Dispose();
                    Process.GetCurrentProcess().CloseMainWindow();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      
    }
}
