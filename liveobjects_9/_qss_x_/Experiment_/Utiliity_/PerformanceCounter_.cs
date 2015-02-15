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
using System;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Utility_
{
    public sealed class PerformanceCounter_ : QS.Fx.Inspection.Inspectable
    {
        public PerformanceCounter_(string category, string counter, string instance)
        {
            this._category = category;
            this._counter = counter;
            this._instance = instance;
            this._pc = new PerformanceCounter(category, counter, instance);
            PerformanceCounterCategory a;

            this._samples = new QS._qss_c_.Statistics_.Samples2D(category +", "+counter+", "+instance);
        }

        //public PerformanceCounter_(string category, string counter, string instance, bool sortable): this(category,counter,instance)
        //{
        //    this._sortable = sortable;
        //}

        //public PerformanceCounter_(string category, string counter, string instance, bool duplicate)
        //    : this(category, counter, instance)
        //{
        //    this._duplicate = duplicate;
        //}

        public PerformanceCounter_(string category, string counter, string instance, bool sortable, bool duplicate)
            : this(category, counter, instance)
        {
            this._duplicate = duplicate;
            this._sortable = sortable;
        }

        string _category, _counter, _instance;
        PerformanceCounter _pc;
        bool _init = false;
        bool _sortable = false;
        bool _duplicate = false;

        public double Max
        {
            get
            {
                double _max = double.NegativeInfinity;
                foreach (QS._core_e_.Data.XY _data in _samples.Samples)
                {
                    if (_data.y > _max)
                    {
                        _max = _data.y;
                    }
                }
                return _max;
            }
            
        }

        public double Average
        {
            get
            {
                double _sum = 0;
                foreach (QS._core_e_.Data.XY _data in _samples.Samples)
                {
                    _sum += _data.y;
                }
                _sum /= _samples.Samples.Length;
                return _sum;
            }
        }

        public double DuplicateAvg
        {
            get
            {
                double _sum = 0;
                int _count = 0;
                double _last = double.NegativeInfinity;
                foreach (QS._core_e_.Data.XY _data in _samples.Samples)
                {
                    if (_last == _data.y)
                        continue;
                    _sum += _data.y;
                    _count++;
                    _last = _data.y;
                }
                _sum /= _count;
                return _sum;
            }
        }

        public void Init()
        {
            if (!_init)
            {
                _init = true;
                _pc.NextValue();
            }
        }

        public void Sample(double _t)
        {
            try
            {
                this._samples.Add(_t, (double)this._pc.NextValue());
            }
            catch (Exception e)
            {

            }
        }


        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _samples;

        public string Name
        {
            get
            {
                return _category + "/" + _counter + "/" + _instance;
            }
        }

        public QS._qss_c_.Statistics_.Samples2D Samples
        {
            get
            {
                return _samples;
            }
        }

        public bool Sortable
        {
            get
            {
                return this._sortable;
            }
        }
        public bool TrimDuplicates
        {
            get
            {
                return this._duplicate;
            }
        }
    }
}
