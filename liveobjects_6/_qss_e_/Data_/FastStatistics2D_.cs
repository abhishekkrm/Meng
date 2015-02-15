/*
Copyright (c) 2009 Krzysztof Ostrowski, Chuck Sakoda. All rights reserved.
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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_e_.Data_
{
    public sealed class FastStatistics2D_ : QS.Fx.Inspection.Inspectable, IDisposable
    {
        #region Constructor

        public FastStatistics2D_(int _maxcount)
        {
            this._maxcount = _maxcount;
            this._samples = new QS._core_e_.Data.XY[_maxcount];
        }

        #endregion

        #region Destructor

        ~FastStatistics2D_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
                if (_disposemanagedresources)
                    this._Dispose();
        }

        private void _Dispose()
        {
            //DateTime _timestamp = DateTime.Now;
            //Random random = new Random();
            //using (StreamWriter writer = new StreamWriter(@"C:\PROFILE_FINE_GRAINED_1_SAMPLES_MAP_" +
            //    _timestamp.Year.ToString("0000") + _timestamp.Month.ToString("00") + _timestamp.Day.ToString("00") + _timestamp.Hour.ToString("00")
            //    + _timestamp.Minute.ToString("00") + _timestamp.Second.ToString("00") + random.Next(1000000).ToString("000000") + ".txt"))
            //{
            //    for (int i = 0; i < this.profile_fine_grained_1_sample_count; i++)
            //        writer.WriteLine(this.profile_fine_grained_1_samples[i].ToString());
            //}
        }

        #endregion

        public QS._core_e_.Data.XY[] Samples
        {
            get
            {
                return this._samples;
            }
        }

        #region Fields

        private int _maxcount;
        private int _disposed;
        private int _count;
        private QS._core_e_.Data.XY[] _samples;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _SAMPLES
        {
            get
            {
                int _mycount = Math.Min(this._count, this._maxcount);
                QS._core_e_.Data.XY[] copy = new QS._core_e_.Data.XY[_mycount];
                if (_mycount > 0)
                    Array.Copy(this._samples, copy, _mycount);
                QS._qss_c_.Statistics_.Samples2D _s = new QS._qss_c_.Statistics_.Samples2D(
                            "SAMPLES",
                            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                foreach(QS._core_e_.Data.XY _xy in copy) {
                    _s.Add(_xy.x,_xy.y);
                }
                return _s;
            }
        }

        #endregion

        #region Add

        public void Add(double _x, double _y)
        {
            int _i = Interlocked.Increment(ref this._count) - 1;
            if (_i < this._maxcount)
                this._samples[_i] = new QS._core_e_.Data.XY(_x, _y);
            else
                throw new ArgumentOutOfRangeException("Already taken max number of samples, " + this._maxcount);
        }

        #endregion

        #region Mean

        public double Mean
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

